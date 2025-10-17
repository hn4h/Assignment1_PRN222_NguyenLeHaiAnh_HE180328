using System;
using System.Linq;
using NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Controllers;

public class CategoryController : Controller
{
    readonly ICategoryService _cats;

    public CategoryController(ICategoryService cats) => _cats = cats;

    int? Role => HttpContext.Session.GetInt32("Role");
    bool IsStaff => Role == 1;

    void LoadLookups(short? parentId = null) =>
        ViewBag.ParentList = new SelectList(_cats.GetAll(true), "CategoryId", "CategoryName", parentId);

    void Validate(Category c)
    {
        if (string.IsNullOrWhiteSpace(c.CategoryName))
            ModelState.AddModelError(nameof(c.CategoryName), "Name is required");
        if (c.ParentCategoryId == c.CategoryId)
            ModelState.AddModelError(nameof(c.ParentCategoryId), "Parent cannot be itself");
    }

    public IActionResult Index(string? search, bool? active, int page = 1, int size = 5)
    {
        if (!IsStaff) return Unauthorized();

        var q = _cats.GetAll(active);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase));

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(q.Count() / (double)size);
        ViewBag.Search = search;
        ViewBag.Status = active;

        return View(q.Skip((page - 1) * size).Take(size));
    }

    public IActionResult Details(short id)
    {
        if (!IsStaff) return Unauthorized();
        var c = _cats.Get(id);
        return c == null ? NotFound() : View(c);
    }

    public IActionResult CreatePopup()
    {
        if (!IsStaff) return Unauthorized();
        LoadLookups();
        return PartialView("Form", new Category { IsActive = true });
    }

    public IActionResult EditPopup(short id)
    {
        if (!IsStaff) return Unauthorized();
        var c = _cats.Get(id);
        if (c == null) return NotFound();
        LoadLookups(c.ParentCategoryId);
        return PartialView("Form", c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(Category c)
    {
        if (!IsStaff) return Unauthorized();
        Validate(c);

        if (!ModelState.IsValid)
        {
            LoadLookups(c.ParentCategoryId);
            return PartialView("Form", c);
        }

        _cats.Add(c);
        TempData["Success"] = "Category created";
        return Json(new { success = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(Category c)
    {
        if (!IsStaff) return Unauthorized();
        Validate(c);

        if (!ModelState.IsValid)
        {
            LoadLookups(c.ParentCategoryId);
            return PartialView("Form", c);
        }

        var result = _cats.Update(c);
        
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            LoadLookups(c.ParentCategoryId);
            return PartialView("Form", c);
        }

        TempData["Success"] = result.Message;
        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(short id)
    {
        if (!IsStaff)
        {
            return Unauthorized();
        }

        try
        {
            var isSuccess = _cats.Delete(id);
            if (isSuccess)
            {
                TempData["Success"] = "Category deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Cannot delete this category because it is in use.";
            }
        }
        catch (Exception)
        {
            TempData["Error"] = "An error occurred while deleting the category.";
        }

        return RedirectToAction(nameof(Index));
    }
}
