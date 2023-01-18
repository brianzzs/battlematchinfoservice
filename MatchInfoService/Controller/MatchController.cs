using FirstGoalBets.Data;
using HtmlAgilityPack;
using MatchInfoService.Migrations;
using MatchInfoService.Models;
using System.Globalization;
using System.Numerics;

namespace MatchInfoService.Controller
{
    public class InfoController
    {

        public static async Task<string> CallUrl(string fullUrl, HttpClient client)
        {
            //Thread.Sleep(1000)
            Console.WriteLine("Iniciando requisição");
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }


        public static List<MatchViewModel> ParseHtml(string html)
        {
            var pageMatches = new List<MatchViewModel>();
            Console.WriteLine("Entrou no metodo parse");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            List<MatchViewModel> listMatches = new List<MatchViewModel>();

            var matches = htmlDoc.DocumentNode.Descendants("table").ToList()[0].Descendants("tr").ToList();
            foreach (var item in matches)
            {
                var matchInfo = item.InnerText.Split('\n').Select(e => e.Trim()).Select(x => x.Replace("\"", "")).Where(z => !z.Equals("")).ToList().Where(v => !v.Equals("-")).ToList();
                if (matchInfo.Count == 3)
                {
                    var match = new MatchViewModel();
                    {
                        try
                        {
                            var url = "https://betsapi.com";
                            var newUrl = item.InnerHtml.Split("\n").ToList().Where(e => e.Contains("<a")).Where(z => z.Contains("/r")).FirstOrDefault().Trim().Split("<a href=").Where(x => !x.Equals("")).FirstOrDefault().Split("<").ToList().FirstOrDefault().Split(">").FirstOrDefault().ToString().Replace("\"", "");
                            url = url + newUrl;
                            var day = item.Descendants("td").ToList()[0].OuterHtml.Split("data-dt=").ToList()[1].Split("T").ToList()[0].Trim('"');
                            var hour = item.Descendants("td").ToList()[0].OuterHtml.Split("data-dt=").ToList()[1].Split("T").ToList()[1].Substring(0, 8);
                            var finalDate = day + " " + hour;
                            var matchDate = DateTime.Parse(finalDate);
                            matchDate = DateTime.SpecifyKind(matchDate, DateTimeKind.Utc);
                            var kind = matchDate.Kind;
                            DateTime dt = matchDate.ToLocalTime();
                            match.GameID = Int32.Parse(item.Descendants("a").ToList()[2].OuterHtml.Split("/r/")[1].Split("/").FirstOrDefault());
                            match.Date = dt;

                            var homeplayer = matchInfo[1].Split(" v ").FirstOrDefault().Split("(").ToList().Select(z => z.Split(")")).ToList()[1].FirstOrDefault();
                            match.HomePlayerName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(homeplayer);
                            match.HomeTeamName = matchInfo[1].Split(" v ").FirstOrDefault().Split("(").ToList().Select(z => z.Split(")")).ToList().FirstOrDefault()[0];
                            match.HomeScore = Int32.Parse(matchInfo[2].Split("-")[0]);

                            var awayplayer = matchInfo[1].Split(" v ")[1].Split("(").ToList().Select(z => z.Split(")")).ToList()[1].FirstOrDefault();
                            match.AwayPlayerName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(awayplayer);
                            match.AwayTeamName = matchInfo[1].Split(" v ")[1].Split("(").ToList().Select(z => z.Split(")")).ToList().FirstOrDefault()[0];
                            match.AwayScore = Int32.Parse(matchInfo[2].Split("-")[1]);
                            match.TotalGoals = match.HomeScore + match.AwayScore;
                            match.UrlMatch = url;
                            pageMatches.Add(match);
                        }
                        catch
                        { Console.WriteLine("Deu Erro no MatchInfo"); }
                    }
                }
                Console.WriteLine($"Trocando de Item");
            }
            return pageMatches;
        }

        public static string ParseFirstGoalHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(html);
                var firstGoalPlayer = "N";
                try
                {

                    var firstGoal = htmlDoc.DocumentNode.Descendants("ul").ToList()[1].Descendants("li").ToList().Where(e => e.InnerText.Contains("Goal")).ToList();
                    if (firstGoal.Count > 0)
                    {
                        firstGoalPlayer = firstGoal.FirstOrDefault().InnerText.Split('\n').Select(e => e.Trim()).FirstOrDefault().Split("-").Where(e => e.Contains("(")).FirstOrDefault().Split("(")[2].Split(")").FirstOrDefault();
                        firstGoalPlayer = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(firstGoalPlayer);
                        return firstGoalPlayer;
                    }
                }
                catch
                {
                    Console.WriteLine("Deu Erro em pegar o primeiro gol");
                }
                return firstGoalPlayer;
            }
            catch { Console.WriteLine("Erro no HTML first goal"); return null; }
        }

        public static DateTime ParseData(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var date = htmlDoc.DocumentNode.Descendants("h1").ToList().FirstOrDefault().Descendants("span").ToList()[2].InnerText + ":00";
            var finalDate = DateTime.Parse(date);
            finalDate = DateTime.SpecifyKind(finalDate, DateTimeKind.Utc);
            var kind = finalDate.Kind;
            DateTime dt = finalDate.ToLocalTime();
            return dt;
        }
    }

}