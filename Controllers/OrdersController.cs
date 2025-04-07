using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Nhom13.Models;

namespace Nhom13.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly IMongoCollection<Product> _productCollection;

        public OrdersController(IMongoDatabase database)
        {
            _orderCollection = database.GetCollection<Order>("orders");
            _productCollection = database.GetCollection<Product>("Products");
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderCollection.Find(_ => true).ToListAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderCollection
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,OrderDetails")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.CreatedDate = DateTime.Now;
                order.CreatedBy = User.Identity?.Name;
                order.Status = "Pending";
                await _orderCollection.InsertOneAsync(order);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderCollection
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Status,OrderDetails")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingOrder = await _orderCollection
                    .Find(o => o.Id == id)
                    .FirstOrDefaultAsync();

                if (existingOrder == null)
                {
                    return NotFound();
                }

                order.CreatedBy = existingOrder.CreatedBy;
                order.CreatedDate = existingOrder.CreatedDate;

                await _orderCollection.ReplaceOneAsync(o => o.Id == id, order);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderCollection
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _orderCollection.DeleteOneAsync(o => o.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
} 