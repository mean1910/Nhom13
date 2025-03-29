using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(SupplierSearchViewModel searchModel)
        {
            var query = _context.Suppliers.AsQueryable();

            // Áp dụng tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchModel.SearchString))
            {
                query = query.Where(s => 
                    s.Name.Contains(searchModel.SearchString) ||
                    s.Phone.Contains(searchModel.SearchString) ||
                    s.Email.Contains(searchModel.SearchString) ||
                    s.Address.Contains(searchModel.SearchString)
                );
            }

            // Áp dụng lọc theo trạng thái
            if (searchModel.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == searchModel.IsActive.Value);
            }

            // Áp dụng sắp xếp
            if (!string.IsNullOrEmpty(searchModel.SortBy))
            {
                query = searchModel.SortBy.ToLower() switch
                {
                    "name" => searchModel.SortOrder == "desc" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                    "phone" => searchModel.SortOrder == "desc" ? query.OrderByDescending(s => s.Phone) : query.OrderBy(s => s.Phone),
                    "email" => searchModel.SortOrder == "desc" ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
                    "createdat" => searchModel.SortOrder == "desc" ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                    _ => query.OrderByDescending(s => s.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(s => s.CreatedAt);
            }

            // Lấy danh sách nhà cung cấp
            var suppliers = await query.ToListAsync();

            // Gán lại model tìm kiếm để giữ lại các giá trị đã chọn
            ViewBag.SearchModel = searchModel;

            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Phone,Email,Note,IsActive")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
    }
} 