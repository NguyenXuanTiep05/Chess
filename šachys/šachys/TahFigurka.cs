using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace šachys
{
    internal class TahFigurka
    {
        Dictionary<string, int[]> legalniTahy = new Dictionary<string, int[]>()
        {
            {"p", [1,0]},
            {"P", [-1,0]},
            {"p2", [1,0  ,2,0]},
            {"P2", [-1,0, -2,0]},
            {"p3", [1,-1,1,1]},
            {"P3", [-1,-1,-1,1]},
            {"k", [0,-1,0,1,1,0,-1,0,-1,1,1,1,1,-1,-1,-1]},
            {"q", [-1,0,-2,0,-3,0,-4,0,-5,0,-6,0,-7,0  ,1,0,2,0,3,0,4,0,5,0,6,0,7,0  ,0,1,0,2,0,3,0,4,0,5,0,6,0,7  ,0,-1,0,-2,0,-3,0,-4,0,-5,0,-6,0,-7  ,-1,1,-2,2,-3,3,-4,4,-5,5,-6,6,-7,7  ,1,-1,2,-2,3,-3,4,-4,5,-5,6,-6,7,-7  ,-1,-1,-2,-2,-3,-3,-4,-4,-5,-5,-6,-6,-7,-7  ,1,1,2,2,3,3,4,4,5,5,6,6,7,7]},
            {"b", [-1,1,-2,2,-3,3,-4,4,-5,5,-6,6,-7,7  ,1,-1,2,-2,3,-3,4,-4,5,-5,6,-6,7,-7  ,-1,-1,-2,-2,-3,-3,-4,-4,-5,-5,-6,-6,-7,-7  ,1,1,2,2,3,3,4,4,5,5,6,6,7,7]},
            {"r", [-1,0,-2,0,-3,0,-4,0,-5,0,-6,0,-7,0  ,1,0,2,0,3,0,4,0,5,0,6,0,7,0  ,0,1,0,2,0,3,0,4,0,5,0,6,0,7  ,0,-1,0,-2,0,-3,0,-4,0,-5,0,-6,0,-7]},
            {"n", [-1,-2,1,-2  ,-1,2,1,2  ,-2,1,-2,-1  ,2,1,2,-1]},
        };
        public List<int> vypocetTahu(int[] cisloAktualnihoPole, string kodFigurky, int[,] stavFigurek, Button[,] hraciPole)
        {
            List<int> tahy = new List<int>();
            string tempKodFigurky = kodFigurky;

            if (kodFigurky != "p" && kodFigurky != "P")
            {
                kodFigurky = kodFigurky.ToLower();
            }

            if (kodFigurky == "p" || kodFigurky == "P")
            {
                tahy = pesakCapture(cisloAktualnihoPole, kodFigurky, hraciPole);
                if (cisloAktualnihoPole[0] == 6 || cisloAktualnihoPole[0] == 1)
                {
                    int y = cisloAktualnihoPole[0] + legalniTahy[kodFigurky + "2"][0];
                    int x = cisloAktualnihoPole[1] + legalniTahy[kodFigurky + "2"][1];
                    int yT = cisloAktualnihoPole[0] + legalniTahy[kodFigurky + "2"][2];
                    int xT = cisloAktualnihoPole[1] + legalniTahy[kodFigurky + "2"][3];
                    if (hraciPole[y, x].Tag == null)
                    {
                        tahy.Add(cisloAktualnihoPole[0] + legalniTahy[kodFigurky][0]);
                        tahy.Add(cisloAktualnihoPole[1] + legalniTahy[kodFigurky][1]);
                    }
                    if (xT < 8 && xT >= 0 && yT < 8 && yT >= 0 && stavFigurek[y, x] == 0)
                    {
                        if (hraciPole[yT, xT].Tag == null)
                        {
                            tahy.Add(yT);
                            tahy.Add(xT);
                        }
                    }
                }

                else
                {
                    int y = cisloAktualnihoPole[0] + legalniTahy[kodFigurky + "2"][0];
                    int x = cisloAktualnihoPole[1] + legalniTahy[kodFigurky + "2"][1];
                    if (x < 8 && x >= 0 && y < 8 && y >= 0 && stavFigurek[y, x] == 0)
                    {
                        if (hraciPole[y, x].Tag == null)
                        {
                            tahy.Add(cisloAktualnihoPole[0] + legalniTahy[kodFigurky][0]);
                            tahy.Add(cisloAktualnihoPole[1] + legalniTahy[kodFigurky][1]);
                        }
                    }
                }
            }

            else if (kodFigurky == "n" || kodFigurky == "k")
            {
                for (int i = 0; i < legalniTahy[kodFigurky].Length; i += 2)
                {
                    int y = cisloAktualnihoPole[0] + legalniTahy[kodFigurky][i];
                    int x = cisloAktualnihoPole[1] + legalniTahy[kodFigurky][i + 1];
                    if (x < 8 && x >= 0 && y < 8 && y >= 0)
                    {
                        if (hraciPole[y, x].Tag != null)
                        {
                            if (hraciPole[y, x].Tag != null &&
                                ((Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && !Char.IsUpper(tempKodFigurky[0])) ||
                                (!Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && Char.IsUpper(tempKodFigurky[0]))))
                            {
                                tahy.Add(y);
                                tahy.Add(x);
                            }
                        }
                        else
                        {
                            tahy.Add(y);
                            tahy.Add(x);
                        }
                    }
                }
            }

            else
            {
                for (int i = 0; i < legalniTahy[kodFigurky].Length; i += 2)
                {
                    int y = cisloAktualnihoPole[0] + legalniTahy[kodFigurky][i];
                    int x = cisloAktualnihoPole[1] + legalniTahy[kodFigurky][i + 1];
                    if (x < 8 && x >= 0 && y < 8 && y >= 0)
                    {
                        if (stavFigurek[y, x] != 1)
                        {
                            tahy.Add(y);
                            tahy.Add(x);
                        }
                        else
                        {
                            if (hraciPole[y, x].Tag != null &&
                                ((Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && !Char.IsUpper(tempKodFigurky[0])) ||
                                (!Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && Char.IsUpper(tempKodFigurky[0]))))
                            {
                                tahy.Add(y);
                                tahy.Add(x);
                            }
                            int stranyPohybu = ((i / 14) + 1) * 14;
                            i = stranyPohybu - 2;
                        }
                    }
                }
            }
            return tahy;
        }

        public List<int> pesakCapture(int[] cisloAktualnihoPole, string kodFigurky, Button[,] hraciPole)
        {
            List<int> tahy = new List<int>();

            int y = cisloAktualnihoPole[0] + legalniTahy[kodFigurky + "3"][0];
            int x = cisloAktualnihoPole[1] + legalniTahy[kodFigurky + "3"][1];
            if (x < 8 && x >= 0 && y < 8 && y >= 0)
            {
                if (hraciPole[y, x].Tag != null)
                {
                    if ((Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && !Char.IsUpper(kodFigurky[0])) || (!Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && Char.IsUpper(kodFigurky[0])))
                    {
                        tahy.Add(y);
                        tahy.Add(x);
                    }
                }
            }

            y = cisloAktualnihoPole[0] + legalniTahy[kodFigurky + "3"][2];
            x = cisloAktualnihoPole[1] + legalniTahy[kodFigurky + "3"][3];
            if (x < 8 && x >= 0 && y < 8 && y >= 0)
            {
                if (hraciPole[y, x].Tag != null)
                {
                    if ((Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && !Char.IsUpper(kodFigurky[0])) || (!Char.IsUpper(hraciPole[y, x].Tag.ToString()[0]) && Char.IsUpper(kodFigurky[0])))
                    {
                        tahy.Add(y);
                        tahy.Add(x);
                    }
                }
            }

            return tahy;
        }

    }
}
