using Microsoft.AspNet.SignalR;

namespace Daftari.Hubs
{
    public class Pike13Hub : Hub
    {
        public void SendNotification(string tableNames)
        {
            // get the current user name
            string userName = Context.User.Identity.Name;
            if (userName == null || userName == "") userName = "Someone";
            Clients.Others.receiveNotification(userName, tableNames);
        }
    }
}