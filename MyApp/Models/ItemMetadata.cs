using System.ComponentModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models
{
    public class ItemMetadata
    {
        
        public string Name { get; set; }
        
        public string Price { get; set; }
        
        public string SerialNumbers { get; set; }
        
        public string Category { get; set; }
        [DisplayName("Clients")]
        public string ItemClients { get; set; }

    }
    [ModelMetadataType(typeof(ItemMetadata))]
    public partial class Item
    {
        public async Task<bool> Create(MyAppContext dbContext)
        {
            IsDeleted = false;
            CreatedBy = "John";
            CreatedDate = DateTime.Now;
            UpdatedBy = "John";
            UpdatedDate = DateTime.Now;
            dbContext.Items.Add(this);
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Item>> GetAll(MyAppContext dbContext)
        {
            return await dbContext.Items.Where(q => q.IsDeleted != true)
                                             .Include(i => i.Category)
                                             .Include(s => s.SerialNumbers)
                                             .Include(ic => ic.ItemClients)
                                             .ThenInclude(c => c.Client)
                                             .ToListAsync();
        }
        public async Task<Item> Update(MyAppContext dbContext)
        {
            IsDeleted = false;
            UpdatedBy = "John";
            UpdatedDate = DateTime.Now;
            dbContext.Items.Update(this);
            await dbContext.SaveChangesAsync();
            return this;
        }
        public async Task<Item?> GetById(MyAppContext dbContext, int id)
        {
            Item? item = await dbContext.Items.Include(i => i.Category)
                                             .Include(s => s.SerialNumbers)
                                             .Include(ic => ic.ItemClients)
                                             .ThenInclude(c => c.Client)
                                             .FirstOrDefaultAsync(q => q.IsDeleted != true && q.Id == id);
            return item;
        }
        public async Task<bool> Delete(MyAppContext dbContext)
        {
            IsDeleted = true;
            UpdatedBy = "John";
            UpdatedDate = DateTime.Now;
            dbContext.Items.Update(this);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
