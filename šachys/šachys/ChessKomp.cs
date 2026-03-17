using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace šachys
{
    internal class ChessKomp
    {
        detekceSachu detekceSachu = new detekceSachu();
        realizacePohybu realizacePohybu = new realizacePohybu();
        evaluaceFigurek evaluaceFigurek = new evaluaceFigurek();
        TridaFigurky TridaFigurky = new TridaFigurky();

        public ChessKomp()
        {
            LoadOpenings();
        }

        int[] minuleSouradniceGlobal;
        int[] aktualniSouradniceGlobal;
        int[] enpeasantGlobal = new int[2];
        string vybranaFigurkaGlobal;
        Button vybranaFigurkaBtnGlobal;
        string sebranaFigurkaGlobal;
        bool povyseni;
        List<TAH> platneTahy;
        private Dictionary<string, double> transpositionTable = new Dictionary<string, double>();
        private Dictionary<int, List<int[]>> NacteneHry = new Dictionary<int, List<int[]>>();
        private int AktualnitTahIndex = 0;
        private int[] posledniTah;
        private  List<int[]> posledniTahyList = new List<int[]>();

        private void LoadOpenings()
        {
            string[] lines = Properties.Resources.games2.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            int Index = 0;

            foreach (string radek in lines)
            {
                string[] moves = radek.Split(' ');
                List<int[]> moveList = new List<int[]>();

                foreach (string move in moves)
                {
                    if (move.Length == 2)
                    {
                        int from = move[0] - '0';
                        int to = move[1] - '0';
                        moveList.Add([ from, to ]);
                    }
                }

                NacteneHry[Index++] = moveList;
            }
        }

        private int zjistitHru(List<int[]> posledniTah)
        {
            if (AktualnitTahIndex >= 10) { return -1; }
            Random random = new Random();
            List<int> matchingIndices = new List<int>();

            foreach (var kvp in NacteneHry)
            {
                int index = kvp.Key;
                List<int[]> game = kvp.Value;
                if (game.Count <= AktualnitTahIndex) continue;

                bool match = true;
                for (int j = 0; j < posledniTah.Count; j++)
                {
                    if (!posledniTah[j].SequenceEqual(game[j]))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    matchingIndices.Add(index);
                }
            }

            if (matchingIndices.Count > 0)
            {
                return matchingIndices[random.Next(matchingIndices.Count)];
            }

            return -1;
        }

        //private int zjistitHru(int[] posledniTah)
        //{
        //    if (AktualnitTahIndex >= 10) { return -1; }
        //    Random random = new Random();
        //    for (int i = 0; i < 15000; i++)
        //    {
        //        int index = random.Next(0, NacteneHry.Count);
        //        if (NacteneHry[index].Count < AktualnitTahIndex) continue;
        //        if (NacteneHry[index][AktualnitTahIndex - 1].SequenceEqual(posledniTah))
        //        {
        //            return index;
        //        }
        //    }

        //    return -1;
        //}


        public Button[,] AI(Button[,] board, int[,] stavHracihoPole, bool bilyNaRade)
        {
            Array.Clear(enpeasantGlobal,0,2);
            povyseni = false;
            int maxDepth = 10;
            Button vybranaFigurka = null;
            int[] MoveTo = null;
            double bestScore = bilyNaRade ? double.NegativeInfinity : double.PositiveInfinity;

            int klicHry = zjistitHru(posledniTahyList);
            if (klicHry != -1)
            {
                platneTahy = srovnatTahy(board, stavHracihoPole, bilyNaRade);
                int[] tempMove = NacteneHry[klicHry][AktualnitTahIndex];
                foreach (TAH move in platneTahy)
                {
                    if (move.To[0] == tempMove[0] && move.To[1] == tempMove[1])
                    {
                        int[] from = move.From;
                        int[] To = move.To;
                        var originalBg = board[To[0], To[1]].BackgroundImage;
                        var originalTag = board[To[0], To[1]].Tag;
                        var figurkaTag = board[from[0], from[1]].Tag.ToString();
                        polozeniFigurek(figurkaTag, from, To, board);
                        int[,] tempStavPole = updateStavHracihoPole(board);

                        double score = evaluaceFigurek.evaluate(board,bilyNaRade, tempStavPole);

                        polozeniFigurek(figurkaTag, To, from, board);
                        board[To[0], To[1]].BackgroundImage = originalBg;
                        board[To[0], To[1]].Tag = originalTag;
                        if ((bilyNaRade && score > bestScore) || (!bilyNaRade && score < bestScore))
                        {
                            bestScore = score;
                            MoveTo = To;
                            vybranaFigurka = board[from[0], from[1]];
                        }
                    }
                }
            }

            else
            {

                Stopwatch timelimit = new Stopwatch();
                timelimit.Start();
                for (int depth = 1; depth <= maxDepth; depth++)
                {
                    platneTahy = srovnatTahy(board, stavHracihoPole, bilyNaRade);
                    foreach (TAH move in platneTahy)
                    {
                        int[] from = move.From;
                        int[] To = move.To;
                        var originalBg = board[To[0], To[1]].BackgroundImage;
                        var originalTag = board[To[0], To[1]].Tag;
                        var figurkaTag = board[from[0], from[1]].Tag.ToString();
                        polozeniFigurek(figurkaTag, from, To, board);
                        int[,] tempStavPole = updateStavHracihoPole(board);

                        double score = Minimax(board, tempStavPole, depth, !bilyNaRade, double.NegativeInfinity, double.PositiveInfinity);

                        polozeniFigurek(figurkaTag, To, from, board);
                        board[To[0], To[1]].BackgroundImage = originalBg;
                        board[To[0], To[1]].Tag = originalTag;

                        if ((bilyNaRade && score > bestScore) || (!bilyNaRade && score < bestScore))
                        {
                            bestScore = score;
                            MoveTo = To;
                            vybranaFigurka = board[from[0], from[1]];
                        }
                        if (timelimit.ElapsedMilliseconds > 10000)
                        {
                            break;
                        }
                    }
                }
            }
            int[] zSour = realizacePohybu.hledacSouradnic(vybranaFigurka, board);
            minuleSouradniceGlobal = zSour;
            aktualniSouradniceGlobal = MoveTo;
            vybranaFigurkaGlobal = vybranaFigurka.Tag.ToString();
            vybranaFigurkaBtnGlobal = vybranaFigurka;
            if (board[MoveTo[0], MoveTo[1]].Tag != null)
            {
                sebranaFigurkaGlobal = board[MoveTo[0], MoveTo[1]].Tag.ToString();
            }

            if ((MoveTo[0] == 7 || MoveTo[0] == 0) && vybranaFigurka.Tag.ToString().ToLower() == "p")
            {
                povyseni = true;
                return polozeniFigurek(bilyNaRade ? "Q" : "q", zSour, MoveTo, board);
            }
            else if (vybranaFigurkaGlobal.ToString() == "p" && Math.Abs(zSour[0] - MoveTo[0]) == 2)
            {
                enpeasantGlobal = MoveTo;
            }
            return polozeniFigurek(vybranaFigurka.Tag.ToString(), zSour, MoveTo, board);
        }

        private double Minimax(Button[,] board, int[,] stavHracihoPole, int depth, bool bilyNaRade, double alpha, double beta)
        {

            string boardHash = GetBoardHash(board, bilyNaRade);
            if (transpositionTable.ContainsKey(boardHash))
            {
                return transpositionTable[boardHash];
            }

            if (depth == 0)
            {
                double evaluation = captureMinMax(board, stavHracihoPole, bilyNaRade, alpha, beta);
                transpositionTable[boardHash] = evaluation;
                return evaluation;
            }

            platneTahy = srovnatTahy(board, stavHracihoPole, bilyNaRade);
            if (platneTahy.Count <= 0) return bilyNaRade ? double.NegativeInfinity : double.PositiveInfinity;
            if (jeKralSach(board, bilyNaRade, platneTahy))
            {
                depth++;
            }

            foreach (TAH move in platneTahy)
            {
                int[] From = move.From;
                int[] To = move.To;
                var originalBg = board[To[0], To[1]].BackgroundImage;
                var originalTag = board[To[0], To[1]].Tag;
                var playedPieceTag = board[From[0], From[1]].Tag.ToString();

                polozeniFigurek(playedPieceTag, From, To, board);
                int[,] tempStavPole = updateStavHracihoPole(board);

                double score = Minimax(board, tempStavPole, depth - 1, !bilyNaRade, -beta, -alpha);

                polozeniFigurek(playedPieceTag, To, From, board);
                board[To[0], To[1]].BackgroundImage = originalBg;
                board[To[0], To[1]].Tag = originalTag;

                if (score >= beta)
                {
                    return beta;
                }

                alpha = Math.Max(alpha, score);
            }
            transpositionTable[boardHash] = alpha;
            return alpha;
        }

        private double captureMinMax(Button[,] board, int[,] stavHracihoPole, bool bilyNaRade, double alpha, double beta)
        {
            double standPat = evaluaceFigurek.evaluate(board, bilyNaRade, stavHracihoPole);
            double Bestscore = standPat;
            if (standPat >= beta)
            {
                return standPat;
            }
            alpha = Math.Max(alpha, standPat);


            List<TAH> tahy = GeneraceTahuAI(stavHracihoPole, board, bilyNaRade, true);
            if (tahy.Count <= 0) { return alpha; }
            foreach (TAH move in tahy)
            {
                int[] From = move.From;
                int[] To = move.To;
                var originalBg = board[To[0], To[1]].BackgroundImage;
                var originalTag = board[To[0], To[1]].Tag;
                var playedPieceTag = board[From[0], From[1]].Tag.ToString();

                polozeniFigurek(playedPieceTag, From, To, board);
                int[,] tempStavPole = updateStavHracihoPole(board);

                double score = captureMinMax(board, tempStavPole, !bilyNaRade, -beta, -alpha);

                polozeniFigurek(playedPieceTag, To, From, board);
                board[To[0], To[1]].BackgroundImage = originalBg;
                board[To[0], To[1]].Tag = originalTag;

                if (score >= beta)
                {
                    return score;
                }
                Bestscore = Math.Max(score, Bestscore);
                alpha = Math.Max(score, alpha);
            }
            return Bestscore;
        }

        private string GetBoardHash(Button[,] board, bool bilyNaRade)
        {
            var sb = new System.Text.StringBuilder(65);

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    sb.Append(board[y, x].Tag?.ToString() ?? " ");
                }
            }

            sb.Append(bilyNaRade ? 'W' : 'B');
            return sb.ToString();
        }

        List<TAH> sebraniList;
        List<TAH> sach;
        List<TAH> povyseniList;
        List<TAH> ostatni;
        private List<TAH> srovnatTahy(Button[,] hraciPole, int[,] stavPole, bool bilyNaRade)
        {
            List<TAH> tahy = GeneraceTahuAI(stavPole, hraciPole, bilyNaRade, false);
            if (tahy.Count <= 0) { return tahy; }

            sebraniList = new List<TAH>();
            sach = new List<TAH>();
            povyseniList = new List<TAH>();
            ostatni = new List<TAH>();

            foreach (TAH move in tahy)
            {
                int[] souradnice = move.To;
                string vybranaFigurka = hraciPole[move.From[0], move.From[1]].Tag.ToString();
                string captureFiurka = hraciPole[souradnice[0], souradnice[1]].Tag?.ToString();

                if (detekceSachu.testTahuProtiSachuMatu(stavPole, hraciPole, !bilyNaRade, hraciPole[move.From[0], move.From[1]], souradnice))
                {
                    sach.Add(move);
                }
                else if (captureFiurka != null && ((Char.IsUpper(captureFiurka[0]) && !bilyNaRade) || (Char.IsLower(captureFiurka[0]) && bilyNaRade)))
                {
                    sebraniList.Add(move);
                }
                else if ((souradnice[0] == 0 || souradnice[0] == 7) && vybranaFigurka.ToLower() == "p")
                {
                    povyseniList.Add(move);
                }
                else
                {
                    ostatni.Add(move);
                }
            }

            tahy.Clear();
            tahy.AddRange(sebraniList);
            tahy.AddRange(sach);
            tahy.AddRange(povyseniList);
            tahy.AddRange(ostatni);

            return tahy;
        }
        public List<TAH> GeneraceTahuAI(int[,] stavPole, Button[,] hraciPole, bool bilyNaRade, bool captureMoves)
        {
            List<TAH> moves = new List<TAH>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Button b = hraciPole[y, x];
                    if (b.Tag == null) continue;

                    string figurkaTag = b.Tag.ToString();
                    if (((bilyNaRade && char.IsUpper(figurkaTag[0])) || (!bilyNaRade && char.IsLower(figurkaTag[0]))))
                    {
                        List<int> tahy = detekceSachu.tahyProtiSachuMatu(stavPole, hraciPole, bilyNaRade, b);
                        if (tahy.Count <= 0) continue;


                        if (!captureMoves)
                        {

                            int[] souradniceFigurky = [y, x];
                            for (int i = 0; i < tahy.Count; i += 2)
                            {
                                moves.Add(new TAH(souradniceFigurky, new int[] { tahy[i], tahy[i + 1] }));
                            }
                        }

                        else if (captureMoves)
                        {

                            int[] souradniceFigurky = [y, x];
                            for (int i = 0; i < tahy.Count; i += 2)
                            {
                                string figSebrat = hraciPole[tahy[i], tahy[i + 1]].Tag?.ToString();
                                if (figSebrat != null)
                                {
                                    if ((bilyNaRade == false && char.IsUpper(figSebrat[0])) || (bilyNaRade == true && char.IsLower(figSebrat[0])))
                                    {
                                        moves.Add(new TAH(souradniceFigurky, new int[] { tahy[i], tahy[i + 1] }));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return moves;
        }


        private bool jeKralSach(Button[,] hraciPole, bool bilyNaRade, List<TAH> moves)
        {
            int[] poziceKrale = detekceSachu.hledacPoziceKrale(hraciPole, bilyNaRade);
            if (poziceKrale == null) { return true; }
            foreach (TAH t in moves)
            {
                if (t.To[0] == poziceKrale[0] && t.To[1] == poziceKrale[1])
                {
                    return true;
                }
            }
            return false;
        }

        private Button[,] polozeniFigurek(string kodfigurky, int[] From, int[] To, Button[,] hraciPole)
        {
            hraciPole[From[0], From[1]].BackgroundImage = null;
            hraciPole[From[0], From[1]].Tag = null;
            hraciPole[To[0], To[1]].BackgroundImage = TridaFigurky.rozpoznavaniFigurek(kodfigurky);
            hraciPole[To[0], To[1]].Tag = kodfigurky;
            return hraciPole;
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

        public int[] vratitAktualniPole()
        {
            return aktualniSouradniceGlobal;
        }
        public int[] vratitMinulePole()
        {
            return minuleSouradniceGlobal;
        }
        public string vratitFigurku()
        {
            return vybranaFigurkaGlobal;
        }
        public Button vratitBtn()
        {
            return vybranaFigurkaBtnGlobal;
        }
        public string vratitSebranaFigurka()
        {
            return sebranaFigurkaGlobal;
        }
        public bool vratitPovysenibool()
        {
            return povyseni;
        }
        public int[] vratitEnpeasant()
        {
            return enpeasantGlobal;
        }

        public List<int[]> vratitMinTahy()
        {
            return posledniTahyList;
        }

        public void endGameReset()
        {
            posledniTahyList.Clear();
            AktualnitTahIndex = 0;
        }

        public void getCisloTahu(int cislo)
        {
            AktualnitTahIndex = cislo;
        }

        public void getPosledniTah(int[] tah)
        {
            posledniTah = (int[])tah.Clone();
            posledniTahyList.Add(posledniTah);
        }

    }
}
