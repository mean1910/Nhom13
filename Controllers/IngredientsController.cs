using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public IngredientsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Ingredients
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;

            var ingredients = from i in _context.Ingredients.Include(i => i.Supplier)
                            select i;

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                ingredients = ingredients.Where(i => 
                    i.Name.ToLower().Contains(searchString) ||
                    i.Description.ToLower().Contains(searchString) ||
                    i.Supplier.Name.ToLower().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ingredients = ingredients.OrderByDescending(i => i.Name);
                    break;
                case "Price":
                    ingredients = ingredients.OrderBy(i => i.Price);
                    break;
                case "price_desc":
                    ingredients = ingredients.OrderByDescending(i => i.Price);
                    break;
                default:
                    ingredients = ingredients.OrderBy(i => i.Name);
                    break;
            }

            return View(await ingredients.ToListAsync());
        }

        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // GET: Ingredients/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        private string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (char c in normalizedString)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        private string GenerateFileName(string originalName)
        {
            string fileName = RemoveDiacritics(originalName).ToLower();
            fileName = Regex.Replace(fileName, @"\s+", "-");
            fileName = Regex.Replace(fileName, @"[^a-z0-9\-]", "");
            return fileName;
        }

        // POST: Ingredients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Unit,Price,Description,SupplierId")] Ingredient ingredient, IFormFile imageFile)
        {
            ModelState.Remove("ImagePath");// Loại bỏ validation cho ImagePath
            ModelState.Remove("Supplier");
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = GenerateFileName(ingredient.Name);
                    string fileExtension = Path.GetExtension(imageFile.FileName);
                    string finalFileName = $"{fileName}{fileExtension}";
                    string filePath = Path.Combine(_environment.WebRootPath, "pictures", finalFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    ingredient.ImagePath = $"/pictures/{finalFileName}";
                }

                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", ingredient.SupplierId);
            return View(ingredient);
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", ingredient.SupplierId);
            return View(ingredient);
        }

        // POST: Ingredients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Unit,Price,Description,SupplierId,ImagePath")] Ingredient ingredient, IFormFile imageFile)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Supplier");
            ModelState.Remove("ImagePath");
            ModelState.Remove("imageFile");// Loại bỏ validation cho ImagePath
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(ingredient.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_environment.WebRootPath, ingredient.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string fileName = GenerateFileName(ingredient.Name);
                        string fileExtension = Path.GetExtension(imageFile.FileName);
                        string finalFileName = $"{fileName}{fileExtension}";
                        string filePath = Path.Combine(_environment.WebRootPath, "pictures", finalFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        ingredient.ImagePath = $"/pictures/{finalFileName}";
                    }

                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.Id))
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", ingredient.SupplierId);
            return View(ingredient);
        }

        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                // Xóa file ảnh
                if (!string.IsNullOrEmpty(ingredient.ImagePath))
                {
                    string filePath = Path.Combine(_environment.WebRootPath, ingredient.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
} 