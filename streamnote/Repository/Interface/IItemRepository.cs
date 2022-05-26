using System.Linq;
using System.Threading.Tasks;
using streamnote.Models;

namespace streamnote.Repository.Interface
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
