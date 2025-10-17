using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;

namespace Presentation.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tags;

        public TagController(ITagService tags)
        {
            _tags = tags;
        }

        int? Role => HttpContext.Session.GetInt32("Role");
        bool IsStaff => Role == 1;

        void Validate(Tag t)
        {
            if (string.IsNullOrWhiteSpace(t.TagName))
                ModelState.AddModelError(nameof(t.TagName), "Tag name is required");
        }

        public IActionResult Index(string? search, int page = 1, int size = 5)
        {
            if (!IsStaff) return Unauthorized();

            var q = _tags.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.TagName.Contains(search, StringComparison.OrdinalIgnoreCase));

            q = q.OrderByDescending(x => x.TagName);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(q.Count() / (double)size);
            ViewBag.Search = search;

            return View(q.Skip((page - 1) * size).Take(size));
        }


        public IActionResult CreatePopup()
        {
            if (!IsStaff) return Unauthorized();
            return PartialView("Form", new Tag());
        }

        public IActionResult EditPopup(int id)
        {
            if (!IsStaff) return Unauthorized();
            //Console.WriteLine("id:" + id);
            var t = _tags.Get(id);
            Console.WriteLine(t.TagId);
            if (t == null) return NotFound();
            return PartialView("Form", t);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag t)
        {
            if (!IsStaff) return Unauthorized();

            Validate(t);
            if (!ModelState.IsValid)
                return PartialView("Form", t);

            _tags.Add(t);
            TempData["Success"] = "Tag created successfully!";
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tag tag)
        {
            if (!IsStaff) return Unauthorized();

            if (!ModelState.IsValid)
                return PartialView("Form", tag);

            try
            {
                Console.WriteLine("note: "+tag.Note);
                _tags.Update(tag); 
                TempData["Success"] = "Tag updated successfully.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("Form", tag);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(short id)
        {
            if (!IsStaff) return Unauthorized();

            try
            {
                var isSuccess = _tags.Delete(id);
                if (isSuccess)
                    TempData["Success"] = "Tag deleted successfully.";
                else
                    TempData["Error"] = "Cannot delete this tag because it is associated with articles.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the tag.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Articles(short id)
        {
            if (!IsStaff) return Unauthorized();

            var tag = _tags.Get(id);
            if (tag == null) return NotFound();

            ViewBag.TagId = id;
            ViewBag.TagName = tag.TagName;
            var articles = _tags.GetArticlesByTag(id);
            return View(articles);
        }
    }
}
