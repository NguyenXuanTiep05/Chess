using System;
using System.Windows.Forms;

namespace šachys.Properties
{
    public partial class Povyseni : Form
    {
        TridaFigurky tridaFigurky = new TridaFigurky();
        private Form1 hlavniForm;

        public Povyseni(Form1 form)
        {
            InitializeComponent();
            hlavniForm = form;
        }
        private void Povyseni_Load(object sender, EventArgs e)
        {

        }
        string VybranaFigurka;

        private void PovyseniClick(object sender, EventArgs e)
        {
            VybranaFigurka = (sender as Button).Tag.ToString();
            if (hlavniForm.bilyNaRade != true)
            {
                VybranaFigurka = VybranaFigurka.ToLower();
            }
            hlavniForm.stavHracihoPole[hlavniForm.souradniceMinulehoPolicka[0], hlavniForm.souradniceMinulehoPolicka[1]] = 0;
            hlavniForm.stavHracihoPole[hlavniForm.souradniceAktualnihoPolicka[0], hlavniForm.souradniceAktualnihoPolicka[1]] = 1;
            hlavniForm.hraciPole[hlavniForm.souradniceMinulehoPolicka[0], hlavniForm.souradniceMinulehoPolicka[1]].BackgroundImage = null;
            hlavniForm.hraciPole[hlavniForm.souradniceMinulehoPolicka[0], hlavniForm.souradniceMinulehoPolicka[1]].Tag = null;
            hlavniForm.hraciPole[hlavniForm.souradniceAktualnihoPolicka[0], hlavniForm.souradniceAktualnihoPolicka[1]].BackgroundImage = tridaFigurky.rozpoznavaniFigurek(VybranaFigurka);
            hlavniForm.hraciPole[hlavniForm.souradniceAktualnihoPolicka[0], hlavniForm.souradniceAktualnihoPolicka[1]].Tag = VybranaFigurka;
            this.Close();

        }

        private void Povyseni_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (VybranaFigurka == null)
            {
                VybranaFigurka = "Q";
                if (hlavniForm.bilyNaRade != true)
                {
                    VybranaFigurka = VybranaFigurka.ToLower();
                }
                hlavniForm.stavHracihoPole[hlavniForm.souradniceMinulehoPolicka[0], hlavniForm.souradniceMinulehoPolicka[1]] = 0;
                hlavniForm.stavHracihoPole[hlavniForm.souradniceAktualnihoPolicka[0], hlavniForm.souradniceAktualnihoPolicka[1]] = 1;
                hlavniForm.hraciPole[hlavniForm.souradniceMinulehoPolicka[0], hlavniForm.souradniceMinulehoPolicka[1]].BackgroundImage = null;
                hlavniForm.hraciPole[hlavniForm.souradniceMinulehoPolicka[0], hlavniForm.souradniceMinulehoPolicka[1]].Tag = null;
                hlavniForm.hraciPole[hlavniForm.souradniceAktualnihoPolicka[0], hlavniForm.souradniceAktualnihoPolicka[1]].BackgroundImage = tridaFigurky.rozpoznavaniFigurek(VybranaFigurka);
                hlavniForm.hraciPole[hlavniForm.souradniceAktualnihoPolicka[0], hlavniForm.souradniceAktualnihoPolicka[1]].Tag = VybranaFigurka;
            }
            this.Close();
        }
    }
}
