using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using MvcInternetApplication.Filters;
using MvcInternetApplication.Models;

namespace MvcInternetApplication.Controllers
{
    public class HomeController : Controller
    {
        //контроли EnglishOutDoors
        // GET: /Home/
        Engine eng = new Engine();

        int nAnsver;
        List<string> idButtn = new List<string>();
        List<string> idLabel = new List<string>();
        List<string> idScrip = new List<string>();
        List<string> acounts = new List<string>();
        void Filling(int n)//заповнюєимо колекції для id кнопок та ін.
        {
            idButtn.Clear();
            idLabel.Clear();
            idScrip.Clear();
            nAnsver = n;
            idButtn.Add("bttn");
            idLabel.Add("labl");
            idScrip.Add("scrp");
            for (int i = 1; i <= n; i++)
            {
                idButtn.Add("bttn" + i.ToString());
                idLabel.Add("labl" + i.ToString());
                idScrip.Add("scrp" + i.ToString());
            }
        }

        //для отримання відповіді конкретною кнопкою
        [HttpGet]
        public ActionResult GetAnsver(string id)//витягуємо текст із відповіддю
        {
            string[] words = id.Split(':');//розділяємо (книга : урок : вправа : рядок)
            string ansverDB = eng.SimpleRequest(words[0], words[1], words[2], words[3]);//номер вправи, номер відповіді, номер уроку
            if (!string.IsNullOrEmpty(ansverDB))
            {
                return Content(ansverDB);
            }
            return Content("Empty");
        }

