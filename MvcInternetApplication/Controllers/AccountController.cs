﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MvcInternetApplication.Models;
using MvcInternetApplication.Filters;

namespace MvcInternetApplication.Controllers
{
    [Authorize]
    [InitializeSimpleMembershipAttribute]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)//вибираємо залогінитись
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //покладемо у поле імя користувача, що залогінився
        string nameUser;

        //
        // POST: /Account/Login


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)//підтвердження уведених даних
        {
            // WebSecurity.Login - аутентифицирует пользователя.
            // Если логин и пароль введены правильно - метод возвращает значение true после чего выполняет добавление специальных значений в cookies.
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                if (model.UserName == "NazarAdmin")
                {
                    ViewBag.Autoriz = "admin";
                    nameUser = (string)model.UserName;
                }
                return RedirectToLocal(returnUrl);
            }

            // Был введен не правильный логин или пароль
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            // Удаление специальных аутентификационных cookie значений
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        //[AllowAnonymous]

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    // Создание пользователя
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    //добавляємо роль
                    SimpleRoleProvider roles = (SimpleRoleProvider)Roles.Provider;
                    roles.AddUsersToRoles(new[] { model.UserName }, new[] { "JustUser" }); // установка роли для пользователя

                    // Аутентификация пользователя
                    //WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("AdminPanel", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)//вікно після зміненого пароля
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        public ActionResult ManageForChange()//для входження у вікно де змінюється пароль
        {
            string nameus = "";
            nameus = Request.Form["mesage"];//значення прийшло від поля вводу пошуку уроку
            ViewBag.UserChangePassword = nameus;
            return View("Manage");
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)//вікно із формами вводу для зміни пароля
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    // ChangePassword выбрасывает исключения в случае не удачной попытки смены пароля.
                    //changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    changePasswordSucceeded = WebSecurity.ChangePassword(model.NameForChange, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)//якщо зміна паролю пройшла успішно...
                {
                    //перенаправляємо знову на контролер Manage із відповідною перегрузкою (аргумент типу ManageMessageId)
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion


    }
}

