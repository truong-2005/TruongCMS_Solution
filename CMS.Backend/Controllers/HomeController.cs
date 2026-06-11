using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data; // Thư mục chứa DbContext [cite: 568]
using System.Linq;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // LINQ: Lấy 3 bài viết mới nhất
        var latestPosts = _context.Posts
            .Include(p => p.Category) // Đã xóa [cite_start] - Lấy kèm tên danh mục để hiển thị
            .OrderByDescending(p => p.CreatedDate) // Đã xóa [cite_start] - Sắp xếp ngày mới nhất lên đầu
            .Take(3) // Chỉ lấy đúng 3 bản tin đầu tiên 
            .ToList();

        return View(latestPosts);
    }

}
