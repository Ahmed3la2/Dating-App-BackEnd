using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.signalR
{
    public class PresenceTracker
    {
        private static Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> {connectionId});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
              OnlineUsers[username].Remove(connectionId);

              if(OnlineUsers[username].Count == 0)
              {
                 OnlineUsers.Remove(username);
              }

            }

            return Task.CompletedTask;
        }

        public Task<List<string>> GetUsersOnline()
        {
            List<string> usersOnline = new List<string>();
            foreach(var username in OnlineUsers.Keys)
            {
                usersOnline.Add(username);
            }

            return Task.FromResult(usersOnline);
        }

    }
}
