using System;
using System.Collections.Generic;
using System.Linq;
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
            {"k", [-1,1, 8,-8, 9,-9]},
            {"K", [-1,1, 8,-8, 9,-9]},
            {"q", [-8]},
            {"Q", [-8]},
            {"b", [-8]},
            {"B", [8]},
            {"r", [-1,-2,-3,-4,-5,-6,-7, 1,2,3,4,5,6,7]},
            {"R", [-1,-2,-3,-4,-5,-6,-7, 1,2,3,4,5,6,7]},
            {"--", [-8]},
            {"---", [-8]},
        };
        public List<int> vypocetTahu(int[] cisloAktualnihoPole, string kodFigurky)
        {
            List<int> tahy = new List<int>();
            for (int i = 0; i < legalniTahy[kodFigurky].Length; i++) {
                tahy.Add(legalniTahy[kodFigurky][i]);
            }

            for (int i = 0; i < tahy.Count - 1; i++)
            {
                if (cisloAktualnihoPole[0] + tahy[i] < 8 && cisloAktualnihoPole[0] + tahy[i] >= 0)
                {
                    tahy[i] = cisloAktualnihoPole[0] + tahy[i];
                }
                tahy[i + 1] = cisloAktualnihoPole[1] + tahy[i + 1];
            }
            return tahy;
        }
    }
}
