using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Supplier> _suppliers;

        public ProductsController(IMongoDatabase database)
        {
            _products = database.GetCollection<Product>("products");
            _suppliers = database.GetCollection<Supplier>("suppliers");
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, string sortBy, string sortOrder)
        {
            var query = _products.Find(Builders<Product>.Filter.Empty);

            // Áp dụng tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                var filter = Builders<Product>.Filter.Or(
                    Builders<Product>.Filter.Regex(x => x.Name, new BsonRegularExpression(searchString, "i")),
                    Builders<Product>.Filter.Regex(x => x.Code, new BsonRegularExpression(searchString, "i")),
                    Builders<Product>.Filter.Regex(x => x.Description, new BsonRegularExpression(searchString, "i"))
                );
                query = _products.Find(filter);
            }

            // Sorting
            var sort = Builders<Product>.Sort;
            var sortDefinition = !string.IsNullOrEmpty(sortBy)
                ? sortOrder == "desc"
                    ? sortBy.ToLower() switch
                    {
                        "name" => sort.Descending(s => s.Name),
                        "code" => sort.Descending(s => s.Code),
                        "price" => sort.Descending(s => s.Price),
                        _ => sort.Descending(s => s.Id)
                    }
                    : sortBy.ToLower() switch
                    {
                        "name" => sort.Ascending(s => s.Name),
                        "code" => sort.Ascending(s => s.Code),
                        "price" => sort.Ascending(s => s.Price),
                        _ => sort.Ascending(s => s.Id)
                    }
                : sort.Descending(s => s.Id);

            query = query.Sort(sortDefinition);

            // Lấy danh sách sản phẩm
            var products = await query.ToListAsync();

            // Lấy thông tin nhà cung cấp cho mỗi sản phẩm
            foreach (var product in products)
            {
                var supplier = await _suppliers.Find(s => s.Id == product.SupplierId).FirstOrDefaultAsync();
                product.SupplierName = supplier?.Name;
            }

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            var supplier = await _suppliers.Find(s => s.Id == product.SupplierId).FirstOrDefaultAsync();
            product.SupplierName = supplier?.Name;

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,Unit,Price,Description,SupplierId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.IsActive = true;
                await _products.InsertOneAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Code,Unit,Price,Description,SupplierId,CreatedAt,IsActive")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _products.ReplaceOneAsync(
                    p => p.Id == id,
                    product
                );

                if (result.ModifiedCount == 0)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = await _suppliers.Find(_ => true).ToListAsync();
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            var supplier = await _suppliers.Find(s => s.Id == product.SupplierId).FirstOrDefaultAsync();
            product.SupplierName = supplier?.Name;

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _products.DeleteOneAsync(p => p.Id == id);
            
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(string id)
        {
            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product != null;
        }
    }
} 