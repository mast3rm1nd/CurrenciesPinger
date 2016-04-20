using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace CurrenciesPinger
{
    class Program
    {
        static void Main(string[] args)
        {
            //var t = DateTime.Now.ToString("dd.MM.yyyy");
            if(args.Length == 0)
            {
                var help = "";

                help += "Использование: pind [количество] обозначение_денежной_единицы" + Environment.NewLine + Environment.NewLine;
                help += "обозначение_денежной_единицы - обозначение, используемое на сайте ЦБ РФ, например USD или EUR для доллара и евро соответственно." + Environment.NewLine;

                Console.Write(help);

                return;
            }

            var currency = "";
            var amount = -1.0;

            if (args.Length == 2)
            {
                currency = args[1].ToUpper();

                if (!double.TryParse(args[0].Replace('.', ','), out amount)) return;
            }
            else if (args.Length == 1)
                currency = args[0].ToUpper();
            else
                return;


            var html = "";
            using(var browser = new WebClient())
            {
                Stream data = browser.OpenRead("http://www.cbr.ru/currency_base/daily.aspx?date_req=" + DateTime.Now.ToString("dd.MM.yyyy"));
                StreamReader reader = new StreamReader(data);
                html = reader.ReadToEnd();
            }

            var regex = "<td>" + currency + "</td>\r\n<td>(?<Amount>\\d+?)</td>\r\n<td>(?<Name>.+?)</td>\r\n<td>(?<Cost>.+?)</td></tr>";

            if(!Regex.IsMatch(html, regex))
            {
                Console.Write("Нет информации о единице " + currency + Environment.NewLine);

                return;
            }

            var match = Regex.Match(html, regex);

            //var t = match.Groups["Amount"].Value.ToString();
            //var t2 = match.Groups["Name"].Value;
            //var t3 = match.Groups["Cost"].Value;

            var amountSite = double.Parse(match.Groups["Amount"].Value);
            var name = match.Groups["Name"].Value.ToString();
            var cost = double.Parse(match.Groups["Cost"].Value);

            var costOfOneUnit = cost / amountSite;

            //var format = "{0} (\"{3}\") {1} = {2:F2} руб.";
            var format = "{0} \"{3}\" = {2:F2} руб.";

            if(amount == -1.0)
                Console.WriteLine(string.Format(format, amountSite, currency, cost, name));
            else
                Console.WriteLine(string.Format(format, amount, currency, costOfOneUnit * amount, name));            
        }
    }
}
