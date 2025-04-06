using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Supplier> _suppliers;

        public OrdersController(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>("orders");
            _products = database.GetCollection<Product>("products");
            _suppliers = database.GetCollection<Supplier>("suppliers");
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchString, string sortBy, string sortOrder)
        {
            var query = _orders.Find(Builders<Order>.Filter.Empty);

            // Áp dụng tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                var filter = Builders<Order>.Filter.Or(
                    Builders<Order>.Filter.Regex(x => x.Code, new BsonRegularExpression(searchString, "i")),
                    Builders<Order>.Filter.Regex(x => x.Status, new BsonRegularExpression(searchString, "i"))
                );
                query = _orders.Find(filter);
            }

            // Sorting
            var sort = Builders<Order>.Sort;
            var sortDefinition = !string.IsNullOrEmpty(sortBy)
                ? sortOrder == "desc"
                    ? sortBy.ToLower() switch
                    {
                        "code" => sort.Descending(s => s.Code),
                        "orderdate" => sort.Descending(s => s.OrderDate),
                        "status" => sort.Descending(s => s.Status),
                        "totalamount" => sort.Descending(s => s.TotalAmount),
                        _ => sort.Descending(s => s.Id)
                    }
                    : sortBy.ToLower() switch
                    {
                        "code" => sort.Ascending(s => s.Code),
                        "orderdate" => sort.Ascending(s => s.OrderDate),
                        "status" => sort.Ascending(s => s.Status),
                        "totalamount" => sort.Ascending(s => s.TotalAmount),
                        _ => sort.Ascending(s => s.Id)
                    }
                : sort.Descending(s => s.OrderDate);

            query = query.Sort(sortDefinition);

            var orders = await query.ToListAsync();

            // Lấy thông tin nhà cung cấp cho mỗi đơn hàng
            foreach (var order in orders)
            {
                var supplier = await _suppliers.Find(s => s.Id == order.SupplierId).FirstOrDefaultAsync();
                order.SupplierName = supplier?.Name;
            }

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            var supplier = await _suppliers.Find(s => s.Id == order.SupplierId).FirstOrDefaultAsync();
            order.SupplierName = supplier?.Name;

            foreach (var detail in order.OrderDetails)
            {
                var product = await _products.Find(p => p.Id == detail.ProductId).FirstOrDefaultAsync();
                detail.ProductName = product?.Name;
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            ViewBag.Products = await _products.Find(_ => true).ToListAsync();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.Status = "Chờ duyệt";
                order.TotalAmount = order.OrderDetails.Sum(d => d.Amount);
                
                await _orders.InsertOneAsync(order);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            ViewBag.Products = await _products.Find(_ => true).ToListAsync();
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            ViewBag.Products = await _products.Find(_ => true).ToListAsync();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                order.TotalAmount = order.OrderDetails.Sum(d => d.Amount);
                
                var result = await _orders.ReplaceOneAsync(
                    o => o.Id == id,
                    order
                );

                if (result.ModifiedCount == 0)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            ViewBag.Products = await _products.Find(_ => true).ToListAsync();
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            var supplier = await _suppliers.Find(s => s.Id == order.SupplierId).FirstOrDefaultAsync();
            order.SupplierName = supplier?.Name;

            foreach (var detail in order.OrderDetails)
            {
                var product = await _products.Find(p => p.Id == detail.ProductId).FirstOrDefaultAsync();
                detail.ProductName = product?.Name;
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _orders.DeleteOneAsync(o => o.Id == id);
            
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
} 