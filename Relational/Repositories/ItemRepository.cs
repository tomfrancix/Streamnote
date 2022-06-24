using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Repositories
{
    /// <summary>
    /// Repository class for item database calls.
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public ItemRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Get all the items.
        /// </summary>                 
        /// <returns></returns>
        public IQueryable<Item> QueryAllItems()
        {
            return Context.Items
                .Include(b => b.User)
                .Include(b => b.Likes).ThenInclude(u => u.User)
                .Include(i => i.Images)
                .Include(i => i.Blocks)
                .Include(t => t.Topics.Where(t => t.ItemCount > 0)).ThenInclude(t => t.Users)
                .Where(u => u.User != null);
        }

        /// <summary>
        /// Get all the items that belong to the logged in user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IQueryable<Item> QueryUsersItems(ApplicationUser user)
        {
            return QueryAllItems().Where(i => i.User.Id == user.Id);
        }

        /// <summary>
        /// Get all the items that belong to the logged in user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IQueryable<Item> QueryPublicItems(ApplicationUser user)
        {
            return QueryAllItems().Where(i => i.IsPublic);
        }

        /// <summary>
        /// Get all the items that belong to the logged in user.
        /// </summary>                
        /// <param name="items"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public IQueryable<Item> FilterItemsByTopic(IQueryable<Item> items, string topic)
        {
            return items.Where(i => i.Topics.Any(t => t.Name == topic));
        }

        public Item Read(int id)
        {
            return Context.Items.FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<int> CreateItem(Item item)
        {
            Context.Add(item);
            await Context.SaveChangesAsync(); 
            await Context.DisposeAsync();

            return item.Id;
        }

        /// <summary>
        /// Update an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<Item> UpdateItemAsync(Item item)
        {
            try
            {
                Context.Update(item);
                await Context.SaveChangesAsync();
                await Context.DisposeAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(item.Id))
                {
                    return item;
                }
                else
                {
                    throw;
                }
            }

            return item;
        }

        /// <summary>
        /// Check if an item exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ItemExists(int id)
        {
            return Context.Items.Any(e => e.Id == id);
        }
    }
}
