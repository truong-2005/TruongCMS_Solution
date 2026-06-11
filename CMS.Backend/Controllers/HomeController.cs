using CMS.Backend.Models;
using CMS.Data; // Thư mục chứa DbContext [cite: 568]
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

[Authorize] // Bắt buộc phải đăng nhập mới được vào các hàm bên dưới
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    // Constructor để khởi tạo DbContext, giúp truy cập dữ liệu từ cơ sở dữ liệu
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // LINQ: Lấy 3 bài viết mới nhất
        var latestPosts = _context.Posts
            .Include(p => p.Category) // Lấy kèm tên danh mục
            .OrderByDescending(p => p.CreatedDate) // Mới nhất lên đầu
            .Take(3)
            .ToList();

        return View(latestPosts);
    }
}