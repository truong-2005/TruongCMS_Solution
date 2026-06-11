using CMS.Data; // ApplicationDbContext
using CMS.Data.Entities; // Quan trọng: Phải có dòng này để dùng lớp Post
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm Index: Hiển thị danh sách bài viết mẫu
        // Tham số 'id' được truyền vào từ URL (ví dụ: /Post/Index/5)
        public IActionResult Index(int? id)
        {
            // 1. Kiểm tra nếu không có id truyền vào thì trả về lỗi hoặc toàn bộ bài viết
            if (id == null)
            {
                return BadRequest("Vui lòng cung cấp mã danh mục.");
            }

            // 2. Sử dụng LINQ với tham số 'id' linh hoạt
            var posts = _context.Posts
                        .Where(p => p.CategoryId == id)
                        .OrderByDescending(p => p.CreatedDate)
                        .Include(p => p.Category)
                        .ToList();

            // 3. Truyền dữ liệu ra View
            return View(posts);
        }


        // Hàm Details: Hiển thị chi tiết một bài viết (Bổ sung  khá giỏi)
      public IActionResult Details(int id)
{
    // 1. Truy vấn bài viết theo ID
    // Sử dụng .Include(p => p.Category) để lấy kèm thông tin Danh mục (Join bảng)
    var post = _context.Posts
        .Include(p => p.Category)
        .FirstOrDefault(p => p.Id == id);

    // 2. Kiểm tra nếu không tìm thấy bài viết (tránh lỗi màn hình trắng)
    if (post == null)
    {
        return NotFound(); // Trả về trang lỗi 404
    }

    // 3. Truyền dữ liệu sang View
    return View(post);
}


    }
}
