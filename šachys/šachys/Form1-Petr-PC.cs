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
        List<Button> hraciPole = new List<Button>();
        int velikostPolicka = 100;
        CheckedListBox poleTahy = new CheckedListBox();
        TridaFigurky figurka = new TridaFigurky();
        TahFigurka TahFigurka = new TahFigurka();

        
        public Form1()
        {
            InitializeComponent();
            kresleniHracihoPole();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.Gray;
        }

        private void kresleniHracihoPole()
        {
            for (int i = 0; i < poleSize; i++)
            {
                for (int j = 0; j < poleSize; j++)
                {
                    Button button = new Button
                    {
                        Size = new Size(velikostPolicka, velikostPolicka),
                        Text = null,
                        BackgroundImageLayout = ImageLayout.Zoom
                    };
                    if (j == 0)
                    {
                        button.Location = new Point(15, i * velikostPolicka + 15);
                    }
                    else
                    {
                        button.Location = new Point(j * velikostPolicka + 15, i * velikostPolicka + 15);
                    }
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    if ((i + j) % 2 == 0)
                    {
                        button.BackColor = Color.FromArgb(235, 236, 208);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 236, 208);
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 236, 208);
                    }

                    else
                    {
                        button.BackColor = Color.FromArgb(95, 129, 62);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(95, 129, 62);
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(95, 129, 62);
                    }
                    button.Click += pole_Click;
                    this.Controls.Add(button);
                    hraciPole.Add(button);
                }
            }
            poleTahy.Location = new Point(poleSize * velikostPolicka + 50, 15);
            poleTahy.Size = new Size(450, poleSize * velikostPolicka);
            this.Controls.Add(poleTahy);
            this.Size = new Size((poleSize * velikostPolicka) + poleTahy.Width + 84, poleSize * velikostPolicka + 65);

            

            hraciPole[34].BackgroundImage = figurka.rozpoznavaniFigurek("r");
            hraciPole[34].Tag = "r";
            hraciPole[32].BackgroundImage = figurka.rozpoznavaniFigurek("R");
            hraciPole[32].Tag = "R";
        }
        string vybranaFigurka;
        int vybranePole;
        bool bilyNaRade = true;
        private void pole_Click(object sender, EventArgs e)
        {
            if ((sender as Button).BackgroundImage != null && (sender as Button).Tag != null)
            {
                vybranaFigurka = null;
                vybranePole = 0;
                string figurkaTag = (sender as Button).Tag.ToString();
                if (bilyNaRade == true && figurkaTag == figurkaTag.ToUpper())
                {
                    vybranaFigurka = (sender as Button).Tag.ToString();
                    vybranePole = hraciPole.IndexOf((sender as Button));
                }
                else if (bilyNaRade == false && figurkaTag == figurkaTag.ToLower())
                {
                    vybranaFigurka = (sender as Button).Tag.ToString();
                    vybranePole = hraciPole.IndexOf((sender as Button));
                }

                List<int> Text = TahFigurka.vypocetTahu(hraciPole.IndexOf((sender as Button)),vybranaFigurka);
                for (int i = 0; i < Text.Count; i++)
                {
                    if (Text[i] > -1 && Text[i] < 64)
                    {
                        hraciPole[Text[i]].BackColor = Color.Gray;
                    }
                }
            }
            else if (vybranaFigurka != null)
            {

                List<int> legalniTahy = TahFigurka.vypocetTahu(vybranePole, vybranaFigurka);

                for (int i = 0; i < legalniTahy.Count; i++)
                {
                    if (hraciPole.IndexOf((sender as Button)) == legalniTahy[i])
                    {
                        (sender as Button).BackgroundImage = figurka.rozpoznavaniFigurek(vybranaFigurka);
                        (sender as Button).Tag = vybranaFigurka;
                        hraciPole[vybranePole].BackgroundImage = null;
                        hraciPole[vybranePole].Tag = null;
                        if (bilyNaRade == true)
                        {
                            bilyNaRade = false;
                        }
                        else
                        {
                            bilyNaRade = true;
                        }
                        poleTahy.Items.Add(vybranaFigurka + hraciPole.IndexOf((sender as Button)));
                        vybranaFigurka = null;
                        vybranePole = 0;
                        break;
                    }
                }
            }
        }
    }
}
