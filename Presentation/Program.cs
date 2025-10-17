using Microsoft.EntityFrameworkCore;
using NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.DAO;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Repositories;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<AdminAccountOptions>(
    builder.Configuration.GetSection("AdminAccount"));


            /* ─── DbContext ───────────────────────────────────────── */
            builder.Services.AddDbContext<FunewsManagementContext>(opts =>
                opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            /* ─── MVC + Session ───────────────────────────────────── */
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            builder.Services.AddScoped<AccountDAO>();
            builder.Services.AddScoped<CategoryDAO>();
            builder.Services.AddScoped<NewsArticleDAO>();
            builder.Services.AddScoped<TagDAO>();

            /* ─── Repositories ────────────────────────────────────── */
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            /* ─── Services ───────────────────────────────────────── */
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
