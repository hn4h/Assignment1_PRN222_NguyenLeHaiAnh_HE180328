using System;
using System.Linq;
using NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Presentation.Controllers
{
    public class ReportController : Controller
    {
        readonly INewsArticleService _news;
        public ReportController(INewsArticleService news) => _news = news;

        bool IsAdmin() => HttpContext.Session.GetInt32("Role") == 0;

        public IActionResult Index(DateTime? start, DateTime? end, string groupBy)
        {
            if (!IsAdmin()) return Unauthorized();

            var list = _news.GetAll()                
                            .Where(n => !start.HasValue || n.CreatedDate >= start)
                            .Where(n => !end.HasValue || n.CreatedDate < end.Value.AddDays(1))
                            .OrderByDescending(n => n.CreatedDate)
                            .ToList();

            ViewBag.Start = start?.ToString("yyyy-MM-dd");
            ViewBag.End = end?.ToString("yyyy-MM-dd");
            ViewBag.GroupBy = groupBy ?? "";
            ViewBag.ActiveCount = list.Count(n => n.NewsStatus == true);
            ViewBag.InactiveCount = list.Count(n => n.NewsStatus == false);
            
            return View(list);
        }

        public IActionResult ExportToExcel(DateTime? start, DateTime? end)
        {
            if (!IsAdmin()) return Unauthorized();

            var list = _news.GetAll()
                            .Where(n => !start.HasValue || n.CreatedDate >= start)
                            .Where(n => !end.HasValue || n.CreatedDate < end.Value.AddDays(1))
                            .OrderByDescending(n => n.CreatedDate)
                            .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("News Report");

            // Add headers
            worksheet.Cells[1, 1].Value = "Article ID";
            worksheet.Cells[1, 2].Value = "Title";
            worksheet.Cells[1, 3].Value = "Headline";
            worksheet.Cells[1, 4].Value = "Created Date";
            worksheet.Cells[1, 5].Value = "Category";
            worksheet.Cells[1, 6].Value = "Status";
            worksheet.Cells[1, 7].Value = "Created By";
            worksheet.Cells[1, 8].Value = "News Source";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 8])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data
            for (int i = 0; i < list.Count; i++)
            {
                var news = list[i];
                int row = i + 2;

                worksheet.Cells[row, 1].Value = news.NewsArticleId;
                worksheet.Cells[row, 2].Value = news.NewsTitle;
                worksheet.Cells[row, 3].Value = news.Headline;
                worksheet.Cells[row, 4].Value = news.CreatedDate?.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cells[row, 5].Value = news.Category?.CategoryName;
                worksheet.Cells[row, 6].Value = news.NewsStatus == true ? "Active" : "Inactive";
                worksheet.Cells[row, 7].Value = news.CreatedBy?.AccountName;
                worksheet.Cells[row, 8].Value = news.NewsSource;
            }

            // Add summary section
            int summaryRow = list.Count + 3;
            worksheet.Cells[summaryRow, 1].Value = "Summary:";
            worksheet.Cells[summaryRow, 1].Style.Font.Bold = true;
            worksheet.Cells[summaryRow + 1, 1].Value = "Total Articles:";
            worksheet.Cells[summaryRow + 1, 2].Value = list.Count;
            worksheet.Cells[summaryRow + 2, 1].Value = "Active:";
            worksheet.Cells[summaryRow + 2, 2].Value = list.Count(n => n.NewsStatus == true);
            worksheet.Cells[summaryRow + 3, 1].Value = "Inactive:";
            worksheet.Cells[summaryRow + 3, 2].Value = list.Count(n => n.NewsStatus == false);

            if (start.HasValue || end.HasValue)
            {
                worksheet.Cells[summaryRow + 4, 1].Value = "Date Range:";
                worksheet.Cells[summaryRow + 4, 2].Value = 
                    $"{(start?.ToString("yyyy-MM-dd") ?? "All")} to {(end?.ToString("yyyy-MM-dd") ?? "All")}";
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            var fileName = $"NewsReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var content = package.GetAsByteArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
