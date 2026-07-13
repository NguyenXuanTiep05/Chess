using System.Collections.Generic;
using System.Windows.Forms;

namespace šachys
{
    internal class vypisTahu
    {
        detekceSachu detekceSachu = new detekceSachu();
        realizacePohybu realizacePohybu = new realizacePohybu();
        TahFigurka TahFigurka = new TahFigurka();
        Dictionary<int, string[]> spravneSyntax = new Dictionary<int, string[]>()
        {
            {0, ["a", "8"]},
            {1, ["b", "7"]},
            {2, ["c", "6"]},
            {3, ["d", "5"]},
            {4, ["e", "4"]},
            {5, ["f", "3"]},
            {6, ["g", "2"]},
            {7, ["h", "1"]},
        };
        public List<string> historieHracihoPolePriPohybu = new List<string>();
        List<int> tempPoziceStejnychFigurek = new List<int>();

        public string pridavaniTahuDoPole(string figurka, Button figurkaBtn, int[] souradnicePole, int[] minuleSouradnice, Button[,] hraciPole, bool bilyNaRade, int[,] stavPole, int[,] stavPolePo, bool castled, bool enPeasant, bool povyseni, Button povysenafig)
        {
            string tahNotace = "";
            string zacatek = figurka;
            string konec = "";
            if (figurka.ToLower() == "p")
            {
                if (!bylaFigurkaSebrana(stavPole, stavPolePo))
                {
                    zacatek = "";
                }
                if (povyseni)
                {
                    string H = bilyNaRade ? povysenafig.Tag.ToString() : povysenafig.Tag.ToString().ToLower();
                    konec += $"=>{H}";
                }
            }


            if (hledaniPoziceStejnychTahu(hraciPole, stavPole, bilyNaRade, figurkaBtn, souradnicePole) > 0)
            {
                for (int i = 0; i < tempPoziceStejnychFigurek.Count; i += 2)
                {
                    if (souradnicePole[0] != tempPoziceStejnychFigurek[i] && souradnicePole[1] != tempPoziceStejnychFigurek[i + 1])
                    {
                        zacatek += upravaSytaxeTahu(minuleSouradnice)[0] + upravaSytaxeTahu(minuleSouradnice)[1];
                        break;
                    }

                    if (souradnicePole[1] == tempPoziceStejnychFigurek[i + 1])
                    {
                        zacatek += upravaSytaxeTahu(minuleSouradnice)[0];
                        break;
                    }
                    if (souradnicePole[0] == tempPoziceStejnychFigurek[i])
                    {
                        zacatek += upravaSytaxeTahu(minuleSouradnice)[1];
                        break;
                    }

                }
            }
            tempPoziceStejnychFigurek.Clear();

            if (bylaFigurkaSebrana(stavPole, stavPolePo))
            {
                if (figurka.ToLower() == "p")
                {
                    zacatek = upravaSytaxeTahu(minuleSouradnice)[0];
                    if (enPeasant)
                    {
                        konec += " e.p. ";
                    }
                }
                zacatek += "x";
            }

            if (detekceSachu.jeKralSach(hraciPole, !bilyNaRade, stavPole))
            {
                konec = "+";
            }


            if (castled)
            {
                int[] stranaCasteling = detekceSachu.hledacPoziceKrale(hraciPole, bilyNaRade);
                if (stranaCasteling[1] == 2)
                {
                    tahNotace = "O-O-O" + konec;
                }
                else if (stranaCasteling[1] == 6)
                {
                    tahNotace = "O-O" + konec;
                }
            }

            else
            {
                tahNotace = zacatek + upravaSytaxeTahu(souradnicePole)[0] + upravaSytaxeTahu(souradnicePole)[1] + konec;
            }
            historieHracihoPolePriPohybu.Add(zmenaPoleNaString(hraciPole));
            return tahNotace;
        }

        public string vypisPole(int index)
        {
            return historieHracihoPolePriPohybu[index];
        }

        public string zmenaPoleNaString(Button[,] hraciPole)
        {
            string poleNaString = "";
            foreach (Button button in hraciPole)
            {
                if (button.Tag != null)
                {
                    poleNaString += button.Tag.ToString();
                    poleNaString += realizacePohybu.hledacSouradnic(button, hraciPole)[0];
                    poleNaString += realizacePohybu.hledacSouradnic(button, hraciPole)[1];
                }
            }
            return poleNaString;
        }


        private int hledaniPoziceStejnychTahu(Button[,] hraciPole, int[,] stavPole, bool bilyNaRade, Button vybranaFigurka, int[] testovanePole)
        {
            int pocetStejnychFigurekTahy = 0;
            if (vybranaFigurka.Tag != null)
            {
                foreach (Button t in hraciPole)
                {
                    if (t.Tag != null && t.Tag.ToString() == vybranaFigurka.Tag.ToString())
                    {
                        int[] tempPoziceBtn = realizacePohybu.hledacSouradnic(t, hraciPole);
                        List<int> temp = TahFigurka.vypocetTahu(tempPoziceBtn, t.Tag.ToString(), stavPole, hraciPole);
                        for (int i = 0; i < temp.Count; i += 2)
                        {
                            if (temp[i] == testovanePole[0] && temp[i + 1] == testovanePole[1])
                            {
                                pocetStejnychFigurekTahy += 1;
                                tempPoziceStejnychFigurek.Add(tempPoziceBtn[0]);
                                tempPoziceStejnychFigurek.Add(tempPoziceBtn[1]);
                                break;
                            }
                        }
                    }
                }
            }

            return pocetStejnychFigurekTahy;
        }



        private string[] upravaSytaxeTahu(int[] souradnicePole)
        {
            string[] upravenySyntaxTahu = new string[2];

            upravenySyntaxTahu[0] = spravneSyntax[souradnicePole[1]][0];
            upravenySyntaxTahu[1] = spravneSyntax[souradnicePole[0]][1];

            return upravenySyntaxTahu;
        }

        public void vymazPriKonci()
        {
            historieHracihoPolePriPohybu.Clear();
        }

        public List<string> vratitListPozici()
        {
            return historieHracihoPolePriPohybu;
        }

        public void sebratListPozic(List<string> temp)
        {
            historieHracihoPolePriPohybu = temp;
        }

        public bool bylaFigurkaSebrana(int[,] stavHracihoPole, int[,] stavPolePredTahem)
        {
            if (stavHracihoPole == null || stavPolePredTahem == null)
            {
                return false;
            }

            int pocetfigurekpred = 0;
            int pocetfigurekpo = 0;

            foreach (int i in stavHracihoPole)
            {
                if (i == 1)
                {
                    pocetfigurekpred++;
                }
            }
            foreach (int i in stavPolePredTahem)
            {
                if (i == 1)
                {
                    pocetfigurekpo++;
                }
            }

            return pocetfigurekpred != pocetfigurekpo;
        }
    }
}
