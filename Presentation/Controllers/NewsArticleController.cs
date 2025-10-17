using System;
using System.Linq;
using NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Controllers;

public class NewsArticleController : Controller
{
    readonly INewsArticleService _news;
    readonly ICategoryService _cats;
    readonly ITagService _tags;

    public NewsArticleController(INewsArticleService news, ICategoryService cats, ITagService tags)
    {
        _news = news; _cats = cats; _tags = tags;
    }

    int? SessionId => HttpContext.Session.GetInt32("AccountId");
    bool IsStaff => HttpContext.Session.GetInt32("Role") == 1;
    bool IsOwner(NewsArticle n) => IsStaff && n.CreatedById == SessionId;

    /* ---------- helpers ---------- */
    void LoadLookups(short? cat = null, IEnumerable<int>? tagIds = null)
    {
        ViewBag.CategoryList = new SelectList(_cats.GetAll(true), "CategoryId", "CategoryName", cat);
        ViewBag.Tags = _tags.GetAll();
        ViewBag.SelectedTags = tagIds ?? Enumerable.Empty<int>();
    }

    void Validate(NewsArticle n, IEnumerable<int>? tags)
    {
        if (n.CategoryId == 0) ModelState.AddModelError(nameof(n.CategoryId), "Choose category");
        if (tags == null || !tags.Any()) ModelState.AddModelError("Tags", "Select at least one tag");
        ModelState.Remove(nameof(n.Category));
        ModelState.Remove(nameof(n.CreatedBy));
        ModelState.Remove(nameof(n.UpdatedBy));
        ModelState.Remove(nameof(n.NewsArticleId));
    }


    public IActionResult Index(string? search, int? tagId, int page = 1, int size = 5)
    {
        var list = _news.GetAll(true);
        
        // Filter by search keyword
        if (!string.IsNullOrWhiteSpace(search))
            list = list.Where(x => x.NewsTitle
                .Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));

        // Filter by tag ID
        if (tagId.HasValue)
            list = list.Where(x => x.Tags.Any(t => t.TagId == tagId.Value));

        // Load all tags for the dropdown filter
        ViewBag.AllTags = _tags.GetAll().OrderBy(t => t.TagName);

        // Pass filter info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(list.Count() / (double)size);
        ViewBag.Search = search;
        ViewBag.TagId = tagId;
        
        // Get tag name if filtering by tag
        if (tagId.HasValue)
        {
            var tag = _tags.Get(tagId.Value);
            ViewBag.TagName = tag?.TagName;
        }

        return View(list.Skip((page - 1) * size).Take(size));
    }

    public IActionResult Details(string id)
    {
        var n = _news.Get(id);
        if (n == null) return NotFound();

        // Get related articles (same category or shared tags)
        var relatedArticles = _news.GetRelatedArticles(id, n.CategoryId, 3);
        ViewBag.RelatedArticles = relatedArticles;

        return View(n);
    }


    public IActionResult MyArticles()
    {
        if (!IsStaff) return Unauthorized();
        var list = _news.GetByStaff((short)SessionId!);
        return View(list);
    }

    public IActionResult CreatePopup()
    {
        if (!IsStaff) return Unauthorized();
        LoadLookups();
        return PartialView("form", new NewsArticle { NewsStatus = true });
    }

    public IActionResult EditPopup(string id)
    {
        if (!IsStaff) return Unauthorized();
        var n = _news.Get(id);
        if (n == null) return NotFound();
        LoadLookups(n.CategoryId, n.Tags.Select(t => t.TagId));
        return PartialView("form", n);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(NewsArticle n, IEnumerable<int> tags)
    {
        if (!IsStaff) return Unauthorized();

        n.NewsArticleId = _news.GetNextId();
        n.CreatedById = (short)SessionId!;
        Console.WriteLine(SessionId);
        n.CreatedDate = DateTime.Now;
        Validate(n, tags);

        if (!ModelState.IsValid)
        {
            LoadLookups(n.CategoryId, tags);
            return PartialView("form", n);
        }

        _news.Add(n, tags);
        return Json(new { success = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(NewsArticle n, IEnumerable<int> tags)
    {
        if (!IsStaff) return Unauthorized();
        var current = _news.Get(n.NewsArticleId);
        if (current == null) return NotFound();

        n.UpdatedById = (short)SessionId!;
        n.ModifiedDate = DateTime.Now;
        Validate(n, tags);

        if (!ModelState.IsValid)
        {
            LoadLookups(n.CategoryId, tags);
            return PartialView("form", n);
        }

        _news.Update(n, tags);
        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult Delete(string id)
    {
        var n = _news.Get(id);
        if (n == null || !IsOwner(n)) return Unauthorized();

        _news.Delete(id);
        TempData["Success"] = "News deleted successfully!";
        return RedirectToAction(nameof(MyArticles));
    }

    //public IActionResult Delete(string id)
    //{
    //    var n = _news.Get(id);
    //    return n == null || !IsOwner(n) ? Unauthorized() : View(n);
    //}

    //[HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    //public IActionResult Delete(string id, bool sure = false)
    //{
    //    var n = _news.Get(id);
    //    if (n == null || !IsOwner(n)) return Unauthorized();

    //    if (!sure)
    //    {
    //        TempData["Error"] = "Confirm delete";
    //        return RedirectToAction(nameof(Delete), new { id });
    //    }

    //    _news.Delete(id);
    //    TempData["Success"] = "News deleted";
    //    return RedirectToAction(nameof(MyArticles));
    //}
}
