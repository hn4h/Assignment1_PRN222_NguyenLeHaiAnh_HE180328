using System;
using System.Linq;
using NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        readonly IAccountService _acc;
        public AccountController(IAccountService acc) => _acc = acc;


        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var a = _acc.Login(email, password);
            if (a == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            HttpContext.Session.SetInt32("AccountId", a.AccountId);
            HttpContext.Session.SetInt32("Role", a.AccountRole.GetValueOrDefault());
            HttpContext.Session.SetString("Name", a.AccountName);
            if (a.AccountRole == 0) // admin
                return RedirectToAction("Index", "Account");
            return RedirectToAction("Index", "NewsArticle");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        bool IsAdmin() => HttpContext.Session.GetInt32("Role") is int r && r != 1 && r != 2;

        public IActionResult Index(string? search, int? roleFilter, int page = 1, int size = 5)
        {
            if (!IsAdmin()) return Unauthorized();

            // Lấy toàn bộ account
            var q = _acc.GetAll();

            // 🔍 Tìm kiếm theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(x =>
                    x.AccountName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.AccountEmail.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // 🎯 Lọc theo Role (1 = Staff, 2 = Lecturer)
            if (roleFilter.HasValue && roleFilter > 0)
            {
                q = q.Where(x => x.AccountRole == roleFilter.Value);
            }

            // 📄 Tính tổng số trang
            var total = q.Count();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / size);

            // 🧭 Lưu lại giá trị search & filter để hiển thị trong view
            ViewBag.Search = search;
            ViewBag.RoleFilter = roleFilter;

            // 📦 Phân trang kết quả
            var data = q
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return View(data);
        }


        public IActionResult CreatePopup()
        {
            if (!IsAdmin()) return Unauthorized();
            return PartialView("Form", new SystemAccount());
        }

        public IActionResult EditPopup(short id)
        {
            if (!IsAdmin()) return Unauthorized();
            var a = _acc.Get(id);
            return a == null ? NotFound() : PartialView("Form", a);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(SystemAccount a, string password)
        {
            if (!IsAdmin()) return Unauthorized();
            Validate(a, true, password);

            if (!ModelState.IsValid)
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? PartialView("Form", a)
                    : View(a);

            _acc.Add(a, password);
            TempData["Success"] = "Account created.";
            return Json(new { success = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(SystemAccount a)
        {
            if (!IsAdmin()) return Unauthorized();
            Validate(a, false, null);

            if (!ModelState.IsValid)
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? PartialView("Form", a)
                    : View(a);

            _acc.Update(a);
            TempData["Success"] = "Account updated.";
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Delete(short id)
        {
            if (!IsAdmin()) return Unauthorized();

            var result = _acc.Delete(id);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var id = HttpContext.Session.GetInt32("AccountId");
            if (id == null) return RedirectToAction("Login");

            var acc = _acc.Get((short)id);
            if (acc == null) return NotFound();

            return View(acc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(SystemAccount acc, string? newPassword)
        {
            var id = HttpContext.Session.GetInt32("AccountId");
            if (id == null || acc.AccountId != id) return Unauthorized();

            var existing = _acc.Get(acc.AccountId);
            if (existing == null) return NotFound();

            if (string.IsNullOrWhiteSpace(acc.AccountName))
                ModelState.AddModelError(nameof(acc.AccountName), "Name is required");

            if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < 6)
            {
                ViewBag.PasswordError = "Password must be at least 6 characters";
                return View(existing);
            }

            if (!ModelState.IsValid) return View(existing);

            existing.AccountName = acc.AccountName;
            _acc.Update(existing, newPassword);

            TempData["Success"] = "Profile updated.";
            return RedirectToAction(nameof(Profile));
        }



        void Validate(SystemAccount a, bool isCreate, string? pwd)
        {
            if (string.IsNullOrWhiteSpace(a.AccountName))
                ModelState.AddModelError(nameof(a.AccountName), "Name required");

            if (!new[] { 1, 2 }.Contains(a.AccountRole ?? 0))
                ModelState.AddModelError(nameof(a.AccountRole), "Role must be 1 (Staff) or 2 (Lecturer)");

            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(pwd))
                    ModelState.AddModelError(nameof(a.AccountPassword), "Password required");
                if (_acc.ExistsEmail(a.AccountEmail))
                    ModelState.AddModelError(nameof(a.AccountEmail), "Email existed");
            }
        }
    }
}
