using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user != null && user.IsActive)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                // Lưu thông tin người dùng vào session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("FullName", user.FullName);

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View();
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Account/Register
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(user);
                }

                if (string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("Password", "Mật khẩu không được để trống");
                    return View(user);
                }

                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.Role = "Employee"; // Mặc định là nhân viên

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Account/Index
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Account/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Chỉ cho phép thay đổi vai trò và trạng thái
                    existingUser.Role = user.Role;
                    existingUser.IsActive = user.IsActive;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        // GET: Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(user.Id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra mật khẩu
                    if (string.IsNullOrEmpty(user.Password))
                    {
                        ModelState.AddModelError("Password", "Mật khẩu không được để trống");
                        return View(user);
                    }

                    // Cập nhật thông tin cá nhân
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Password = user.Password;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công";
                    return RedirectToAction(nameof(Profile));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        // GET: Account/ChangePassword
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id, string currentPassword, string password, string confirmPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Password != currentPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng";
                return View(user);
            }

            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu không khớp";
                return View(user);
            }

            if (string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Mật khẩu mới không được để trống";
                return View(user);
            }

            user.Password = password;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
            return RedirectToAction(nameof(ChangePassword));
        }

        // POST: Account/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
} 