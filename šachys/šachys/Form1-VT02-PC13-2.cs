using šachys.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace šachys
{
    public partial class Form1 : Form
    {
        int poleSize = 8;
        public Button[,] hraciPole = new Button[8, 8];
        int velikostPolicka = 70;
        public int[,] stavHracihoPole = {
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
        };
        bool hratProtiAI = true;
        List<string> cernyCapturedString = new List<string>();
        List<string> bilyCapturedString = new List<string>();

        ChessKomp ChessKomp = new ChessKomp();
        TridaFigurky figurka = new TridaFigurky();
        TahFigurka TahFigurka = new TahFigurka();
        realizacePohybu realizacePohybu = new realizacePohybu();
        detekceSachu detekceSachu = new detekceSachu();
        Povyseni povyseni;
        SachmatRemiza sachmatRemiza = new SachmatRemiza();

        ListBox poleTahy = new ListBox();
        Button OnOffAI = new Button();
        Button Nacist = new Button();
        Button Ulozit = new Button();
        DataGridView cernySebran = new DataGridView();
        DataGridView bilySebran = new DataGridView();
        vypisTahu vypisTahu = new vypisTahu();
        Color barvaLicha = Color.FromArgb(95, 129, 62);
        Color barvaSuda = Color.FromArgb(235, 236, 208);
        string PostaveniFigurek = "r00r07n01n06b02b05q03k04p10p11p12p13p14p15p16p17P60P61P62P63P64P65P66P67R70R77N71N76B72B75Q73K74";



        public Form1()
        {
            povyseni = new Povyseni(this);
            povyseni.Hide();
            InitializeComponent();
            kresleniHracihoPole();
            kresleniFigurek(PostaveniFigurek, hraciPole);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.Black;
            //pokud chcete videt hrat dve "AI" mezi sebou
            testAI(false);
        }

        private async void testAI(bool spustit)
        {
            if (spustit)
            {
                Array.Clear(EnPeasant, 0, 2);
                var tempStavHracihoPole = (int[,])stavHracihoPole.Clone();
                hraciPole = ChessKomp.AI(hraciPole, stavHracihoPole, bilyNaRade);
                stavHracihoPole = updateStavHracihoPole(hraciPole);
                castlingDetect(ChessKomp.vratitFigurku(), ChessKomp.vratitAktualniPole());

                Button tempBtn = new Button()
                {
                    Tag = "Q"
                };
                poleTahy.Items.Add(vypisTahu.pridavaniTahuDoPole(ChessKomp.vratitFigurku(), ChessKomp.vratitBtn(), ChessKomp.vratitAktualniPole(), ChessKomp.vratitMinulePole(), hraciPole, bilyNaRade, tempStavHracihoPole, stavHracihoPole, false, false, ChessKomp.vratitPovysenibool(), tempBtn));
                poleTahy.TopIndex = poleTahy.Items.Count - 1;
                bilyNaRade = !bilyNaRade;

                if (vypisTahu.bylaFigurkaSebrana(stavHracihoPole, tempStavHracihoPole))
                {
                    if (bilyNaRade)
                    {
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        imageColumn.Width = 100;
                        imageColumn.Image = figurka.rozpoznavaniFigurek(ChessKomp.vratitSebranaFigurka());
                        cernySebran.Columns.Add(imageColumn);
                        cernyCapturedString.Add(ChessKomp.vratitSebranaFigurka());
                    }
                    else
                    {
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        imageColumn.Width = 100;
                        imageColumn.Image = figurka.rozpoznavaniFigurek(ChessKomp.vratitSebranaFigurka());
                        bilySebran.Columns.Add(imageColumn);
                        bilyCapturedString.Add(ChessKomp.vratitSebranaFigurka());
                    }
                }

                if (sachmatRemiza.maOponentPlatnePohyby(stavHracihoPole, hraciPole, bilyNaRade) && detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole))
                {
                    string vyherce = !bilyNaRade ? "BÍLÝ" : "ČERNÝ";
                    poleTahy.Items.Add(!bilyNaRade ? "1–0" : "0–1");
                    poleTahy.TopIndex = poleTahy.Items.Count - 1;
                    MessageBox.Show($"Šachmat, {vyherce} vyhrál!!!!!!! [*_*]");
                    novaHra();
                }
                else if (sachmatRemiza.nedostatekMaterialu(hraciPole))
                {
                    poleTahy.Items.Add("½–½");
                    poleTahy.TopIndex = poleTahy.Items.Count - 1;
                    //MessageBox.Show("No nic remíza | Nedostatek materiálu");
                    await Task.Delay(250);
                    novaHra();
                    await Task.Delay(250);
                }
                else if (sachmatRemiza.maOponentPlatnePohyby(stavHracihoPole, hraciPole, bilyNaRade) && !detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole))
                {
                    poleTahy.Items.Add("½–½");
                    poleTahy.TopIndex = poleTahy.Items.Count - 1;
                    //MessageBox.Show("No nic remíza");
                    await Task.Delay(250);
                    novaHra();
                    await Task.Delay(250);
                }
                await Task.Delay(50);
                testAI(spustit);
            }
        }

        private void kresleniHracihoPole()
        {
            cernySebran.Location = new Point(15, 15);
            cernySebran.Size = new Size(velikostPolicka * 8, Convert.ToInt16(velikostPolicka * 75 / 100));
            cernySebran.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            cernySebran.RowTemplate.Height = Convert.ToInt16(velikostPolicka * 75 / 100);
            cernySebran.ColumnHeadersVisible = false;
            cernySebran.RowHeadersVisible = false;
            cernySebran.AllowUserToAddRows = false;
            cernySebran.AllowUserToDeleteRows = false;
            cernySebran.RowCount = 1;
            cernySebran.ReadOnly = true;
            cernySebran.BackgroundColor = Color.Black;
            cernySebran.GridColor = this.BackColor;
            cernySebran.ScrollBars = ScrollBars.None;

            bilySebran.Location = new Point(15, 30 + cernySebran.Height + (poleSize * velikostPolicka) + 15);
            bilySebran.Size = new Size(velikostPolicka * 8, Convert.ToInt16(velikostPolicka * 75 / 100));
            bilySebran.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            bilySebran.RowTemplate.Height = Convert.ToInt16(velikostPolicka * 75 / 100);
            bilySebran.ColumnHeadersVisible = false;
            bilySebran.RowHeadersVisible = false;
            bilySebran.AllowUserToAddRows = false;
            bilySebran.AllowUserToDeleteRows = false;
            bilySebran.RowCount = 1;
            bilySebran.ReadOnly = true;
            bilySebran.BackgroundColor = Color.Black;
            bilySebran.GridColor = this.BackColor;
            bilySebran.ScrollBars = ScrollBars.None;


            for (int y = 0; y < poleSize; y++)
            {
                for (int x = 0; x < poleSize; x++)
                {
                    Button button = new Button
                    {
                        Size = new Size(velikostPolicka, velikostPolicka),
                        Text = null,
                        BackgroundImageLayout = ImageLayout.Zoom
                    };
                    if (x == 0)
                    {
                        button.Location = new Point(15, y * velikostPolicka + 30 + cernySebran.Height);
                    }
                    else
                    {
                        button.Location = new Point(x * velikostPolicka + 15, y * velikostPolicka + 30 + cernySebran.Height);
                    }
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    if ((y + x) % 2 == 0)
                    {
                        button.BackColor = barvaSuda;
                        button.FlatAppearance.MouseOverBackColor = barvaSuda;
                        button.FlatAppearance.MouseDownBackColor = barvaSuda;
                    }

                    else
                    {
                        button.BackColor = barvaLicha;
                        button.FlatAppearance.MouseOverBackColor = barvaLicha;
                        button.FlatAppearance.MouseDownBackColor = barvaLicha;
                    }
                    button.Click += pole_Click;
                    this.Controls.Add(button);
                    hraciPole[y, x] = button;
                }
            }
            poleTahy.Location = new Point(poleSize * velikostPolicka + 35, 30 + cernySebran.Height);
            poleTahy.Size = new Size(150, poleSize * velikostPolicka + 15);
            poleTahy.Font = new Font(poleTahy.Font.FontFamily, 15,FontStyle.Bold);
            poleTahy.SelectedIndexChanged += poleTahyIndex;
            poleTahy.ForeColor = Color.White;
            poleTahy.BackColor = Color.DarkSlateGray;


            Nacist.Text = "Load";
            Nacist.BackColor = Color.White;
            Nacist.ForeColor = Color.Black;
            Nacist.FlatAppearance.BorderSize = 0;
            Nacist.FlatStyle = FlatStyle.Flat;
            Nacist.Font = new Font(poleTahy.Font.FontFamily, 13, FontStyle.Bold);
            Nacist.TextAlign = ContentAlignment.MiddleCenter;
            Nacist.Click += LoadXml;
            Nacist.Location = new Point(poleTahy.Location.X, poleTahy.Location.Y + 15 + (poleSize * velikostPolicka));
            Nacist.Size = new Size(Convert.ToInt32(poleTahy.Width * 0.5) - 5, cernySebran.Height);

            Ulozit.Text = "Save";
            Ulozit.BackColor = Color.White;
            Ulozit.ForeColor = Color.Black;
            Ulozit.FlatAppearance.BorderSize = 0;
            Ulozit.FlatStyle = FlatStyle.Flat;
            Ulozit.Font = new Font(poleTahy.Font.FontFamily, 13, FontStyle.Bold);
            Ulozit.TextAlign = ContentAlignment.MiddleCenter;
            Ulozit.Click += SaveXml;
            Ulozit.Location = new Point(Nacist.Location.X + Nacist.Width + 10, Nacist.Location.Y);
            Ulozit.Size = Nacist.Size;

            OnOffAI.Text = "On/Off AI";
            OnOffAI.BackColor = Color.Green;
            OnOffAI.ForeColor = Color.Black;
            OnOffAI.FlatAppearance.BorderSize = 0;
            OnOffAI.FlatStyle = FlatStyle.Flat;
            OnOffAI.Font = new Font(poleTahy.Font.FontFamily, 13, FontStyle.Bold);
            OnOffAI.TextAlign = ContentAlignment.MiddleCenter;
            OnOffAI.Click += OnOff;
            OnOffAI.Location = new Point(poleTahy.Location.X, poleTahy.Location.Y - Nacist.Height - 15);
            OnOffAI.Size = new Size(poleTahy.Width ,Nacist.Height);




            this.Controls.Add(OnOffAI);
            this.Controls.Add(Ulozit);
            this.Controls.Add(Nacist);
            this.Controls.Add(poleTahy);
            this.Controls.Add(cernySebran);
            this.Controls.Add(bilySebran);

            this.Size = new Size((poleSize * velikostPolicka) + poleTahy.Width + 70, poleSize * velikostPolicka + cernySebran.Height + bilySebran.Height + 95);
        }

        public void kresleniFigurek(string PostaveniFigurek, Button[,] tempHraciPole)
        {
            foreach (Button v in tempHraciPole)
            {
                v.BackgroundImage = null;
                v.Tag = null;
            }

            for (int y = 0; y < stavHracihoPole.GetLength(0); y++)
            {
                for (int x = 0; x < stavHracihoPole.GetLength(1); x++)
                {
                    stavHracihoPole[y, x] = 0;
                }
            }

            for (int i = 0; i < PostaveniFigurek.Length; i += 3)
            {
                char yT = PostaveniFigurek[i + 1];
                char xT = PostaveniFigurek[i + 2];
                int y = yT - '0';
                int x = xT - '0';
                char pismeno = PostaveniFigurek[i];
                hraciPole[y, x].BackgroundImage = figurka.rozpoznavaniFigurek(pismeno.ToString());
                hraciPole[y, x].Tag = pismeno.ToString();
                stavHracihoPole[y, x] = 1;
            }
        }


        public int[] souradniceMinulehoPolicka;
        public int[] souradniceAktualnihoPolicka;
        Button vybranaFigurkaBtn;
        string vybranaFigurka;

        public bool bilyNaRade = true;
        private int[] EnPeasant = new int[2]{
            0, 0
        };
        bool castled = false;
        bool enpeasantChecker = false;
        bool promoted = false;
        bool KingBMove = false;
        bool KingWMove = false;
        string[] RookMoves = new string[4];

        private void OnOff(object sender, EventArgs e)
        {
            hratProtiAI = !hratProtiAI;
            OnOffAI.BackColor = hratProtiAI ? Color.Green : Color.Red;
        }

        private void poleTahyIndex(object sender, EventArgs e)
        {
            vybranaFigurka = null;
            resetovaniBarevPolicek();
            int selectedIndex = poleTahy.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < vypisTahu.historieHracihoPolePriPohybu.Count - 1)
            {
                kresleniFigurek(vypisTahu.vypisPole(selectedIndex), hraciPole);
                foreach (Button v in hraciPole)
                {
                    v.Enabled = false;
                }
            }

            if (selectedIndex == vypisTahu.historieHracihoPolePriPohybu.Count - 1)
            {
                kresleniFigurek(vypisTahu.vypisPole(selectedIndex), hraciPole);
                foreach (Button v in hraciPole)
                {
                    v.Enabled = true;
                }
            }

            poleTahy.SelectedIndex = -1;
        }

        private void pole_Click(object sender, EventArgs e)
        {
            if ((sender as Button).BackgroundImage != null && (sender as Button).Tag != null && vybranaFigurka == null)
            {
                souradniceMinulehoPolicka = realizacePohybu.hledacSouradnic(sender as Button, hraciPole);
                vybranaFigurka = null;
                string figurkaTag = (sender as Button).Tag.ToString();
                if ((bilyNaRade == true && figurkaTag == figurkaTag.ToUpper()) || (bilyNaRade == false && figurkaTag == figurkaTag.ToLower()))
                {
                    vybranaFigurka = (sender as Button).Tag.ToString();
                    vybranaFigurkaBtn = (sender as Button);
                }
                if (vybranaFigurka != null)
                {
                    mozneTahy(sender as Button);
                    if (EnPeasant[0] != 0 && EnPeasant[1] != 0 && spravnaFigurkaNaEnPeasant())
                    {
                        if (detekceSachu.testTahuProtiSachuMatuEnPeasant(stavHracihoPole, hraciPole, bilyNaRade, vybranaFigurkaBtn, souradniceAktualnihoPolicka, (int[])EnPeasant.Clone()))
                        {
                            hraciPole[EnPeasant[0], EnPeasant[1]].BackColor = Color.Red;
                        }
                    }
                }
            }
            else if (vybranaFigurka != null)
            {
                var tempStavHracihoPole = (int[,])stavHracihoPole.Clone();
                souradniceAktualnihoPolicka = realizacePohybu.hledacSouradnic(sender as Button, hraciPole);

                if (souradniceMinulehoPolicka[0] != souradniceAktualnihoPolicka[0] || souradniceMinulehoPolicka[1] != souradniceAktualnihoPolicka[1])
                {
                    string sebranaFigurka = "";
                    if (hraciPole[souradniceAktualnihoPolicka[0], souradniceAktualnihoPolicka[1]].Tag != null)
                    {
                        sebranaFigurka = hraciPole[souradniceAktualnihoPolicka[0], souradniceAktualnihoPolicka[1]].Tag.ToString();
                    }
                    if (EnPeasant[0] != 0 && EnPeasant[1] != 0 && spravnaFigurkaNaEnPeasant())
                    {
                        sebranaFigurka = bilyNaRade ? "p" : "P";
                    }

                    if (vybranaFigurka.ToLower() == "p")
                    {
                        pesakSpecialPohyb(sender as Button);
                    }
                    if ((sender as Button).Tag != null && !detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole) && castelingSpravneFigurky(sender as Button))
                    {
                        if (castelingTrue(sender as Button) && detekceSachu.testTahuProtiSachuMatuCastling(stavHracihoPole, hraciPole, bilyNaRade, vybranaFigurkaBtn, souradniceAktualnihoPolicka, RookMoves, KingBMove, KingWMove))
                        {
                            hraciPole = realizacePohybu.castlingMoveHandle(souradniceMinulehoPolicka, souradniceAktualnihoPolicka, vybranaFigurka, (sender as Button).Tag.ToString(), hraciPole, RookMoves, KingBMove, KingWMove);
                            castled = realizacePohybu.vratitCastled();
                            Array.Clear(EnPeasant, 0, 2);
                        }
                    }
                    else
                    {
                        platneTahyHandle(vybranaFigurkaBtn);
                    }
                    stavHracihoPole = updateStavHracihoPole(hraciPole);

                    if (JeZmenavHracimPoli(tempStavHracihoPole, stavHracihoPole))
                    {
                        poleTahy.Items.Add(vypisTahu.pridavaniTahuDoPole(vybranaFigurka, hraciPole[souradniceAktualnihoPolicka[0], souradniceAktualnihoPolicka[1]], souradniceAktualnihoPolicka, souradniceMinulehoPolicka, hraciPole, bilyNaRade, tempStavHracihoPole, stavHracihoPole, castled, enpeasantChecker, promoted, hraciPole[souradniceAktualnihoPolicka[0], souradniceAktualnihoPolicka[1]]));
                        poleTahy.TopIndex = poleTahy.Items.Count - 1;
                        bilyNaRade = !bilyNaRade;
                    }
                    if (sachmatRemiza.maOponentPlatnePohyby(stavHracihoPole, hraciPole, bilyNaRade) && detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole))
                    {
                        string vyherce = !bilyNaRade ? "BÍLÝ" : "ČERNÝ";
                        poleTahy.Items.Add(!bilyNaRade ? "1–0" : "0–1");
                        poleTahy.TopIndex = poleTahy.Items.Count - 1;
                        MessageBox.Show($"Šachmat, {vyherce} vyhrál!!!!!!! [*_*]");
                        novaHra();
                        return;
                    }
                    else if (sachmatRemiza.nedostatekMaterialu(hraciPole))
                    {
                        poleTahy.Items.Add("½–½");
                        poleTahy.TopIndex = poleTahy.Items.Count - 1;
                        MessageBox.Show("No nic remíza | Nedostatek materiálu");
                        novaHra();
                        return;
                    }
                    else if (sachmatRemiza.maOponentPlatnePohyby(stavHracihoPole, hraciPole, bilyNaRade) && !detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole))
                    {
                        poleTahy.Items.Add("½–½");
                        poleTahy.TopIndex = poleTahy.Items.Count - 1;
                        MessageBox.Show("No nic remíza");
                        novaHra();
                        return;
                    }
                    else if (vypisTahu.bylaFigurkaSebrana(stavHracihoPole, tempStavHracihoPole))
                    {
                        if (bilyNaRade)
                        {
                            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                            imageColumn.Width = 100;
                            imageColumn.Image = figurka.rozpoznavaniFigurek(sebranaFigurka);
                            cernySebran.Columns.Add(imageColumn);
                            cernyCapturedString.Add(sebranaFigurka);
                        }
                        else
                        {
                            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                            imageColumn.Width = 100;
                            imageColumn.Image = figurka.rozpoznavaniFigurek(sebranaFigurka);
                            bilySebran.Columns.Add(imageColumn);
                            bilyCapturedString.Add(sebranaFigurka);
                        }
                    }

                    if (JeZmenavHracimPoli(tempStavHracihoPole, stavHracihoPole) && hratProtiAI)
                    {
                        Array.Clear(EnPeasant, 0, 2);
                        tempStavHracihoPole = (int[,])stavHracihoPole.Clone();
                        hraciPole = ChessKomp.AI(hraciPole, stavHracihoPole, bilyNaRade);
                        stavHracihoPole = updateStavHracihoPole(hraciPole);
                        castlingDetect(ChessKomp.vratitFigurku(), ChessKomp.vratitAktualniPole());

                        Button tempBtn = new Button() {
                            Tag = "Q"
                        };
                        poleTahy.Items.Add(vypisTahu.pridavaniTahuDoPole(ChessKomp.vratitFigurku(), ChessKomp.vratitBtn(), ChessKomp.vratitAktualniPole(), ChessKomp.vratitMinulePole(), hraciPole, bilyNaRade, tempStavHracihoPole, stavHracihoPole, false, false, ChessKomp.vratitPovysenibool(), tempBtn));
                        poleTahy.TopIndex = poleTahy.Items.Count - 1;
                        EnPeasant = ChessKomp.vratitEnpeasant();

                        bilyNaRade = !bilyNaRade;

                        if (vypisTahu.bylaFigurkaSebrana(stavHracihoPole, tempStavHracihoPole))
                        {
                            if (bilyNaRade)
                            {
                                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                                imageColumn.Width = 100;
                                imageColumn.Image = figurka.rozpoznavaniFigurek(ChessKomp.vratitSebranaFigurka());
                                cernySebran.Columns.Add(imageColumn);
                                cernyCapturedString.Add(ChessKomp.vratitSebranaFigurka());
                            }
                            else
                            {
                                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                                imageColumn.Width = 100;
                                imageColumn.Image = figurka.rozpoznavaniFigurek(ChessKomp.vratitSebranaFigurka());
                                bilySebran.Columns.Add(imageColumn);
                                bilyCapturedString.Add(ChessKomp.vratitSebranaFigurka());
                            }
                        }

                        if (sachmatRemiza.maOponentPlatnePohyby(stavHracihoPole, hraciPole, bilyNaRade) && detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole))
                        {
                            string vyherce = !bilyNaRade ? "BÍLÝ" : "ČERNÝ";
                            poleTahy.Items.Add(!bilyNaRade ? "1–0" : "0–1");
                            poleTahy.TopIndex = poleTahy.Items.Count - 1;
                            MessageBox.Show($"Šachmat, {vyherce} vyhrál!!!!!!! [*_*]");
                            novaHra();
                            return;
                        }
                        else if (sachmatRemiza.nedostatekMaterialu(hraciPole))
                        {
                            poleTahy.Items.Add("½–½");
                            poleTahy.TopIndex = poleTahy.Items.Count - 1;
                            MessageBox.Show("No nic remíza | Nedostatek materiálu");
                            novaHra();
                            return;
                        }
                        else if (sachmatRemiza.maOponentPlatnePohyby(stavHracihoPole, hraciPole, bilyNaRade) && !detekceSachu.jeKralSach(hraciPole, bilyNaRade, stavHracihoPole))
                        {
                            poleTahy.Items.Add("½–½");
                            poleTahy.TopIndex = poleTahy.Items.Count - 1;
                            MessageBox.Show("No nic remíza");
                            novaHra();
                            return;
                        }
                    }
                }
                enpeasantChecker = false;
                castled = false;
                vybranaFigurka = null;
                promoted = false;
                resetovaniBarevPolicek();
            }
        }




        private void novaHra()
        {
            cernySebran.Columns.Clear();
            bilySebran.Columns.Clear();
            poleTahy.Items.Clear();
            vypisTahu.vymazPriKonci();
            cernyCapturedString.Clear();
            bilyCapturedString.Clear();
            KingBMove = false;
            KingWMove = false;
            Array.Clear(EnPeasant, 0, 2);
            Array.Clear(RookMoves, 0, 4);
            kresleniFigurek(PostaveniFigurek, hraciPole);
            bilyNaRade = true;
            resetovaniBarevPolicek();
            enpeasantChecker = false;
            castled = false;
            vybranaFigurka = null;
            promoted = false;
            resetCaptueGrid();
        }

        private void resetCaptueGrid()
        {
            cernySebran.Columns.Clear();
            cernySebran.RowCount = 1;
            cernySebran.BackgroundColor = Color.Black;
            cernySebran.GridColor = this.BackColor;
            cernySebran.ScrollBars = ScrollBars.None;

            bilySebran.Columns.Clear();
            bilySebran.RowCount = 1;
            bilySebran.BackgroundColor = Color.Black;
            bilySebran.GridColor = this.BackColor;
            bilySebran.ScrollBars = ScrollBars.None;
        }





        private void platneTahyHandle(Button vybranaFigurkaButton)
        {

            if (spravnaFigurkaPovyseniCapture())
            {
                povyseni.ShowDialog();
                Array.Clear(EnPeasant, 0, 2);
                promoted = true;
            }
            else
            {
                List<int> legalniTahy = detekceSachu.tahyProtiSachuMatu(stavHracihoPole, hraciPole, bilyNaRade, vybranaFigurkaButton);

                for (int x = 0; x < legalniTahy.Count; x += 2)
                {
                    if (souradniceAktualnihoPolicka[0] == legalniTahy[x] && souradniceAktualnihoPolicka[1] == legalniTahy[x + 1])
                    {
                        Array.Clear(EnPeasant, 0, 2);
                        hraciPole = realizacePohybu.polozeniFigurek(vybranaFigurka, souradniceMinulehoPolicka, souradniceAktualnihoPolicka, hraciPole);
                        castlingDetect(vybranaFigurka, souradniceAktualnihoPolicka);
                        enPeasantDetekce(souradniceAktualnihoPolicka, souradniceMinulehoPolicka);
                        break;
                    }
                }
            }
        }

        bool spravnaFigurkaNaEnPeasant()
        {
            return (souradniceMinulehoPolicka[0] == EnPeasant[0] - 1 && (souradniceMinulehoPolicka[1] == EnPeasant[1] - 1 || souradniceMinulehoPolicka[1] == EnPeasant[1] + 1) && vybranaFigurka == "p") ||
                   (souradniceMinulehoPolicka[0] == EnPeasant[0] + 1 && (souradniceMinulehoPolicka[1] == EnPeasant[1] - 1 || souradniceMinulehoPolicka[1] == EnPeasant[1] + 1) && vybranaFigurka == "P");
        }


        bool spravnaFigurkaNaPovyseni()
        {
            return (vybranaFigurka == "P" && souradniceAktualnihoPolicka[0] == 0 && souradniceMinulehoPolicka[0] == souradniceAktualnihoPolicka[0] + 1) ||
                    (vybranaFigurka == "p" && souradniceAktualnihoPolicka[0] == 7 && souradniceMinulehoPolicka[0] == souradniceAktualnihoPolicka[0] - 1);
        }
        bool spravnaFigurkaPovyseniCapture()
        {
            return spravnaFigurkaNaPovyseni() && souradniceAktualnihoPolicka[1] != souradniceMinulehoPolicka[1]
                && hraciPole[souradniceAktualnihoPolicka[0], souradniceAktualnihoPolicka[1]].Tag != null &&
                (souradniceAktualnihoPolicka[1] == souradniceMinulehoPolicka[1] - 1 || souradniceAktualnihoPolicka[1] == souradniceMinulehoPolicka[1] + 1);
        }

        private void pesakSpecialPohyb(Button button)
        {
            if (spravnaFigurkaNaPovyseni() && button.BackgroundImage == null && souradniceAktualnihoPolicka[1] == souradniceMinulehoPolicka[1])
            {
                povyseni.ShowDialog();
                promoted = true;
                Array.Clear(EnPeasant, 0, 2);
            }
            if (souradniceAktualnihoPolicka[0] == EnPeasant[0] && souradniceAktualnihoPolicka[1] == EnPeasant[1])
            {
                if (spravnaFigurkaNaEnPeasant() && detekceSachu.testTahuProtiSachuMatuEnPeasant(stavHracihoPole, hraciPole, bilyNaRade, vybranaFigurkaBtn, souradniceAktualnihoPolicka, (int[])EnPeasant.Clone()))
                {
                    if (Char.IsUpper(vybranaFigurka[0]))
                    {
                        EnPeasant[0] += 1;
                    }
                    else
                    {
                        EnPeasant[0] -= 1;
                    }
                    hraciPole = realizacePohybu.enPeasantCapture(vybranaFigurka, hraciPole[EnPeasant[0], EnPeasant[1]].Tag.ToString(), souradniceMinulehoPolicka, souradniceAktualnihoPolicka, EnPeasant, hraciPole, bilyNaRade);
                    enpeasantChecker = true;
                    Array.Clear(EnPeasant, 0, 2);
                }
            }
        }



        private void enPeasantDetekce(int[] aktualni, int[] minuly)
        {
            if (vybranaFigurka.ToLower() == "p" && Math.Abs(aktualni[0] - minuly[0]) == 2)
            {
                Array.Clear(EnPeasant, 0, 2);
                EnPeasant[0] = aktualni[0];
                EnPeasant[1] = aktualni[1];
                if (Char.IsUpper(vybranaFigurka[0]))
                {
                    EnPeasant[0] += 1;
                }
                else
                {
                    EnPeasant[0] -= 1;
                }
            }
        }

        private void castlingDetect(string kodfigurka, int[] pole)
        {
            if (kodfigurka.ToLower() == "k" || kodfigurka.ToLower() == "r")
            {
                if (kodfigurka.ToLower() == "k")
                {
                    if (!Char.IsUpper(kodfigurka[0]))
                    {
                        KingBMove = true;
                    }
                    else
                    {
                        KingWMove = true;
                    }
                }
                else
                {
                    if (!Char.IsUpper(kodfigurka[0]))
                    {
                        if (pole[0] == 0 && pole[1] == 0)
                        {
                            RookMoves[0] = "N";
                        }
                        else
                        {
                            RookMoves[1] = "N";
                        }
                    }
                    else
                    {
                        if (pole[0] == 7 && pole[1] == 0)
                        {
                            RookMoves[2] = "N";
                        }
                        else
                        {
                            RookMoves[3] = "N";
                        }
                    }
                }
            }
        }


        private bool castelingSpravneFigurky(Button button)
        {
            return (vybranaFigurka == "k" && button.Tag.ToString() == "r") || (vybranaFigurka == "K" && button.Tag.ToString() == "R") ||
                    (vybranaFigurka == "r" && button.Tag.ToString() == "k") || (vybranaFigurka == "R" && button.Tag.ToString() == "k");
        }

        private bool castelingTrue(Button button)
        {
            return ((vybranaFigurka.ToLower() == "k" && button.Tag.ToString().ToLower() == "r") || (vybranaFigurka.ToLower() == "r" && button.Tag.ToString().ToLower() == "k"));
        }

        private bool JeZmenavHracimPoli(int[,] stavHracihoPole, int[,] tempStavHracihoPole)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (stavHracihoPole[y, x] != tempStavHracihoPole[y, x])
                    {
                        return true;
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

        private void mozneTahy(Button vybranaFigurkaButton)
        {
            List<int> temp = detekceSachu.tahyProtiSachuMatu(stavHracihoPole, hraciPole, bilyNaRade, vybranaFigurkaButton);
            for (int i = 0; i < temp.Count; i += 2)
            {
                if (temp[i] < 8 && temp[i] >= 0 && temp[i + 1] < 8 && temp[i + 1] >= 0)
                {
                    hraciPole[temp[i], temp[i + 1]].BackColor = Color.Red;
                }
            }
        }

        private void resetovaniBarevPolicek()
        {
            foreach (var v in hraciPole)
            {
                if (v.BackColor == Color.Red || v.BackColor == Color.Blue)
                {
                    int[] temp = realizacePohybu.hledacSouradnic(v, hraciPole);
                    v.BackColor = ((temp[0] + temp[1]) % 2 == 0) ? barvaSuda : barvaLicha;
                }
            }
        }


        private void LoadXml(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files|*.xml",
                Title = "Načti save"

            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XDocument xmlDocument = XDocument.Load(openFileDialog.FileName);

                    novaHra();
                    kresleniFigurek(xmlDocument.Root.Name.LocalName, hraciPole);
                    var pozice = xmlDocument.Descendants("Pozice").Elements("Item").Select(item => item.Value).ToList();
                    vypisTahu.sebratListPozic(pozice);

                    var tahy = xmlDocument.Descendants("Tahy").Elements("Item").Select(item => item.Value).ToList();
                    foreach (var v in tahy)
                    {
                        poleTahy.Items.Add(v);
                    }

                    var bilyNaRade1 = xmlDocument.Descendants("bilyNaRade").FirstOrDefault()?.Value;
                    var kingBMove1 = xmlDocument.Descendants("KingBMove").FirstOrDefault()?.Value;
                    var kingWMove1 = xmlDocument.Descendants("KingWMove").FirstOrDefault()?.Value;
                    bilyNaRade = Convert.ToBoolean(bilyNaRade1);
                    KingBMove = Convert.ToBoolean(kingBMove1);
                    KingWMove = Convert.ToBoolean(kingWMove1);

                    var rookMoves1 = xmlDocument.Descendants("RookMoves").Elements("Item").Select(item => item.Value).ToList();
                    for (int i = 0; i < rookMoves1.Count; i++)
                    {
                        if (rookMoves1[i] != "N")
                        {
                            RookMoves[i] = null;
                        }
                        else
                        {
                            RookMoves[i] = "N";
                        }
                    }

                    var cernyCapturedString1 = xmlDocument.Descendants("cernyCapturedString").Elements("Item").Select(item => item.Value).ToList();
                    foreach(var c in cernyCapturedString1)
                    {
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        imageColumn.Width = 100;
                        imageColumn.Image = figurka.rozpoznavaniFigurek(c);
                        cernySebran.Columns.Add(imageColumn);
                        cernyCapturedString.Add(c);
                    }

                    var bilyCapturedString1 = xmlDocument.Descendants("bilyCapturedString").Elements("Item").Select(item => item.Value).ToList();
                    foreach (var c in bilyCapturedString1)
                    {
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        imageColumn.Width = 100;
                        imageColumn.Image = figurka.rozpoznavaniFigurek(c);
                        bilySebran.Columns.Add(imageColumn);
                        bilyCapturedString.Add(c);
                    }

                    var Enpeasant1 = xmlDocument.Descendants("Enpeasant").Elements("Item").Select(item => item.Value).ToList();
                    EnPeasant[0] = Convert.ToInt32(Enpeasant1[0]);
                    EnPeasant[1] = Convert.ToInt32(Enpeasant1[1]);

                    MessageBox.Show("\n" + openFileDialog.FileName, "Soubor se úspěšně načetl");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načtení: " + ex.Message);
                }
            }
        }

        private void SaveXml(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XML Files|*.xml",
                Title = "Ulož si hru"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<string> tahy = new List<string>();
                    foreach (var i in poleTahy.Items)
                    {
                        tahy.Add(i.ToString());
                    }
                    List<string> pozice = vypisTahu.vratitListPozici();
                    List<string> RookMovesTemp = new List<string>();
                    foreach (string i in RookMoves)
                    {
                        if (i != "N")
                        {
                            RookMovesTemp.Add("A");
                        }
                        else
                        {
                            RookMovesTemp.Add(i);
                        }
                    }

                    XDocument xmlDocument = new XDocument(
                        new XElement(pozice[pozice.Count - 1],
                            new XElement("Pozice", pozice.Select(p => new XElement("Item", p))),
                            new XElement("Tahy", tahy.Select(t => new XElement("Item", t))),
                            new XElement("bilyNaRade", [bilyNaRade]),
                            new XElement("KingBMove", [KingBMove]),
                            new XElement("KingWMove", [KingWMove]),
                            new XElement("RookMoves", RookMovesTemp.Select(t => new XElement("Item", t))),
                            new XElement("cernyCapturedString", cernyCapturedString.Select(t => new XElement("Item", t))),
                            new XElement("bilyCapturedString", bilyCapturedString.Select(t => new XElement("Item", t))),
                            new XElement("Enpeasant", EnPeasant.Select(t => new XElement("Item", t)))
                        )
                    );
                    xmlDocument.Save(saveFileDialog.FileName);
                    MessageBox.Show("Uloženo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error při uložení: " + ex.Message, "Error");
                }
            }
        }
    }
}
