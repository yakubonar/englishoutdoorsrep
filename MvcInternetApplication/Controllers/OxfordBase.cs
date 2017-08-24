//крайні виправлення: 08082017
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcInternetApplication.Controllers
{
    public class OxfordBase
    {
        public int[] lB = new int[14];//номер уроку
        public int[] eB = new int[14];//номер вправи
        public int[] nB = new int[14];//номер відповіді
        public string[] tB = new string[14];//текст із відповіддю

        public OxfordBase()//конструктор ініціалізації БД
        {
            //відповіді:
            lB[0] = 1; eB[0] = 1; nB[0] = 1; tB[0] = "two hundred";
            lB[1] = 1; eB[1] = 1; nB[1] = 2; tB[1] = "three hundred and forty";
            lB[2] = 1; eB[2] = 1; nB[2] = 3; tB[2] = "twenty-two";
            lB[3] = 1; eB[3] = 1; nB[3] = 4; tB[3] = "forty-two thousand five hundred";
            lB[4] = 1; eB[4] = 1; nB[4] = 5; tB[4] = "one thousand two hundred";
            lB[5] = 1; eB[5] = 1; nB[5] = 6; tB[5] = "two thousand three hundred and fifty";
            lB[6] = 80; eB[6] = 3; nB[6] = 1; tB[6] = "get a/the bus";
            lB[7] = 80; eB[7] = 3; nB[7] = 2; tB[7] = "get here/home/back";
            lB[8] = 80; eB[8] = 3; nB[8] = 3; tB[8] = "get them";
            lB[9] = 80; eB[9] = 3; nB[9] = 4; tB[9] = "getting cold";
            lB[10] = 80; eB[10] = 3; nB[10] = 5; tB[10] = "getting late";
            lB[11] = 80; eB[11] = 3; nB[11] = 6; tB[11] = "get it";
            lB[12] = 80; eB[12] = 3; nB[12] = 7; tB[12] = "get one";
            lB[13] = 80; eB[13] = 3; nB[13] = 8; tB[13] = "get a bus/taxi";
        }
    }
}