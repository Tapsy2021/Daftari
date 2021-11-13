using Daftari.SMSHandling.Models;
using LukeApps.TrackingExtended;
using System.Collections.Generic;

namespace Daftari.SMSHandling.DAL
{
    internal class SMSInitializer : System.Data.Entity.CreateDatabaseIfNotExists<SMSContext>
    {
        protected override void Seed(SMSContext context)
        {
            var smsConfig = new List<SMSConfig>
            {
                new SMSConfig{UserID="mctswimsch", password="Alhabsy@2017", AuditDetail = new AuditDetail { CreatedEntryUserID = "Dev" }},
            };

            smsConfig.ForEach(s => context.SMSConfigs.Add(s));
            context.SaveChanges();
        }
    }
}