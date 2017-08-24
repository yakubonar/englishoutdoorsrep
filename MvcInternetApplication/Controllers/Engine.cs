using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MvcInternetApplication.Controllers
{
    public class Engine
    {
        OxfordBase oxfb = new OxfordBase();
        OxfordIntermediate oxfInterm = new OxfordIntermediate();
        public string lessonsText = "1";
        public string exerciseText = "1";

        public int killkisttAnswer = 0;//скільки пунктів у прикладі
        public List<int> riadokAnswerModel = new List<int>();//номер рядка у екселі із відповіддю
        public string exersiceNumber;//номер блоку

        #region Base
        void SearchLessonOxBase(string nomer)//шукаємо урок, кількість вправ і кількість кнопок через OxfordBase
        {
            //шукаємо вправу із масивів OxfordBase
            int lesson = 0;//, nButtn = 0, indexM = 0;
            lesson = Convert.ToInt32(nomer);
            quantImage = 1;//oxfb.imageQ[lesson];//кількість зображень у даному уроці
            for (int i = 0; i < oxfb.lB.Length; i++)
            {
                if (oxfb.lB[i] == lesson)//шукаємо урок/вправу
                {
                    DetermQuantExercBase(lesson);
                    lessonsText = lesson.ToString();//теперішній урок (вивід на сторінку)
                    nextLesson = NextLesson(lesson, 80);//наступний урок
                    backLesson = BackLesson(lesson, 80);//теперішній урок, максимальний урок
                    break;
                }
            }
        }
        void DetermQuantExercBase(int lesson)//визначаємо назви вправ, їх кількість кількість відповідей для кожної вправи
        {
            string calledName = "", calledQuant = "", sEx = "", sAn = "";
            int calledEx = 0, quantity = 0;
            bool first = false;
            for (int i = 0; i < oxfb.lB.Length; i++)
            {
                if (oxfb.lB[i] == lesson)
                {
                    //потрібно не залежати від інкрименту, так як уроки не завжди будуть збільшуватись на 1

                    //визначаємо кількість вправ, визначаємо назви вправ
                    if (oxfb.eB[i] != calledEx)
                    {
                        calledEx = oxfb.eB[i];
                        calledName += calledEx.ToString() + "_";
                        if (first)//перший індекс у вибраному уроці пропускаємо
                            calledQuant += quantity.ToString() + "_";
                        quantity = 1;
                        first = true;
                    }
                    else
                        quantity++;
                    
                    //фіксуємо кінцевий індекс для вибраного уроку
                    if (i + 1 < oxfb.lB.Length)
                        if (oxfb.lB[i + 1] > lesson)
                            if (oxfb.lB[i] == lesson)
                                calledQuant += quantity.ToString() + "_";
                    if (i == oxfb.lB.Length - 1)//для крайнього уроку
                        calledQuant += quantity.ToString() + "_";
                }
            }
            quntExerCalled = calledName.Remove(calledName.Length - 1);//видаляємо кінцевий символ "_"
            quantAnsver = calledQuant.Remove(calledQuant.Length - 1);
            int indxExer = quntExerCalled.IndexOf("_");//індекс для виділення першого значення
            int indxAnsver = quantAnsver.IndexOf("_");
            if (indxExer == -1)
            {
                sEx = quntExerCalled;//немає символу "_", тобто на уроці тільки одна вправа
                sAn = quantAnsver;
            }
            else
            {
                sEx = quntExerCalled.Substring(0, indxExer);//перше значення у рядку
                sAn = quantAnsver.Substring(0, indxAnsver);
            }
            int firstEx = Convert.ToInt32(sEx);//перша вправа на уроці
            killkisttAnswer = Convert.ToInt32(sAn);//кількість відповідей для першої вправи

            exerciseText = sEx;//номер (назва) першої вправи для даного уроку
            string[] spl = quntExerCalled.Split('_');
            quntExercise = spl.Length;//кількість вправ у вибраному уроці
        }
        string SearchAnsverFromMassyvBase(string numbLesson, string numbExercise, string numbAnsver)//розділяємо номер уроку від номера вправи
        {
            string rez = "";
            int numberEx = 0, numberLn = 0, numberAn = 0;
            numberLn = Convert.ToInt32(numbLesson);
            numberEx = Convert.ToInt32(numbExercise);
            numberAn = Convert.ToInt32(numbAnsver);
            for (int i = 0; i < oxfb.lB.Length; i++)
            {
                //шукаємо урок/вправу/номер кнопки/вибираємо текст
                if (oxfb.lB[i] == numberLn)
                    if (oxfb.eB[i] == numberEx)
                        if (oxfb.nB[i] == numberAn)
                            return rez = oxfb.tB[i];//знайшли текст із відповіддю
            }
            return rez = "Відповіді не знайдено!";
        }
        public int DetermFirstLesExBase()//визначаємо кількість відповідей у першій вправі першого уроку
        {
            int rez = 0;
            while (oxfb.eB[rez] == 1)
                rez++;
            return rez;
        }
        #endregion
        
        #region Intermediate
        void SearchLessonOxIntermed(string nomer)//шукаємо урок, кількість вправ і кількість кнопок через OxfordIntermediate
        {
            int lesson = 0;
            lesson = Convert.ToInt32(nomer);
            quantImage = 1;// oxfInterm.imageQ[lesson];//кількість зображень у даному уроці
            for (int i = 0; i < oxfInterm.lB.Length; i++)
            {
                if (oxfInterm.lB[i] == lesson)//шукаємо урок/вправу
                {
                    DetermQuantExercIntermed(lesson);
                    lessonsText = lesson.ToString();//теперішній урок (вивід на сторінку)
                    nextLesson = NextLesson(lesson, 80);//наступний урок
                    backLesson = BackLesson(lesson, 80);//теперішній урок, максимальний урок
                    break;
                }
            }
        }
        void DetermQuantExercIntermed(int lesson)//визначаємо назви вправ, їх кількість кількість відповідей для кожної вправи
        {
            string calledName = "", calledQuant = "", sEx = "", sAn = "";
            int calledEx = 0, quantity = 0;
            bool first = false;
            for (int i = 0; i < oxfInterm.lB.Length; i++)
            {
                if (oxfInterm.lB[i] == lesson)
                {
                    //визначаємо кількість вправ і назви вправ
                    if (oxfInterm.eB[i] != calledEx)
                    {
                        calledEx = oxfInterm.eB[i];
                        calledName += calledEx.ToString() + "_";
                        if (first)//перший індекс у вибраному уроці пропускаємо
                            calledQuant += quantity.ToString() + "_";
                        quantity = 1;
                        first = true;
                    }
                    else
                        quantity++;
                    //фіксуємо кінцевий індекс для вибраного уроку
                    if (i + 1 < oxfInterm.lB.Length)
                        if (oxfInterm.lB[i + 1] > lesson)
                            if (oxfInterm.lB[i] == lesson)
                                calledQuant += quantity.ToString() + "_";
                    if (i == oxfInterm.lB.Length - 1)//для крайнього уроку
                        calledQuant += quantity.ToString() + "_";
                }
            }
            quntExerCalled = calledName.Remove(calledName.Length - 1);//видаляємо кінцевий символ "_"
            quantAnsver = calledQuant.Remove(calledQuant.Length - 1);
            int indxExer = quntExerCalled.IndexOf("_");//індекс для виділення першого значення
            int indxAnsver = quantAnsver.IndexOf("_");
            if (indxExer == -1)
            {
                sEx = quntExerCalled;//немає символу "_", тобто на уроці тільки одна вправа
                sAn = quantAnsver;
            }
            else
            {
                sEx = quntExerCalled.Substring(0, indxExer);//перше значення у рядку
                sAn = quantAnsver.Substring(0, indxAnsver);
            }
            int firstEx = Convert.ToInt32(sEx);//перша вправа на уроці
            killkisttAnswer = Convert.ToInt32(sAn);//кількість відповідей для першої вправи

            exerciseText = sEx;//номер (назва) першої вправи для даного уроку
            string[] spl = quntExerCalled.Split('_');
            quntExercise = spl.Length;//кількість вправ у вибраному уроці
        }
        string SearchAnsverFromMassyvIntermed(string numbLesson, string numbExercise, string numbAnsver)//розділяємо номер уроку від номера вправи
        {
            string rez = "";
            int numberEx = 0, numberLn = 0, numberAn = 0;
            numberLn = Convert.ToInt32(numbLesson);
            numberEx = Convert.ToInt32(numbExercise);
            numberAn = Convert.ToInt32(numbAnsver);
            for (int i = 0; i < oxfInterm.lB.Length; i++)
            {
                //шукаємо урок/вправу/номер кнопки/вибираємо текст
                if (oxfInterm.lB[i] == numberLn)
                    if (oxfInterm.eB[i] == numberEx)
                        if (oxfInterm.nB[i] == numberAn)
                            return rez = oxfInterm.tB[i];//знайшли текст із відповіддю
            }
            return rez = "Відповіді не знайдено!";
        }
        public int DetermFirstLesExIntermed()//визначаємо кількість відповідей у першій вправі першого уроку
        {
            int rez = 0;
            //while (oxfInterm.eB[rez] == 1)
                //rez++;
            return rez = 6;
        }
        #endregion

        public void Search(string level, string nomer)//Знаходимо заданий юніт, якщо він є (підручник, номер вправи)
        {
            if (level == "OxfordBase")
                SearchLessonOxBase(nomer);//шукаємо вправу із масивів OxfordBase
            if (level == "OxfordIntermed")
                SearchLessonOxIntermed(nomer);//шукаємо вправу із масивів OxfordBase
        }
        
        string NextLesson(int lesson, int lessonMax)
        {
            string rez = "";
            if (lesson == 1)
                rez = 80.ToString();
            else
                rez = 1.ToString();
            return rez;
        }
        string BackLesson(int lesson, int lessonMax)
        {
            string rez = "";
            if (lesson == 1)
                rez = 80.ToString();
            else
                rez = 1.ToString();
            return rez;
        }
        
        public string quntExerCalled;//рядок із кількостями вправ і їх назвою
        public string quantAnsver;//кількостями відповідей до кожної вправи

        public int quantImage;//кількість зображень у вибраному уроці
        public int quntExercise;//кількість вправ у вибраному уроці
        public string backLesson;//попередній урок за списком
        public string nextLesson;//наступний урок за списком
        List<string> listAnswerSplit = new List<string>();
        
        public string SimpleRequest(string book, string numbLesson, string numbExercise, string numbAnsver)//запит в БД для відповіді на кнопці
        {
            string rez = "";
            if (book == "OxfordBase")
                rez = SearchAnsverFromMassyvBase(numbLesson, numbExercise, numbAnsver);
            if (book == "OxfordIntermed")   
                rez = SearchAnsverFromMassyvIntermed(numbLesson, numbExercise, numbAnsver);
            return rez;
        }
    }
}