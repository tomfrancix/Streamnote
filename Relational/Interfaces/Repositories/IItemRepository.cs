using System.Linq;
using System.Threading.Tasks;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Interfaces.Repositories
{
    public interface IItemRepository
    {
        IQueryable<Item> QueryAllItems();

        IQueryable<Item> QueryUsersItems(ApplicationUser user);

        Task CreateItem(Item item);
        Task<Item> UpdateItemAsync(Item item);
        IQueryable<Item> QueryPublicItems(ApplicationUser user);
        IQueryable<Item> FilterItemsByTopic(IQueryable<Item> items, string topic);

        Item Read(int id);
    }
}