        //контроли Login
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        void CommonFilling()//загальна заповненні viewbag
        {
            ViewBag.Exercise = "1";
            ViewBag.NextLesson = "2";
            ViewBag.BackLesson = "80";
            ViewBag.NextExercise = "1";
            ViewBag.BackExercise = "1";
            ViewBag.Lesson = "1";
        }
        void VariabFilling()//динамічна інформація для заповнення
        {
            ViewBag.Exercise = eng.exerciseText;//eng.exerciseText;//номер вправи
            ViewBag.NextLesson = eng.nextLesson;//наступний урок
            ViewBag.BackLesson = eng.backLesson;//попередня урок
            ViewBag.Lesson = eng.lessonsText;
            ViewBag.NImage = eng.quantImage;
            ViewBag.NAnsver = nAnsver;
            ViewBag.Buttons = idButtn;
            ViewBag.Labels = idLabel;
            ViewBag.Scrips = idScrip;//id для скриптів
            ViewBag.NExercise = eng.quntExercise;
            ViewBag.QuntExerAnsv = eng.quntExerCalled;//рядок із кліькостями вправ і кількостями відповідей до кожної вправи
            ViewBag.QuntExerCalled = eng.quntExerCalled;//рядок із кліькостями вправ і кількостями відповідей до кожної вправи
            ViewBag.QuantAnsver = eng.quantAnsver;//кількість відповідей для кожної вправи
        }
        [Authorize(Roles = "Admin, User")]
        public ActionResult Cabinet()
        {
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public ActionResult OxfordBase()//книга OxfordBasic
        {
            string exerc = "";
            ViewBag.Book = "OxfordBase";
            ViewBag.Directory = "base";
            CommonFilling();
            try
            {
                exerc = Request.Form["message"];//значення прийшло від поля вводу пошуку уроку
                eng.Search("OxfordBase", exerc);//пошук уроку, наступний, попередній (замість методу SearchLesson())
                if (exerc == null)//перший вхід в книгу
                    eng.Search("OxfordBase", "1");//для заходу в книгу із кабінету (перший вхід)
            }
            catch//якщо некоректний ввід 
            {
                eng.Search("OxfordBase", "1");//якщо урок не знайдений, кладемо урок 1
            }
            Filling(eng.DetermFirstLesExBase());
            VariabFilling();//переписуємо заповненими значеннями
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public ActionResult OxfordIntermed()//книга OxfordIntermediate
        {
            string exerc = "";
            ViewBag.Book = "OxfordIntermed";
            ViewBag.Directory = "intermed";
            CommonFilling();
            try
            {
                exerc = Request.Form["message"];//значення прийшло від поля вводу пошуку уроку
                eng.Search("OxfordIntermed", exerc);//пошук уроку, наступний, попередній (замість методу SearchLesson())
                if (exerc == null)//перший вхід в книгу
                    eng.Search("OxfordIntermed", "1");//для заходу в книгу із кабінету (перший вхід)
            }
            catch//якщо некоректний ввід 
            {
                eng.Search("OxfordIntermed", "1");//якщо урок не знайдений, кладемо урок 1
            }
            Filling(eng.DetermFirstLesExIntermed());
            VariabFilling();//переписуємо заповненими значеннями
            return View("OxfordBase");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminPanel()
        {
            ViewBag.Message = "Admin Panel, " + DateTime.Now.ToUniversalTime();
            FillInAllUsers();//витягуємо з БД всіх користувачів
            userToAdminPanel.Clear();
            for (int i = 1; i <= nameUserTable.Count(); i++)
            {
                userToAdminPanel.Add(new UsersModelTable()//передаємо на сторону клієнта всіх користувачів
                {
                    AllNameUser = nameUserTable[i - 1],//NameRole - поле у моделі таблиці UsersModelTable
                });
            }
            return View(userToAdminPanel);
        }

        UsersContext fromdb = new UsersContext();

        List<UsersModelTable> userToAdminPanel = new List<UsersModelTable>();
        List<int> idUsers = new List<int>();//id користувачів
        List<string> nameUserTable = new List<string>();//імена користувачів

        string ForNameUser(int index)//вводимо індекс в таблиці ролей, отримуємо імя користувача
        {
            string rez = ""; int rindx = 0;
            List<webpages_UsersInRoles> uspro = fromdb.UserInRoles.ToList();//кожна таблиця має метод, який виведе у вигляді списку всі записи в таблиці
            foreach (webpages_UsersInRoles uf in uspro)
            {
                if (uf.UserId == index)
                {
                    rindx = uf.RoleId;//знайшли індекс потрібної ролі
                    break;
                }
            }
            if (rindx == 2)//роль адмін                        
                rez = "Admin";
            if (rindx == 3)//роль user                        
                rez = "User";
            return rez;
        }
        void FillInAllUsers()//витягуємо з БД всіх користувачів
        {
            idUsers.Clear();
            nameUserTable.Clear();
            List<UserProfile> list = fromdb.UserProfiles.ToList();//кожна таблиця має метод, який виведе у вигляді списку всі записи в таблиці
            foreach (UserProfile item in list)
            {
                nameUserTable.Add(item.UserName);//колекція всіх nameUser
                idUsers.Add(item.UserId);//заклали у колекцію також id
            }
        }
        List<RoleUserModel> roleUserModel = new List<RoleUserModel>();
        void FillRolesForUsers()//витягуємо з БД ролі для користувачів
        {
            roleUserModel.Clear();
            for (int i = 1; i <= nameUserTable.Count(); i++)
            {
                roleUserModel.Add(new RoleUserModel()
                {
                    NameUser = nameUserTable[i - 1], //nameUserTable[qunty - 1]
                    NameRole = ForNameUser(idUsers[i - 1])
                });
            }
            //розписуємо всі таблиці
            List<webpages_Roles> listRoles = fromdb.TableRoles.ToList();
            string rolesAll = "";
            foreach (webpages_Roles r in listRoles)
            {
                rolesAll += r.RoleId + ", " + r.RoleName + " | ";
            }
            ViewBag.AccesRoles = rolesAll;

            List<webpages_UsersInRoles> list = fromdb.UserInRoles.ToList();//кожна таблиця має метод, який виведе у вигляді списку всі записи в таблиці
            string usersRoles = "";
            foreach (webpages_UsersInRoles u in list)
            {
                usersRoles += u.RoleId + ", " + u.UserId + " | ";
            }
            ViewBag.UsersWithRoles = usersRoles;

            List<UserProfile> listUse = fromdb.UserProfiles.ToList();//кожна таблиця має метод, який виведе у вигляді списку всі записи в таблиці
            string usersProfsl = "";
            foreach (UserProfile usl in listUse)
            {
                usersProfsl += usl.UserId + ", " + usl.UserName + " | ";
            }
            ViewBag.UsersProfill = usersProfsl;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete()//видаляє аккаунт користувача
        {
            string nameus = "";
            SimpleMembershipProvider membership = (SimpleMembershipProvider)Membership.Provider;
            SimpleRoleProvider roles = (SimpleRoleProvider)Roles.Provider;
            try
            {
                nameus = Request.Form["mesage"];//значення прийшло від поля вводу пошуку уроку
                if (membership.GetUser(nameus, false) != null)
                {
                    FillInAllUsers();//заповнили службові колекції
                    int nomId = NumberOfId(nameus);//отримали id за іменем

                    membership.DeleteAccount(nameus);//видалили дані із таблиці [webpages_Membership]

                    //видаляємо привязку користувача до ролі (якщо воно існує в таблиці webpages_UsersInRoles)
                    try
                    {
                        webpages_UsersInRoles inroles = new webpages_UsersInRoles { UserId = nomId };
                        fromdb.UserInRoles.Attach(inroles);
                        fromdb.UserInRoles.Remove(inroles);
                        fromdb.SaveChanges();
                    }
                    catch 
                    {
                    }
                    membership.DeleteUser(nameus, false);//не працює коли є роль для даного користувача
                }
            }

            catch
            {
                ViewBag.Message = "Admin Panel. Видалення не спрацювало!";
            }
            return RedirectToAction("AdminPanel", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RolePanel()//блокує доступ користувачу
        {
            ViewBag.Message = "Roles Panel, " + DateTime.Now.ToUniversalTime();
            FillInAllUsers();//передаємо на сторону клієнта всіх користувачів
            FillRolesForUsers();//заповняємо поточні ролі користувачів (тільки після методу FillInAllUsers())
            return View(roleUserModel);
        }

        //для ролей
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult LaunchRoleJustUser()//надаємо користувачу звичайні права
        {
            //провірка на існування хоч якоїсь ролі для користувача не застосовується через те, що 
            //при створенні користувача, по замовчуванню, створюється й базова роль для нього (JustUser)
            FillInAllUsers();
            string name = "";//, sqlString = "";
            name = Request.Form["mesage"];//імя юзера і назва ролі
            
            int nomId = NumberOfId(name);//отримали id за іменем
            var roll = fromdb.UserInRoles.Where(c => c.UserId == nomId).FirstOrDefault();//вибрали поле із відповідної таблиці та відповідним id
            roll.RoleId = 3;
            fromdb.SaveChanges();

            ViewBag.Message = "Roles Panel, " + DateTime.Now.ToUniversalTime();
            FillInAllUsers();//передаємо на сторону клієнта всіх користувачів
            FillRolesForUsers();//заповняємо поточні ролі користувачів (тільки після методу FillInAllUsers())
            return View("RolePanel", roleUserModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult LaunchRoleAdmin()//надаємо користувачу права адміна
        {
            //провірка на існування хоч якоїсь ролі для користувача не застосовується через те, що 
            //при створенні користувача, по замовчуванню, створюється й базова роль для нього (JustUser)
            FillInAllUsers();
            string name = "";//, sqlString = "";//, nomId = "";
            name = Request.Form["mesage"];//імя юзера і назва ролі

            int nomId = NumberOfId(name);//отримали id за іменем
            var roll = fromdb.UserInRoles.Where(c => c.UserId == nomId).FirstOrDefault();//вибрали поле із відповідної таблиці та відповідним id
            roll.RoleId = 2;
            fromdb.SaveChanges();

            ViewBag.Message = "Roles Panel, " + DateTime.Now.ToUniversalTime();
            FillInAllUsers();//передаємо на сторону клієнта всіх користувачів
            FillRolesForUsers();//заповняємо поточні ролі користувачів (тільки після методу FillInAllUsers())
            return View("RolePanel", roleUserModel);
        }
        int NumberOfId(string nameuser)//отримаємо номер id за іменем користувача
        {
            int rez = 0;
            for (int i = 0; i < nameUserTable.Count(); i++)
            {
                if (nameuser == nameUserTable[i])
                {
                    rez = idUsers[i];
                    break;
                }
            }
            return rez;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult StopRole()//блокуємо доступ до ресурсу
        {
            FillInAllUsers();
            string name = "";//, sqlString = "", nomId = "";
            name = Request.Form["mesage"];//імя юзера і назва ролі

            int nomId = Convert.ToInt32(NumberOfId(name));//отримали id за іменем
            var roll = fromdb.UserInRoles.Where(c => c.UserId == nomId).FirstOrDefault();//вибрали поле із відповідної таблиці та відповідним id
            roll.RoleId = 4;
            fromdb.SaveChanges();

            ViewBag.Message = "Roles Panel, " + DateTime.Now.ToUniversalTime();
            FillInAllUsers();//передаємо на сторону клієнта всіх користувачів
            FillRolesForUsers();//заповняємо поточні ролі користувачів (тільки після методу FillInAllUsers())
            return View("RolePanel", roleUserModel);
        }

    }
}

