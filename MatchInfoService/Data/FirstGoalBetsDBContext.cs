using MatchInfoService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FirstGoalBets.Data
{
    public class FirstGoalBetsDBContext : DbContext
    {

        public DbSet<MatchViewModel> Match { get; set; }
        public DbSet<GoalsViewModel> Goals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost; initial catalog=FirstGoal; uid=root;pwd=#!", new MySqlServerVersion(new Version(8, 0, 11)));
        }
    }
}
