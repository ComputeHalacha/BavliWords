using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace WordsInShas
{
    static class Program
    {
        private static char[] _hundreds = new char[] { 'ק', 'ר', 'ש', 'ת' };
        private static char[] _sings = new char[] { 'א', 'ב', 'ג', 'ד', 'ה', 'ו', 'ז', 'ח', 'ט' };
        private static char[] _tens = new char[] { 'י', 'כ', 'ל', 'מ', 'נ', 'ס', 'ע', 'פ', 'צ' };

        /// <summary>
        /// Converts a number into its Jewish number equivalent. I.E. 254 is רכ"ד
        /// NOTE: The exact thousands numbers (1000, 2000, 3000 etc.)
        /// will look awfully similar to the single digits, but will be formatted with a double apostrophe I.E. 2000 = "''ב"
        /// </summary>
        /// <param name="number">The number to convert</param>
        /// <returns>A Hebrew string representation of the number</returns>
        public static string ToNumberHeb(this int number)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Min value is 1");
            }

            if (number > 9999)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Max value is 9999");
            }

            int n = number;
            StringBuilder retval = new StringBuilder();

            if (n >= 1000)
            {
                retval.AppendFormat("{0}'", _sings[((n - (n % 1000)) / 1000) - 1]);
                n = n % 1000;
            }

            while (n >= 400)
            {
                retval.Append('ת');
                n -= 400;
            }

            if (n >= 100)
            {
                retval.Append(_hundreds[((n - (n % 100)) / 100) - 1]);
                n = n % 100;
            }

            if (n == 15)
            {
                retval.Append("טו");
            }
            else if (n == 16)
            {
                retval.Append("טז");
            }
            else
            {
                if (n > 9)
                {
                    retval.Append(_tens[((n - (n % 10)) / 10) - 1]);
                }
                if ((n % 10) > 0)
                {
                    retval.Append(_sings[(n % 10) - 1]);
                }
            }
            if (number > 999 && (number % 1000 < 10))
            {
                retval.Insert(0, "'");
            }
            else if (retval.Length == 1)
            {
                retval.Insert(0, "'");
            }
            else
            {
                retval = retval.Insert(retval.Length - 1, "\"");
            }

            return retval.ToString();
        }

        public static string getHtml(IEnumerable<string> items, int fromNumber, int toNumber)
        {
            var sb = new StringBuilder();
            var list = GetDataList(fromNumber, toNumber);
            var doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.ShasData);
            foreach (string item in items)
            {
                sb.AppendFormat("<h2>{0}</h2>", ParseTag(item));
                string allText = "";
                foreach (XmlElement n in doc.SelectNodes(GetXpath(item)))
                {
                    allText += n.InnerText + " ";
                }
                string[] words = allText.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);
                int count = 1;
                foreach (SingleWord singleWord in list)
                {
                    if (!Properties.Settings.Default.SkipWords.Contains(singleWord.Word))
                    {
                        sb.Append(SingleWordHtml(
                            singleWord,
                            words.Count(w => w.Trim() == singleWord.Word),
                            count));
                        count++;
                    }
                }
                sb.Append("<hr style=\"clear:both;\" />");
            }
            return sb.ToString();
        }

        private static string GetXpath(string item)
        {
            var parts = item.Split(',');
            string maseches = parts[0];
            int daf = parts.Length > 1 ? Convert.ToInt32(parts[1]) : 0,
                amud = parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0;
            string xpath = "//amud[m=\"" + maseches + "\"";
            if (daf > 0)
            {
                xpath += " and d=\"" + daf.ToString() + "\"";
            }
            if (amud > 0)
            {
                xpath += " and a=\"" + amud.ToString() + "\"";
            }
            xpath += "]";
            return xpath;
        }

        private static List<SingleWord> GetDataList(int fromNumber, int toNumber)
        {
            var list = new List<SingleWord>();
            int ranker = fromNumber;
            foreach (string line in Properties.Resources.CommonWords.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Take(toNumber + 1)
                .Skip(fromNumber))
            {
                string[] splitted = line.Split(',');
                list.Add(new SingleWord
                {
                    Word = splitted[0],
                    Rank = ranker
                });
                ranker++;
            }

            return list;
        }

        public static void SkipWordByRank(int rank)
        {
            using (StringReader sr = new StringReader(Properties.Resources.CommonWords))
            {
                for(int i = 0; i < rank; i++, sr.ReadLine()) { }
                string[] splitted = sr.ReadLine().Split(',');
                Properties.Settings.Default.SkipWords.Add(splitted[0]);
            }
        }

        public static string ParseTag(string tag)
        {
            StringBuilder sb = new StringBuilder();
            var parts = tag.Split(',');
            string maseches = parts[0];
            int daf = parts.Length > 1 ? Convert.ToInt32(parts[1]) : 0,
                amud = parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0;
            sb.Append("Maseches " + maseches);
            if (daf > 0)
            {
                sb.AppendFormat(" Daf {0} ", daf);
            }
            if (amud > 0)
            {
                sb.Append(amud == 1 ? " Amud Alef" : " Amud Bais");
            }
            if (daf + amud > 0)
            {
                sb.Append(" - ");
                if (daf > 0)
                {
                    sb.AppendFormat(" דף {0}", ToNumberHeb(daf));
                }
                if (amud > 0)
                {
                    sb.Append(amud == 1 ? " עמוד א" : " עמוד ב");
                }
            }
            return sb.ToString();
        }

        private static string SingleWordHtml(SingleWord singleWord, int instances, int count)
        {
            if (instances == 0)
            {
                return "";
            }
            return string.Format(@"<div class='item'>{0:N0}. 
                <span class='word'>{1}</span>
                <br />
                <br />                
                {2:N0} instances.
                <br />
                <br />                
                Rank #{3:N0}
                <br />
                <br />" +
                "<a class=\"noPrint\" onclick=\"javascript:skipWord({3}, this);return false;\">Skip Word</a>" +
                "</div>",
                count, singleWord.Word, instances, singleWord.Rank);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }

    public struct SingleWord
    {
        public string Word;
        public int Rank;
    }

    /// <summary>
    /// Represents a single Masechta in Shas.
    /// This structure is not meant to be instantiated directly.
    /// To access the DafYomi, use <see cref="DafYomi.GetDafYomi(JewishDate)"/>
    /// </summary>
    public struct Masechta
    {
        /// <summary>
        /// The name of the masechta in English
        /// </summary>
        public string NameEnglish;

        /// <summary>
        /// The name of the masechta in Hebrew
        /// </summary>
        public string NameHebrew;

        /// <summary>
        /// The number of dappim in the current masechta
        /// </summary>
        public int Dappim;

        /// <summary>
        /// Create a new Masechta
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="heb"></param>
        /// <param name="dappim"></param>
        internal Masechta(string eng, string heb, int dappim) { NameEnglish = eng; NameHebrew = heb; Dappim = dappim; }

        public static Masechta[] MasechtaList = new Masechta[]
        {
            new Masechta ("Berachos", "ברכות", 64),
            new Masechta ("Shabbos", "שבת", 157),
            new Masechta ("Eruvin", "ערובין", 105),
            new Masechta ("Pesachim", "פסחים", 121),
            new Masechta ("Yoma", "יומא", 88),
            new Masechta ("Sukkah", "סוכה", 56),
            new Masechta ("Beitzah", "ביצה", 40),
            new Masechta ("Rosh Hashana", "ראש השנה", 35),
            new Masechta ("Taanis", "תענית", 31),
            new Masechta ("Megillah", "מגילה", 32),
            new Masechta ("Moed Katan", "מועד קטן", 29),
            new Masechta ("Chagigah", "חגיגה", 27),
            new Masechta ("Yevamos", "יבמות", 122),
            new Masechta ("Kesubos", "כתובות", 112),
            new Masechta ("Nedarim", "נדרים", 91),
            new Masechta ("Nazir", "נזיר", 66),
            new Masechta ("Sotah", "סוטה", 49),
            new Masechta ("Gitin", "גיטין", 90),
            new Masechta ("Kiddushin", "קדושין", 82),
            new Masechta ("Baba Kamma","בבא קמא",119),
            new Masechta ("Baba Metzia","בבא מציעא",119),
            new Masechta ("Baba Batra","בבא בתרא",176),
            new Masechta ("Sanhedrin","סנהדרין",113),
            new Masechta ("Makkot","מכות",24),
            new Masechta ("Shevuot","שבועות",49),
            new Masechta ("Avodah Zarah","עבודה זרה",76),
            new Masechta ("Horayot","הוריות",14),
            new Masechta ("Zevachim","זבחים",120),
            new Masechta ("Menachos", "מנחות", 110),
            new Masechta ("Chullin", "חולין", 142),
            new Masechta ("Bechoros", "בכורות", 61),
            new Masechta ("Arachin", "ערכין", 34),
            new Masechta ("Temurah", "תמורה", 34),
            new Masechta ("Kerisos", "כריתות", 28),
            new Masechta ("Meilah", "מעילה", 22),
            new Masechta ("Kinnim", "קנים", 4),
            new Masechta ("Tamid", "תמיד", 10),
            new Masechta ("Midos", "מדות", 4),
            new Masechta ("Niddah", "נדה",73)
        };
    }

}
