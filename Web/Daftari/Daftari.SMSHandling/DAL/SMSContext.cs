using Daftari.SMSHandling.Models;
using System.Data.Entity;
using TrackerEnabledDbContext;

namespace Daftari.SMSHandling.DAL
{
    internal class SMSContext : TrackerContext
    {
        public SMSContext() : base("SMSContext")
        {
            Database.SetInitializer(new SMSInitializer());
        }

        public DbSet<SMSLog> SMSLogs { get; set; }
        public DbSet<SMSConfig> SMSConfigs { get; set; }
    }
}