using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using streamnote.Data;

namespace streamnote.Messenger
{
    public class SignalRMessenger : Hub
    {
        private readonly ApplicationDbContext DbContext;

        public SignalRMessenger(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task SendMessage(string username, string message)
        {
            var user = DbContext.Users.FirstOrDefault(u => u.UserName == username);

            if (user != null) 
                await Clients.User(user.Id).SendAsync("ReceiveMessage", message);

            if (Context.UserIdentifier != null)
                await Clients.User(Context.UserIdentifier).SendAsync("ReceiveMessage", message);
        }

        public async Task SendLikeNotification(string username, int itemId)
        {
            var user = DbContext.Users.FirstOrDefault(u => u.UserName == username);

            var loggedInUser = await DbContext.Users.FindAsync(Context.UserIdentifier);

            if (user != null)
                await Clients.User(user.Id).SendAsync("ReceiveLike", $"<div class='notification' >{loggedInUser.FirstName} liked your <a href='Item/Details/{itemId}'>post</a></span>");
        }
    }
}