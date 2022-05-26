using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using streamnote.Models;

namespace streamnote.Repository.Interface
{
    public interface IItemRepository
    {
        IQueryable<Item> QueryAllItems();

        IQueryable<Item> QueryUsersItems(ApplicationUser user);

        void CreateItem(Item item);
        Item UpdateItem(Item item);
        IQueryable<Item> QueryPublicItems(ApplicationUser user);
        IQueryable<Item> FilterItemsByTopic(IQueryable<Item> items, string topic);

        Item Read(int id);
    }
}
