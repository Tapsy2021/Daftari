using TrackerEnabledDbContext.Common.Interfaces;

namespace LukeApps.TrackingExtended.Interfaces
{
    public interface IExtendedContext : ITrackerContext
    {
        string GetUsername();
    }
}