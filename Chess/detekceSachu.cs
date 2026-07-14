using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace šachys
{
    internal class detekceSachu
    {
        TahFigurka tahFigurka = new TahFigurka();
        realizacePohybu realizacePohybu = new realizacePohybu();
        int[] poziceKrale;

        public bool testTahuProtiSachuMatu(int[,] stavPole, Button[,] hraciPole, bool bilyNaRade, Button poziceTestovaneFigurky, int[] pozicePolozeniTestovaciFigurky)
        {
            if (poziceTestovaneFigurky.Tag == null)
            {
                return false;
            }

            string figurka = poziceTestovaneFigurky.Tag.ToString();
            if ((Char.IsUpper(figurka[0]) && bilyNaRade) || (!Char.IsUpper(figurka[0]) && !bilyNaRade))
            {
                // tah se docasne zahraje jen pres Tagy a stavPole a pak se vrati zpet
                // (zadne kopirovani celeho pole Buttonu)
                int[] odkud = realizacePohybu.hledacSouradnic(poziceTestovaneFigurky, hraciPole);
                int kamY = pozicePolozeniTestovaciFigurky[0];
                int kamX = pozicePolozeniTestovaciFigurky[1];

                object puvodniCilTag = hraciPole[kamY, kamX].Tag;
                int puvodniStavOdkud = stavPole[odkud[0], odkud[1]];
                int puvodniStavKam = stavPole[kamY, kamX];

                hraciPole[kamY, kamX].Tag = figurka;
                hraciPole[odkud[0], odkud[1]].Tag = null;
                stavPole[kamY, kamX] = 1;
                stavPole[odkud[0], odkud[1]] = 0;

                bool sach = jeKralSach(hraciPole, bilyNaRade, stavPole);

                hraciPole[odkud[0], odkud[1]].Tag = figurka;
                hraciPole[kamY, kamX].Tag = puvodniCilTag;
                stavPole[odkud[0], odkud[1]] = puvodniStavOdkud;
                stavPole[kamY, kamX] = puvodniStavKam;

                return !sach;
            }

            return false;
        }

        public bool testTahuProtiSachuMatuCastling(int[,] stavPole, Button[,] hraciPole, bool bilyNaRade, Button poziceTestovaneFigurky, int[] pozicePolozeniTestovaciFigurky, string[] RookMoves, bool KingBMove, bool KingWMove)
        {
            if (poziceTestovaneFigurky.Tag == null)
            {
                return false;
            }

            if ((Char.IsUpper(poziceTestovaneFigurky.Tag.ToString()[0]) && bilyNaRade) || (!Char.IsUpper(poziceTestovaneFigurky.Tag.ToString()[0]) && !bilyNaRade))
            {
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

                temphraciPole = realizacePohybu.castlingMoveHandle(realizacePohybu.hledacSouradnic(poziceTestovaneFigurky, hraciPole), pozicePolozeniTestovaciFigurky, poziceTestovaneFigurky.Tag.ToString(), hraciPole[pozicePolozeniTestovaciFigurky[0], pozicePolozeniTestovaciFigurky[1]].Tag.ToString(), temphraciPole, RookMoves, KingBMove, KingWMove);

                tempStavPole = updateStavHracihoPole(temphraciPole);

                if (!jeKralSach(temphraciPole, bilyNaRade, tempStavPole))
                {
                    return true;
                }
            }

            return false;
        }

        public bool testTahuProtiSachuMatuEnPeasant(int[,] stavPole, Button[,] hraciPole, bool bilyNaRade, Button poziceTestovaneFigurky, int[] pozicePolozeniTestovaciFigurky, int[] pole3)
        {
            if (poziceTestovaneFigurky.Tag == null)
            {
                return false;
            }

            if ((Char.IsUpper(poziceTestovaneFigurky.Tag.ToString()[0]) && bilyNaRade) || (!Char.IsUpper(poziceTestovaneFigurky.Tag.ToString()[0]) && !bilyNaRade))
            {
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

                int[] tempPole3 = (int[])pole3.Clone();

                int[,] tempStavPole = (int[,])stavPole.Clone();

                if (Char.IsUpper(poziceTestovaneFigurky.Tag.ToString()[0]))
                {
                    tempPole3[0] += 1;
                }
                else
                {
                    tempPole3[0] -= 1;
                }


                temphraciPole = realizacePohybu.enPeasantCapture(poziceTestovaneFigurky.Tag.ToString(), temphraciPole[tempPole3[0], tempPole3[1]].Tag?.ToString(), realizacePohybu.hledacSouradnic(poziceTestovaneFigurky, hraciPole), pole3, tempPole3, temphraciPole, bilyNaRade);

                tempStavPole = updateStavHracihoPole(temphraciPole);

                if (!jeKralSach(temphraciPole, bilyNaRade, tempStavPole))
                {
                    return true;
                }
            }

            return false;
        }

        public List<int> tahyProtiSachuMatu(int[,] stavPole, Button[,] hraciPole, bool bilyNaRade, Button vybranaFigurka)
        {
            List<int> tahyProtiSM = new List<int>();

            if (vybranaFigurka.Tag != null)
            {
                List<int> temp = tahFigurka.vypocetTahu(realizacePohybu.hledacSouradnic(vybranaFigurka, hraciPole), vybranaFigurka.Tag.ToString(), stavPole, hraciPole);
                for (int i = 0; i < temp.Count; i += 2)
                {
                    int[] temp2 = { temp[i], temp[i + 1] };
                    if (testTahuProtiSachuMatu(stavPole, hraciPole, bilyNaRade, vybranaFigurka, temp2))
                    {
                        tahyProtiSM.Add(temp[i]);
                        tahyProtiSM.Add(temp[i + 1]);
                    }
                }
            }
            return tahyProtiSM;
        }

        public List<int> hledaniPoziceFigurekSach(Button[,] hraciPole, int[,] stavPole, bool bilyNaRade)
        {
            poziceKrale = hledacPoziceKrale(hraciPole, bilyNaRade);
            List<int> poziceFigurekSachTemp = new List<int>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Button t = hraciPole[y, x];
                    if (t.Tag != null)
                    {
                        List<int> temp = tahFigurka.vypocetTahu(new int[] { y, x }, t.Tag.ToString(), stavPole, hraciPole);
                        for (int i = 0; i < temp.Count; i += 2)
                        {
                            if (temp[i] == poziceKrale[0] && temp[i + 1] == poziceKrale[1])
                            {
                                poziceFigurekSachTemp.Add(y);
                                poziceFigurekSachTemp.Add(x);
                            }
                        }
                    }
                }
            }
            return poziceFigurekSachTemp;
        }

        public int[] hledacPoziceKrale(Button[,] hraciPole, bool bilyNaRade)
        {
            string hledanyKral = bilyNaRade ? "K" : "k";
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    object tag = hraciPole[y, x].Tag;
                    if (tag != null && tag.ToString() == hledanyKral)
                    {
                        return new int[] { y, x };
                    }
                }
            }
            return null;
        }

        public bool jeKralSach(Button[,] hraciPole, bool bilyNaRade, int[,] stavPole)
        {
            poziceKrale = hledacPoziceKrale(hraciPole, bilyNaRade);
            if (poziceKrale == null) { return false; }
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Button t = hraciPole[y, x];
                    if (t.Tag == null) continue;

                    string figurka = t.Tag.ToString();
                    // sach muze dat jen souperova figurka
                    if (Char.IsUpper(figurka[0]) == bilyNaRade) continue;

                    List<int> temp = tahFigurka.vypocetTahu(new int[] { y, x }, figurka, stavPole, hraciPole);
                    for (int i = 0; i < temp.Count; i += 2)
                    {
                        if (temp[i] == poziceKrale[0] && temp[i + 1] == poziceKrale[1])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private int[,] updateStavHracihoPole(Button[,] hraciPole)
        {
            int[,] newStavHracihoPole = new int[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (hraciPole[y, x].BackgroundImage != null)
                    {
                        newStavHracihoPole[y, x] = 1;
                    }
                    else
                    {
                        newStavHracihoPole[y, x] = 0;
                    }
                }
            }
            return newStavHracihoPole;
        }
    }
}
