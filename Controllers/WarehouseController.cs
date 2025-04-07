using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Warehouse
        public async Task<IActionResult> Index()
        {
            var warehouseIngredients = await _context.WarehouseIngredients
                .Include(w => w.Ingredient)
                .OrderBy(w => w.Ingredient.Name)
                .ToListAsync();

            // Kiểm tra và đánh dấu các nguyên liệu có số lượng dưới mức tối thiểu
            var lowStockItems = warehouseIngredients
                .Where(w => w.Quantity <= w.MinimumQuantity)
                .ToList();

            ViewBag.LowStockItems = lowStockItems;

            return View(warehouseIngredients);
        }

        // GET: Warehouse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouseIngredient = await _context.WarehouseIngredients
                .Include(w => w.Ingredient)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseIngredient == null)
            {
                return NotFound();
            }

            return View(warehouseIngredient);
        }

        // POST: Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IngredientId,Quantity,MinimumQuantity")] WarehouseIngredient warehouseIngredient)
        {
            if (id != warehouseIngredient.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Ingredient");
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin IngredientId từ database để đảm bảo không bị thay đổi
                    var existingWarehouseIngredient = await _context.WarehouseIngredients
                        .AsNoTracking()
                        .FirstOrDefaultAsync(w => w.Id == id);

                    if (existingWarehouseIngredient == null)
                    {
                        return NotFound();
                    }

                    warehouseIngredient.IngredientId = existingWarehouseIngredient.IngredientId;
                    _context.Update(warehouseIngredient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseIngredientExists(warehouseIngredient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu ModelState không hợp lệ, load lại Ingredient để hiển thị view
            warehouseIngredient.Ingredient = await _context.Ingredients.FindAsync(warehouseIngredient.IngredientId);
            return View(warehouseIngredient);
        }

        // GET: Warehouse/AdjustStock/5
        public async Task<IActionResult> AdjustStock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouseIngredient = await _context.WarehouseIngredients
                .Include(w => w.Ingredient)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseIngredient == null)
            {
                return NotFound();
            }

            return View(warehouseIngredient);
        }

        // POST: Warehouse/AdjustStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(int id, double adjustmentQuantity, string adjustmentType)
        {
            var warehouseIngredient = await _context.WarehouseIngredients.FindAsync(id);

            if (warehouseIngredient == null)
            {
                return NotFound();
            }

            if (adjustmentQuantity <= 0)
            {
                ModelState.AddModelError("", "Số lượng điều chỉnh phải lớn hơn 0");
                return View(warehouseIngredient);
            }

            if (adjustmentType == "subtract" && adjustmentQuantity > warehouseIngredient.Quantity)
            {
                ModelState.AddModelError("", "Số lượng xuất kho không thể lớn hơn số lượng tồn kho");
                return View(warehouseIngredient);
            }

            // Cập nhật số lượng
            if (adjustmentType == "add")
            {
                warehouseIngredient.Quantity += adjustmentQuantity;
            }
            else
            {
                warehouseIngredient.Quantity -= adjustmentQuantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseIngredientExists(int id)
        {
            return _context.WarehouseIngredients.Any(e => e.Id == id);
        }
    }
} 