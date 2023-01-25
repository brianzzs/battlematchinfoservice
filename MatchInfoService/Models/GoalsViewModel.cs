using System.ComponentModel.DataAnnotations;

namespace MatchInfoService.Models
{
    public class GoalsViewModel
    {
        [Key]
        public int GoalsGameID { get; set; }
        public string FirstGoal { get; set; }
        public int TotalGoals { get; set; }
        public DateTime GameDate { get; set; }
    }
}
