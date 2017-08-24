using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcInternetApplication.Controllers
{
    public class OxfordIntermediate
    {
        public int[] lB = new int[11];//номер уроку
        public int[] eB = new int[11];//номер вправи
        public int[] nB = new int[11];//номер відповіді
        public string[] tB = new string[11];//текст із відповіддю

        public OxfordIntermediate()
        {
            //відповіді:
            lB[0] = 1; eB[0] = 1; nB[0] = 1; tB[0] = "gist";
            lB[1] = 1; eB[1] = 1; nB[1] = 2; tB[1] = "identify";
            lB[2] = 1; eB[2] = 1; nB[2] = 3; tB[2] = "construct";
            lB[3] = 1; eB[3] = 1; nB[3] = 4; tB[3] = "foreign";
            lB[4] = 1; eB[4] = 1; nB[4] = 5; tB[4] = "context";
            lB[5] = 1; eB[5] = 1; nB[5] = 6; tB[5] = "translate";

            lB[6] = 80; eB[6] = 1; nB[6] = 1; tB[6] = "sidewalk";
            lB[7] = 80; eB[7] = 1; nB[7] = 2; tB[7] = "cellphone";
            lB[8] = 80; eB[8] = 1; nB[8] = 3; tB[8] = "datebook";
            lB[9] = 80; eB[9] = 1; nB[9] = 4; tB[9] = "drugstore";
            lB[10] = 80; eB[10] = 1; nB[10] = 5; tB[10] = "railroad";
        }
    }
}