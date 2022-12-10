using System.ComponentModel.DataAnnotations;

namespace MatchInfoService.Models
{
    public class MatchViewModel
    {
        [Key]
        public int GameID { get; set; }
        public DateTime Date { get; set; }
        public string HomePlayerName { get; set; }
        public string HomeTeamName { get; set; }
        public int HomeScore { get; set; }
        public string AwayPlayerName { get; set; }
        public string AwayTeamName { get; set; }
        public int AwayScore { get; set; }
        public int TotalGoals { get; set; }
        public string UrlMatch { get; set; } 
    }
}
