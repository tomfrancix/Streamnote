using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using streamnote.Data;

namespace streamnote.Messenger
{
    /// <summary>
    /// The SignalR messenger hub class (invoked by javascript see:chat.js).
    /// </summary>
    public class SignalRMessenger : Hub
    {
        private readonly ApplicationDbContext DbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dbContext"></param>
        public SignalRMessenger(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// Send a message to a user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string username, string message)
        {
            var user = DbContext.Users.FirstOrDefault(u => u.UserName == username);

            if (user != null) 
                await Clients.User(user.Id).SendAsync("ReceiveMessage", message);

            if (Context.UserIdentifier != null)
                await Clients.User(Context.UserIdentifier).SendAsync("ReceiveMessage", message);
        }

        /// <summary>
        /// Send a like notification to a user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task SendLikeNotification(string username, int itemId)
        {
            var user = DbContext.Users.FirstOrDefault(u => u.UserName == username);

            var loggedInUser = await DbContext.Users.FindAsync(Context.UserIdentifier);

            if (user != null)
                await Clients.User(user.Id).SendAsync("ReceiveLike", $"<div class='notification' >{loggedInUser.FirstName} liked your <a href='Item/Details/{itemId}'>post</a></span>");
        }
    }
}