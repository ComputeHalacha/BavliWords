using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace dostuff
{
    class Program
    {
        static string path = @"C:\repos_git\BavliWords\Files";
        static Dictionary<string, string> Masechtos = new Dictionary<string, string>
        {
            { "ברכות","Berachos"},
            { "שבת","Shabbos"},
            { "עירובין","Eruvin"},
            { "פסחים","Pesachim"},
            { "שקלים","Shekalim"},
            { "יומא","Yoma"},
            { "סוכה","Sukkah"},
            { "ביצה","Beitzah"},
            { "ראש השנה","Rosh Hashana"},
            { "תענית","Taanis"},
            { "מגילה","Megillah"},
            { "מועד קטן","Moed Katan"},
            { "חגיגה","Chagigah"},
            { "יבמות","Yevamos"},
            { "כתובות","Kesubos"},
            { "נדרים","Nedarim"},
            { "נזיר","Nazir"},
            { "סוטה","Sotah"},
            { "גיטין","Gitin"},
            { "קידושין","Kiddushin"},
            {"בבא קמא" ,"Baba Kamma"},
            {"בבא מציעא" ,"Baba Metzia"},
            {"בבא בתרא" ,"Baba Batra"},
            {"סנהדרין" ,"Sanhedrin"},
            {"מכות" ,"Makkot"},
            {"שבועות" ,"Shevuot"},
            {"עבודה זרה" ,"Avodah Zarah"},
            {"הוריות" ,"Horayot"},
            {"זבחים" ,"Zevachim"},
            { "מנחות","Menachos"},
            { "חולין","Chullin"},
            { "בכורות","Bechoros"},
            { "ערכין","Arachin"},
            { "תמורה","Temurah"},
            { "כריתות","Kerisos"},
            { "מעילה","Meilah"},
            { "קנים","Kinnim"},
            { "תמיד","Tamid"},
            { "מדות","Midos"},
            { "נידה" ,"Niddah"}
        };
        static Dictionary<char, int> Gematriyos = new Dictionary<char, int>
        {
            {'א', 1},
            {'ב', 2},
            { 'ג', 3},
            { 'ד', 4},
            { 'ה', 5},
            { 'ו', 6},
            { 'ז', 7},
            { 'ח', 8},
            { 'ט', 9},
            { 'י', 10},
            { 'כ', 20},
            { 'ל', 30},
            { 'מ', 40},
            { 'נ', 50},
            { 'ס', 60},
            { 'ע', 70},
            { 'פ', 80},
            { 'צ', 90},
            { 'ק', 100},
            { 'ר', 200},
            { 'ש', 300},
            { 'ת', 400}
        };

        static void Main(string[] args)
        {
            //convertFromHTML();
            //Rename();
            getTable();
            //fixXml();
            Console.WriteLine("Press <ENTER> to (ironically) exit...");
            Console.ReadLine();
        }

        static int getGematriya(string text)
        {
            int tally = 0;
            foreach (char letter in text)
            {
                tally += Gematriyos[letter];
            }
            return tally;
        }
        static void convertFromHTML()
        {
            Encoding hebEnc = Encoding.GetEncoding(1255),
                utf8 = Encoding.UTF8;
            Regex re1 = new Regex(@"\<HR\><P CLASS\=s\>.+?</P>\n\<HR\>\n",
                  RegexOptions.Singleline | RegexOptions.Compiled),
            re2 = new Regex(@".+?\<H2\>(.+)\<\/H2>(.+)\<\/DIV\>\<\/BODY\>\<\/HTML\>",
                RegexOptions.Singleline | RegexOptions.Compiled),
            re3 = new Regex(@"\<HR\>.+?<HR\>\n",
                  RegexOptions.Singleline | RegexOptions.Compiled);
            foreach (string file in Directory.GetFiles(path, "*.htm"))
            {
                Console.WriteLine("Started " + file);
                string text = File.ReadAllText(file, hebEnc);
                text = utf8.GetString(Encoding.Convert(hebEnc, utf8, hebEnc.GetBytes(text)));
                text = re1.Replace(text, "</P>");
                text = re2.Replace(text, "<CONTENT>\n<H2>$1</H2>\n$2\n</CONTENT>");
                text = text.Replace("\n\n", "\n")
                    .Replace("\n<BR></P>", "</P>")
                    .Replace("\n\n", "\n");
                text = re3.Replace(text, "");
                File.WriteAllText(file + ".xml", text, utf8);
                Console.WriteLine("Processed " + file + ".xml");
            }
        }
        static void rename()
        {
            Regex re = new Regex(@"\<H2\>(.+?)\</H2\>",
                    RegexOptions.Singleline | RegexOptions.Compiled),
                re2 = new Regex(@"מסכת (.+) פרק (.+)", RegexOptions.Compiled);
            foreach (string file in Directory.GetFiles(path, "*.xml"))
            {
                Console.WriteLine("Started " + file);

                string text = File.ReadAllText(file);
                var heading = re.Match(text).Groups[1].Value;
                var groups = re2.Match(heading).Groups;
                string maseches = groups[1].Value, perek = groups[2].Value;
                File.WriteAllText(path + @"\" +
                    Masechtos[maseches] + "_" + getGematriya(perek).ToString() + ".xml", text);
            }
        }
        static void getTable()
        {
            var dict = new Dictionary<string, long>();
            Regex re = new Regex(@"\<t\>(.+)\<\/t\>", RegexOptions.Compiled);
            foreach (string line in File.ReadLines(path + "\\_ShasData.xml", Encoding.UTF8))
            {
                var results = re.Match(line).Groups;
                if (!(results.Count > 1)) continue;
                var words = results[1].Value;
                if (!string.IsNullOrWhiteSpace(words))
                {
                    foreach (string word in words.Split(' '))
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            var cleaned = word.Trim().Replace(",", "");
                            if (dict.ContainsKey(cleaned))
                            {
                                dict[cleaned]++;
                            }
                            else
                            {
                                dict.Add(cleaned, 1);
                            }
                        }
                    }
                }
            }
            var list = dict.ToList();
            list.Sort((kvp1, kvp2) => kvp2.Value.CompareTo(kvp1.Value));
            if (File.Exists(path + "\\results.csv"))
            {
                File.Delete(path + "\\results.csv");
            }
            using (var file = File.OpenWrite(path + "\\results.csv"))
            {
                using (var sw = new StreamWriter(file, Encoding.UTF8))
                {
                    sw.WriteLine("word,instances");
                    foreach (var kvp in list)
                    {
                        sw.WriteLine(kvp.Key + "," + kvp.Value.ToString());
                    }
                }
            }
        }
        static void fixXml()
        {
            Regex reHeading = new Regex(@"\<heading\>מסכת (.+?) פרק (.+?)\<\/heading\>", RegexOptions.Compiled);
            Regex reDesc = new Regex(@"\<desc\>דף (.+?),(.) (.+?)\<\/desc\>", RegexOptions.Compiled);
            Regex reText = new Regex(@".+\/desc\>(.+)\<\/amud\>", RegexOptions.Compiled);

            if (File.Exists(path + "\\_ShasData.xml"))
            {
                File.Delete(path + "\\_ShasData.xml");
            }

            using (var file = File.OpenWrite(path + "\\_ShasData.xml"))
            {
                using (var sw = new StreamWriter(file, Encoding.UTF8))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
                    sw.WriteLine("<shas>");
                    string masechta = null;
                    int daf = 0,
                        amud = 0;
                    string text = null;

                    foreach (string line in File.ReadLines(path + "\\_SHAS.xml"))
                    {
                        var results = reHeading.Match(line).Groups;
                        if (results.Count > 1)
                        {
                            string thisMasechta = Masechtos[results[1].Value];
                            if (thisMasechta != masechta)
                            {
                                masechta = thisMasechta;
                                daf = 0;
                            }
                        }
                        else
                        {
                            results = reDesc.Match(line).Groups;
                            if (!(results.Count > 1)) continue;
                            int thisDaf = getGematriya(results[1].Value),
                            thisAmud = getGematriya(results[2].Value);
                            string thisText = reText.Replace(line, "$1");

                            if (daf > 0 && thisDaf == daf && thisAmud == amud)
                            {
                                text += thisText;
                            }
                            else
                            {
                                if (daf != 0)
                                {
                                    sw.WriteLine("<amud><m>{0}</m><d>{1}</d><a>{2}</a><t>{3}</t></amud>",
                                        masechta, daf, amud, text);
                                }

                                daf = thisDaf;
                                amud = thisAmud;
                                text = thisText;
                            }

                        }
                    }
                    sw.WriteLine("</shas>");
                }
            }
        }

    }
}
