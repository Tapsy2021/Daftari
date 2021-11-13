using LukeApps.BugsTracker.Models;
using LukeApps.DAL;
using LukeApps.TrackingExtended;
using System;
using System.Web;

namespace LukeApps.BugsTracker
{
    public class BugsHandler : IDisposable
    {
        public BugsHandler(Exception ex, HttpContextBase cb)
        {
            exception = ex;
            filePath = cb.Request.Path;
            url = cb.Request.Url.OriginalString;
            reportedBy = cb.User.Identity.Name;
            errorCode = cb.Response.StatusCode;
        }

        public BugsHandler(Exception ex, string reportedBy, int errorCode = 0, string filePath = null, string url = null)
        {
            exception = ex;
            this.filePath = filePath;
            this.url = url;
            this.reportedBy = reportedBy;
            this.errorCode = errorCode;
        }

        private readonly Exception exception;
        private readonly string filePath;
        private readonly string url;
        private readonly string reportedBy;
        private readonly int errorCode;
        private BugsContext db = new BugsContext();

        public void Log_Error(string comments)
        {
            saveBug(exception, comments);
        }

        public void Log_Error()
        {
            saveBug(exception);
        }

        private int saveBug(Exception ex, string comments = null)
        {
            Bug bug = new Bug
            {
                Category = ex.GetType().ToString(),
                Description = ex.Message,
                FilePath = filePath,
                Source = ex.Source,
                StackTrace = ex.StackTrace?.ToString(),
                Url = url,
                ReportedBy = reportedBy,
                ErrorCode = errorCode,
                AuditDetail = new AuditDetail
                {
                    CreatedEntryUserID = reportedBy
                }
            };

            if (comments != null)
                bug.Comments = comments;

            db.Bugs.Add(bug);
            db.SaveChanges();
            if (ex.InnerException != null)
                saveBug(ex.InnerException);
            return 1;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}