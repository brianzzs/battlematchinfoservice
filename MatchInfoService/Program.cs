using FirstGoalBets.Data;
using Microsoft.EntityFrameworkCore;
using MatchInfoService.Controller;
using MatchInfoService.Migrations;
using MatchInfoService.Models;
using System.Linq;
using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

using var backLogConnection = new FirstGoalBetsDBContext();


HttpClient client = new HttpClient();
var startpage = 1;

while (startpage <= 40 )
{
    using var context = new FirstGoalBetsDBContext();
    string url = $"https://betsapi.com/le/22614/Esoccer-Battle--8-mins-play/p.{startpage}";
    var response = InfoController.CallUrl(url, client).Result;
    var pageMatches = InfoController.ParseHtml(response);
    var select = from match in context.Match
                 select match.GameID;

    foreach (var match in pageMatches)
    {
        var connection = new FirstGoalBetsDBContext();

        if (!select.Contains(match.GameID))
        {
            Console.WriteLine($"Adicionando jogo {match.UrlMatch} ao banco de dados");
            connection.Match.Add(match);
        }
        else
        {
            Console.WriteLine("Jogo já existe!");
            continue;
        }
        connection.SaveChanges();
    }
    context.SaveChanges();
    startpage++;
    Console.WriteLine($"Pagina: {startpage} ");

}



var backlog = from match in backLogConnection.Match
              join goals in backLogConnection.Goals on match.GameID equals goals.GoalsGameID into g  
              from goals in g.DefaultIfEmpty()
              where goals.GoalsGameID == null
              select match;

var finalBackLog = backlog.ToList();


HttpClient goalsClient = new HttpClient();

foreach (var match in finalBackLog)
{
    Console.WriteLine($"Acessando URL {match.UrlMatch}");
    var response = InfoController.CallUrl(match.UrlMatch, goalsClient).Result;
    var firstGoalPlayer = InfoController.ParseFirstGoalHtml(response);
    var date = InfoController.ParseData(response);
    try
    {
        var goals = new GoalsViewModel();
        {
            var connection = new FirstGoalBetsDBContext();
            goals.GoalsGameID = match.GameID;
            goals.TotalGoals = match.TotalGoals;
            goals.FirstGoal = firstGoalPlayer;
            goals.GameDate = date;
            connection.Goals.Add(goals);
            connection.SaveChanges();
            connection.Dispose();
        }
    }
    catch
    {
        var connectionError = new FirstGoalBetsDBContext();
        Console.WriteLine($"Erro ao pegar o primeiro gol - Jogo {match.UrlMatch}");
        var goalsErro = new GoalsViewModel();
        {
            goalsErro.GoalsGameID = match.GameID;
            goalsErro.TotalGoals = match.TotalGoals;
            goalsErro.FirstGoal = "N";
            connectionError.Goals.Add(goalsErro);
            connectionError.SaveChanges();
            connectionError.Dispose();
        }
    }
    Console.WriteLine("Salvando no banco");
}



