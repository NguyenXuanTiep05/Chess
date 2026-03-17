using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace šachys
{
    public partial class Form1 : Form
    {
        int poleSize = 8;
        Button[,] hraciPole = new Button[8,8];
        int velikostPolicka = 70;
        int[,] stavHracihoPole = {
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
        };
        CheckedListBox poleTahy = new CheckedListBox();
        TridaFigurky figurka = new TridaFigurky();
        TahFigurka TahFigurka = new TahFigurka();
        Color barvaLicha = Color.FromArgb(95, 129, 62);
        Color barvaSuda = Color.FromArgb(235, 236, 208);



        public Form1()
        {
            InitializeComponent();
            kresleniHracihoPole();
            kresleniFigurek();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.Gray;
        }

        private void kresleniHracihoPole()
        {
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
                        button.Location = new Point(15, y * velikostPolicka + 15);
                    }
                    else
                    {
                        button.Location = new Point(x * velikostPolicka + 15, y * velikostPolicka + 15);
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
                    hraciPole[y,x] = button;
                }
            }
            poleTahy.Location = new Point(poleSize * velikostPolicka + 50, 15);
            poleTahy.Size = new Size(450, poleSize * velikostPolicka);
            this.Controls.Add(poleTahy);
            this.Size = new Size((poleSize * velikostPolicka) + poleTahy.Width + 84, poleSize * velikostPolicka + 65);
        }



        string PostaveniFigurek = "r00n01b02q03k04b05n06r07p10p11p12p13p14p15p16p17P60P61P62P63P64P65P66P67R70n71B72Q73K74B75n76R77";
        public void kresleniFigurek()
        {
            for (int i = 0; i < PostaveniFigurek.Length; i+= 3)
            {
                char yT = PostaveniFigurek[i + 1];
                char xT = PostaveniFigurek[i + 2];
                int y = yT - '0';
                int x = xT - '0';
                char pismeno = PostaveniFigurek[i];
                hraciPole[y,x].BackgroundImage = figurka.rozpoznavaniFigurek(pismeno.ToString());
                hraciPole[y,x].Tag = pismeno;
                stavHracihoPole[y, x] = 1;
            }
        }



        string vybranaFigurka;
        int vybranePoleX;
        int vybranePoleY;
        bool bilyNaRade = true;
        int[] souradniceMinulehoPolicka;
        int[] souradniceAktualnihoPolicka;

        private void pole_Click(object sender, EventArgs e)
        {
            if ((sender as Button).BackgroundImage != null && (sender as Button).Tag != null)
            {
                souradniceMinulehoPolicka = kordFinder(sender as Button);
                vybranaFigurka = null;
                vybranePoleY = 0;
                vybranePoleX = 0;
                string figurkaTag = (sender as Button).Tag.ToString();
                if (bilyNaRade == true && figurkaTag == figurkaTag.ToUpper())
                {
                    vybranaFigurka = (sender as Button).Tag.ToString();
                    vybranePoleY = souradniceMinulehoPolicka[0];
                    vybranePoleX = souradniceMinulehoPolicka[1];
                }
                else if (bilyNaRade == false && figurkaTag == figurkaTag.ToLower())
                {
                    vybranaFigurka = (sender as Button).Tag.ToString();
                    vybranePoleY = souradniceMinulehoPolicka[0];
                    vybranePoleX = souradniceMinulehoPolicka[1];
                }

                List<int> test = TahFigurka.vypocetTahu(souradniceMinulehoPolicka, vybranaFigurka);
                poleTahy.Items.Add(test.Count);
                poleTahy.Items.Add("");
                poleTahy.Items.Add("");
                poleTahy.Items.Add("");
                for (int i = 0; i < test.Count; i += 2)
                {
                    poleTahy.Items.Add(test[i]);
                    poleTahy.Items.Add(test[i + 1]);
                    hraciPole[test[i], test[i + 1]].BackColor = Color.LightGray;
                }
            }
            else if (vybranaFigurka != null)
            {
                List<int> legalniTahy = TahFigurka.vypocetTahu(souradniceMinulehoPolicka, vybranaFigurka);
                souradniceAktualnihoPolicka = kordFinder(sender as Button);
                for (int x = 0; x < legalniTahy.Count; x+=2)
                {
                    if (souradniceAktualnihoPolicka[0] == legalniTahy[x] && souradniceAktualnihoPolicka[1] == legalniTahy[x + 1])
                    {
                        (sender as Button).BackgroundImage = figurka.rozpoznavaniFigurek(vybranaFigurka);
                        (sender as Button).Tag = vybranaFigurka;
                        hraciPole[vybranePoleY, vybranePoleX].BackgroundImage = null;
                        hraciPole[vybranePoleY, vybranePoleX].Tag = null;

                        List<int> test = TahFigurka.vypocetTahu(souradniceMinulehoPolicka, vybranaFigurka);
                        poleTahy.Items.Add(test.Count);
                        poleTahy.Items.Add("");
                        poleTahy.Items.Add("");
                        poleTahy.Items.Add("");
                        for (int i = 0; i < test.Count; i += 2)
                        {
                            poleTahy.Items.Add(test[i]);
                            poleTahy.Items.Add(test[i + 1]);
                            hraciPole[test[i], test[i + 1]].BackColor = Color.Red;
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            for (int z = 0; z < 8; z++)
                            {
                                poleTahy.Items.Add(stavHracihoPole[i, z]);
                            }
                        }

                        vybranaFigurka = null;
                        vybranePoleY = 0;
                        vybranePoleX = 0;

                        if (bilyNaRade == true)
                        {
                            bilyNaRade = false;
                        }
                        else
                        {
                            bilyNaRade = true;
                        }
                        break;
                    }
                }
                //for (int i = 0; i < legalniTahy.Count; i++)
                //{
                //    poleTahy.Items.Add(legalniTahy[i]);
                //}
                //for (int i = 0; i < souradniceAktualnihoPolicka.Length; i++)
                //{
                //    poleTahy.Items.Add(souradniceAktualnihoPolicka[i]);
                //}
            }
        }
        public int[] kordFinder(Button kordButton)
        {
            int[] kords = new int[2];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (hraciPole[i, j].Equals(kordButton))
                    {
                        kords[0] = i;
                        kords[1] = j;
                    }
                }
            }
            return kords;
        }
    }
}
