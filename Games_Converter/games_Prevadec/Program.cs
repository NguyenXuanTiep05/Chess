using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace prevadecHer
{
    internal class Prevod
    {
        static Dictionary<int, string[]> spravneSyntax = new Dictionary<int, string[]>
        {
            {0, new string[]{"a", "8"}},
            {1, new string[]{"b", "7"}},
            {2, new string[]{"c", "6"}},
            {3, new string[]{"d", "5"}},
            {4, new string[]{"e", "4"}},
            {5, new string[]{"f", "3"}},
            {6, new string[]{"g", "2"}},
            {7, new string[]{"h", "1"}},
        };

        static List<string> hryFinal = new List<string>();

        static void Main(string[] args)
        {
            string output = "C:/Users/Draxi/Downloads/games.txt";
            string soubor = "C:/Users/Draxi/Downloads/twic1498.pgn";

            List<string> lines = LoadPgn(soubor);
            List<List<string>> hra = ExtractHer(lines);
            ConvertHry(hra);

            File.WriteAllLines(output, hryFinal);
        }

        static void ConvertHry(List<List<string>> hra)
        {
            Dictionary<string, string> reverseDict = new Dictionary<string, string>();

            foreach (var entry in spravneSyntax)
            {
                reverseDict[entry.Value[0]] = entry.Key.ToString(); 
                reverseDict[entry.Value[1]] = entry.Key.ToString();
            }

            foreach (var game in hra)
            {
                List<string> converted = new List<string>();
                foreach (string move in game)
                {
                    if (Regex.IsMatch(move, @"^[NBRQK]?[a-h]?[1-8]?x?[a-h][1-8]"))
                    {
                        string destSquare = move.Substring(move.Length - 2);
                        if (reverseDict.ContainsKey(destSquare[0].ToString()) && reverseDict.ContainsKey(destSquare[1].ToString()))
                        {
                            string file = reverseDict[destSquare[0].ToString()];
                            string rank = reverseDict[destSquare[1].ToString()];
                            converted.Add($"{rank}{file}");
                        }
                        else
                        {
                            converted.Add(move);
                        }
                    }
                    else if (move == "O-O" || move == "O-O-O")
                    {
                        break;
                    }
                    else
                    {
                        converted.Add(move);
                    }
                }
                hryFinal.Add(string.Join(" ", converted));
            }
        }

        static List<string> LoadPgn(string soubor)
        {
            return new List<string>(File.ReadAllLines(soubor));
        }

        static List<List<string>> ExtractHer(List<string> lines)
        {
            List<List<string>> hry = new List<List<string>>();
            List<string> aktualniHra = new List<string>();
            Regex moveRegex = new Regex(@"\b([NBRQK]?[a-h]?[1-8]?x?[a-h][1-8]|O-O|O-O-O)\b");

            foreach (string line in lines)
            {
                if (line.StartsWith("["))
                {
                    if (aktualniHra.Count > 0)
                    {
                        hry.Add(new List<string>(aktualniHra));
                        aktualniHra.Clear();
                    }
                }
                else
                {
                    string cleanedLine = Regex.Replace(line, "\\d+\\.+", "");
                    MatchCollection matches = moveRegex.Matches(cleanedLine);
                    foreach (Match match in matches)
                    {
                        if (aktualniHra.Count < 10)
                        {
                            aktualniHra.Add(match.Value);
                        }
                    }
                }
            }

            if (aktualniHra.Count > 0)
            {
                hry.Add(aktualniHra);
            }

            return hry;
        }
    }
}