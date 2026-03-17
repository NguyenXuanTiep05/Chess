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

                temphraciPole = realizacePohybu.polozeniFigurek(poziceTestovaneFigurky.Tag.ToString(), realizacePohybu.hledacSouradnic(poziceTestovaneFigurky, hraciPole), pozicePolozeniTestovaciFigurky, temphraciPole);

                tempStavPole[pozicePolozeniTestovaciFigurky[0], pozicePolozeniTestovaciFigurky[1]] = 1;
                tempStavPole[realizacePohybu.hledacSouradnic(poziceTestovaneFigurky, hraciPole)[0], realizacePohybu.hledacSouradnic(poziceTestovaneFigurky, hraciPole)[1]] = 0;

                if (!jeKralSach(temphraciPole, bilyNaRade, tempStavPole))
                {
                    return true;
                }
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
            foreach (Button t in hraciPole)
            {
                if (t.Tag != null)
                {
                    List<int> temp = tahFigurka.vypocetTahu(realizacePohybu.hledacSouradnic(t, hraciPole), t.Tag.ToString(), stavPole, hraciPole);
                    for (int i = 0; i < temp.Count; i += 2)
                    {
                        if (temp[i] == poziceKrale[0] && temp[i + 1] == poziceKrale[1])
                        {
                            poziceFigurekSachTemp.Add(realizacePohybu.hledacSouradnic(t, hraciPole)[0]);
                            poziceFigurekSachTemp.Add(realizacePohybu.hledacSouradnic(t, hraciPole)[1]);
                        }
                    }
                }
            }
            return poziceFigurekSachTemp;
        }

        public int[] hledacPoziceKrale(Button[,] hraciPole, bool bilyNaRade)
        {
            foreach (Button t in hraciPole)
            {
                if (t.Tag != null)
                {
                    if (t.Tag.ToString() == "k" && !bilyNaRade)
                    {
                        return realizacePohybu.hledacSouradnic(t, hraciPole);
                    }
                    if (t.Tag.ToString() == "K" && bilyNaRade)
                    {
                        return realizacePohybu.hledacSouradnic(t, hraciPole);
                    }
                }
            }
            return null;
        }

        public bool jeKralSach(Button[,] hraciPole, bool bilyNaRade, int[,] stavPole)
        {
            poziceKrale = hledacPoziceKrale(hraciPole, bilyNaRade);
            foreach (Button t in hraciPole)
            {
                if (t.Tag != null)
                {
                    List<int> temp = tahFigurka.vypocetTahu(realizacePohybu.hledacSouradnic(t, hraciPole), t.Tag.ToString(), stavPole, hraciPole);
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
