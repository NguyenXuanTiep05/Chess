using System.Collections.Generic;
using System.Windows.Forms;

namespace šachys
{
    internal class SachmatRemiza
    {
        detekceSachu detekceSachu = new detekceSachu();

        public bool maOponentPlatnePohyby(int[,] stavPole, Button[,] hraciPole, bool bilyNaRade)
        {
            List<int> platneTahy = new List<int>();
            Button[,] temphraciPole = new Button[8, 8];
            for (int y = 0; y < hraciPole.GetLength(0); y++)
            {
                for (int x = 0; x < hraciPole.GetLength(1); x++)
                {
                    temphraciPole[y, x] = new Button();
                    temphraciPole[y, x].Tag = hraciPole[y, x].Tag;
                    temphraciPole[y, x].BackgroundImage = hraciPole[y, x].BackgroundImage;
                }
            }
            int[,] tempStavPole = (int[,])stavPole.Clone();

            foreach (Button button in temphraciPole)
            {
                if (button.Tag != null)
                {
                    List<int> temp = detekceSachu.tahyProtiSachuMatu(tempStavPole, temphraciPole, bilyNaRade, button);
                    foreach (int y in temp)
                    {
                        platneTahy.Add(y);
                    }
                }
            }

            if (platneTahy.Count == 0)
            {
                return true;
            }

            return false;
        }

        public bool nedostatekMaterialu(Button[,] hraciPole)
        {
            int WB = 0;
            int BB = 0;
            int WN = 0;
            int BN = 0;
            int bileFigurky = 0;
            int cerneFigurky = 0;

            foreach (Button t in hraciPole)
            {
                if (t.Tag != null)
                {
                    string piece = t.Tag.ToString();
                    switch (piece)
                    {
                        case "K":
                            bileFigurky++;
                            break;
                        case "k":
                            cerneFigurky++;
                            break;
                        case "B":
                            WB++;
                            bileFigurky++;
                            break;
                        case "b":
                            BB++;
                            cerneFigurky++;
                            break;
                        case "N":
                            WN++;
                            bileFigurky++;
                            break;
                        case "n":
                            BN++;
                            cerneFigurky++;
                            break;
                        default:
                            return false;
                    }
                }
            }

            if (bileFigurky == 1 && cerneFigurky == 1)
            {
                return true;
            }
            if (bileFigurky == 2 && cerneFigurky == 1 && (WB == 1 || WN == 1))
            {
                return true;
            }
            if (bileFigurky == 1 && cerneFigurky == 2 && (BB == 1 || BN == 1))
            {
                return true;
            }
            if (bileFigurky == 2 && cerneFigurky == 2 && WB == 1 && BB == 1)
            {
                return true;
            }
            return false;
        }


    }
}
