namespace LukeApps.TrackingExtended
{
    public interface IAuditDetail
    {
        AuditDetail AuditDetail { get; set; }
        bool IsDeleted { get; set; }
    }
}