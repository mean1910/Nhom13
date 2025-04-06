using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using Nhom13.Models;

namespace Nhom13.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly IMongoCollection<Supplier> _suppliers;

        public SuppliersController(IMongoDatabase database)
        {
            _suppliers = database.GetCollection<Supplier>("suppliers");
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(string searchString, string sortBy, string sortOrder)
        {
            var query = _suppliers.Find(Builders<Supplier>.Filter.Empty);

            // Áp dụng tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                var filter = Builders<Supplier>.Filter.Or(
                    Builders<Supplier>.Filter.Regex(x => x.Name, new BsonRegularExpression(searchString, "i")),
                    Builders<Supplier>.Filter.Regex(x => x.Address, new BsonRegularExpression(searchString, "i")),
                    Builders<Supplier>.Filter.Regex(x => x.Phone, new BsonRegularExpression(searchString, "i")),
                    Builders<Supplier>.Filter.Regex(x => x.Email, new BsonRegularExpression(searchString, "i"))
                );
                query = _suppliers.Find(filter);
            }

            // Áp dụng lọc theo trạng thái
            //if (searchModel.IsActive.HasValue)
            //{
            //    query = query.Where(s => s.IsActive == searchModel.IsActive.Value);
            //}

            // Sorting
            var sort = Builders<Supplier>.Sort;
            var sortDefinition = !string.IsNullOrEmpty(sortBy)
                ? sortOrder == "desc"
                    ? sortBy.ToLower() switch
                    {
                        "name" => sort.Descending(s => s.Name),
                        "address" => sort.Descending(s => s.Address),
                        "phone" => sort.Descending(s => s.Phone),
                        "email" => sort.Descending(s => s.Email),
                        _ => sort.Descending(s => s.Id)
                    }
                    : sortBy.ToLower() switch
                    {
                        "name" => sort.Ascending(s => s.Name),
                        "address" => sort.Ascending(s => s.Address),
                        "phone" => sort.Ascending(s => s.Phone),
                        "email" => sort.Ascending(s => s.Email),
                        _ => sort.Ascending(s => s.Id)
                    }
                : sort.Descending(s => s.Id);

            query = query.Sort(sortDefinition);

            // Lấy danh sách nhà cung cấp
            var suppliers = await query.ToListAsync();

            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var supplier = await _suppliers
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync();
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,Phone,Email,Note")] Supplier supplier)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    supplier.CreatedAt = DateTime.Now;
                    supplier.IsActive = true;
                    await _suppliers.InsertOneAsync(supplier);
                    return RedirectToAction(nameof(Index));
                }
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving supplier: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu nhà cung cấp. Vui lòng thử lại.");
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _suppliers
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync();
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Address,Phone,Email,Note,CreatedAt,IsActive")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _suppliers.ReplaceOneAsync(
                    s => s.Id == id,
                    supplier
                );

                if (result.ModifiedCount == 0)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _suppliers
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync();
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _suppliers.DeleteOneAsync(s => s.Id == id);
            
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SupplierExists(string id)
        {
            var supplier = await _suppliers.Find(s => s.Id == id).FirstOrDefaultAsync();
            return supplier != null;
        }
    }
} 