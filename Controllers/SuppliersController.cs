using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(ApplicationDbContext context, ILogger<SuppliersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(SupplierSearchViewModel searchModel)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.SearchString))
            {
                query = query.Where(s => s.Name.Contains(searchModel.SearchString));
            }

            var suppliers = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
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
        public async Task<IActionResult> Create([Bind("Name,Phone,Email,Address,Note")] Supplier supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra xem nhà cung cấp đã tồn tại chưa
                    var existingSupplier = await _context.Suppliers
                        .FirstOrDefaultAsync(s => s.Name == supplier.Name || s.Phone == supplier.Phone);
                    
                    if (existingSupplier != null)
                    {
                        ModelState.AddModelError("", "Nhà cung cấp với tên hoặc số điện thoại này đã tồn tại!");
                        return View(supplier);
                    }

                    supplier.CreatedAt = DateTime.Now;
                    supplier.IsActive = true;

                    _context.Add(supplier);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
                    _logger.LogInformation($"Đã thêm nhà cung cấp mới: {supplier.Name}");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Model không hợp lệ khi thêm nhà cung cấp");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm nhà cung cấp: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhà cung cấp. Vui lòng thử lại!");
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Email,Address,Note,IsActive")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSupplier = await _context.Suppliers.FindAsync(id);
                    if (existingSupplier == null)
                    {
                        return NotFound();
                    }

                    existingSupplier.Name = supplier.Name;
                    existingSupplier.Phone = supplier.Phone;
                    existingSupplier.Email = supplier.Email;
                    existingSupplier.Address = supplier.Address;
                    existingSupplier.Note = supplier.Note;
                    existingSupplier.IsActive = supplier.IsActive;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật nhà cung cấp thành công!";
                    return RedirectToAction(nameof(Index));
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