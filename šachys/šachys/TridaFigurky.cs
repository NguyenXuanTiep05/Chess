using System.Collections.Generic;
using System.Drawing;

namespace šachys
{
    internal class TridaFigurky
    {
        Dictionary<string, Image> CestakFigurkam = new Dictionary<string, Image>()
        {
            {"k", šachys.Properties.Resources.KingB},
            {"K", šachys.Properties.Resources.KingW},
            {"q", šachys.Properties.Resources.QueenB},
            {"Q", šachys.Properties.Resources.QueenW},
            {"b", šachys.Properties.Resources.BishopB},
            {"B", šachys.Properties.Resources.BishopW},
            {"r", šachys.Properties.Resources.RookB},
            {"R", šachys.Properties.Resources.RookW},
            {"p", šachys.Properties.Resources.PawnB},
            {"P", šachys.Properties.Resources.PawnW},
            {"n", šachys.Properties.Resources.KnightB},
            {"N", šachys.Properties.Resources.KnightW},

        };
        public Image rozpoznavaniFigurek(string kodFigurky)
        {
            return CestakFigurkam[kodFigurky];
        }
    }
}
