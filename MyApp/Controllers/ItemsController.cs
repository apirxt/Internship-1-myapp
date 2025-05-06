using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class ItemsController : Controller
    {
        private readonly MyAppContext _context;
        public ItemsController(MyAppContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            List<Item> items = await _context.Items
            .Include(i => i.SerialNumbers)
            .Include(i => i.Category)
            .Include(i => i.ItemClients) // ต้องใช้ ItemClients
              .ThenInclude(ic => ic.Client)
            .ToListAsync();
            return View(items);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.SerialNumbers)
                .Include(i => i.ItemClients)
                .ThenInclude(ic => ic.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            ViewBag.Client = new SelectList(_context.Clients, nameof(Client.Id), nameof(Client.Name));
            Item item = new Item();
            item.ItemClients.Add(new ItemClient());
            return View(item);
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,CategoryId,ItemClients")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync(); // item.Id ถูกสร้างที่นี่
                var SerialName = Request.Form["SerialName"];
                if (!string.IsNullOrWhiteSpace(SerialName))
                {
                    var serial = new SerialNumber
                    {
                        Name = SerialName,
                        ItemId = item.Id // ต้องใส่ ItemId
                    };
                    _context.SerialNumbers.Add(serial);
                    await _context.SaveChangesAsync();
                }
                var ClientName = Request.Form["ClientName"];
                if (!string.IsNullOrWhiteSpace(ClientName))
                {
                    var client = new Client
                    {
                        Name = ClientName
                    };
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                    item.ItemClients.Add(new ItemClient
                    {
                        ClientId = client.Id,
                        Remark = ""
                    });
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.Items
                .Include(i => i.SerialNumbers)
                .Include(i => i.ItemClients)
                .ThenInclude(ic => ic.Client)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name), item.CategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId")] Item item, List<string> SerialName)
        {
            if (id != item.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                var existingItem = await _context.Items
                    .Include(i => i.SerialNumbers)
                    .Include(i => i.ItemClients)
                    .ThenInclude(ic => ic.Client)
                    .FirstOrDefaultAsync(i => i.Id == id);
                if (existingItem == null)
                    return NotFound();
                existingItem.Name = item.Name;
                existingItem.Price = item.Price;
                existingItem.CategoryId = item.CategoryId;
                // SerialNumbers
                existingItem.SerialNumbers.Clear();
                foreach (var sn in SerialName.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    existingItem.SerialNumbers.Add(new SerialNumber { Name = sn });
                }
                // ClientName
                var clientName = Request.Form["ClientName"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(clientName))
                {
                    var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Name == clientName);
                    if (existingClient == null)
                    {
                        existingClient = new Client { Name = clientName };
                        _context.Clients.Add(existingClient);
                        await _context.SaveChangesAsync();
                    }
                    var itemClient = existingItem.ItemClients.FirstOrDefault();
                    if (itemClient != null)
                    {
                        itemClient.ClientId = existingClient.Id;
                    }
                    else
                    {
                        existingItem.ItemClients.Add(new ItemClient
                        {
                            ClientId = existingClient.Id,
                            Remark = ""
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.Items
                .Include(i => i.SerialNumbers)
                .Include(i => i.Category)
                .Include(i => i.ItemClients)
                .ThenInclude(ic => ic.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items
                .Include(i => i.SerialNumbers)
                .Include(i => i.ItemClients)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item != null)
            {
                _context.SerialNumbers.RemoveRange(item.SerialNumbers);
                _context.ItemClients.RemoveRange(item.ItemClients);
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}