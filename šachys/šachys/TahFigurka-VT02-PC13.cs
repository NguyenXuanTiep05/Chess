using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace šachys
{
    internal class TahFigurka
    {
        Dictionary<string, int[]> legalniTahy = new Dictionary<string, int[]>()
        {
            {"p", [1,0]},
            {"P", [-1,0]},
            {"k", [0,-1,0,1,1,0,-1,0,-1,1,1,1,1,-1,-1,-1]},
            {"q", [-8]},
            {"b", [-1,1,-2,2,-3,3,-4,4,-5,5,-6,6,-7,7  ,1,-1,2,-2,3,-3,4,-4,5,-5,6,-6,7,-7  ,-1,-1,-2,-2,-3,-3,-4,-4,-5,-5,-6,-6,-7,-7  ,1,1,2,2,3,3,4,4,5,5,6,6,7,7]},
            {"r", [-1,0,-2,0,-3,0,-4,0,-5,0,-6,0,-7,0  ,1,0,2,0,3,0,4,0,5,0,6,0,7,0  ,0,1,0,2,0,3,0,4,0,5,0,6,0,7  ,0,-1,0,-2,0,-3,0,-4,0,-5,0,-6,0,-7]},
            {"n", [-8]},
        };
        public List<int> vypocetTahu(int[] cisloAktualnihoPole, string kodFigurky)
        {
            if (kodFigurky != "p" && kodFigurky != "P")
            {
                kodFigurky = kodFigurky.ToLower();
            }
            List<int> tahy = new List<int>();

            for (int i = 0; i < legalniTahy[kodFigurky].Length; i+= 2)
            {
                if (cisloAktualnihoPole[0] + legalniTahy[kodFigurky][i] < 8 && cisloAktualnihoPole[0] + legalniTahy[kodFigurky][i] >=0)
                {
                    if (cisloAktualnihoPole[1] + legalniTahy[kodFigurky][i + 1] < 8 && cisloAktualnihoPole[1] + legalniTahy[kodFigurky][i + 1] >= 0) 
                    {
                        tahy.Add(cisloAktualnihoPole[0] + legalniTahy[kodFigurky][i]);
                        tahy.Add(cisloAktualnihoPole[1] + legalniTahy[kodFigurky][i + 1]);
                    }
                }
            }
            return tahy;
        }
    }
}
