using CMS.Data; // ApplicationDbContext
using CMS.Data.Entities; // Quan trọng: Phải có dòng này để dùng lớp Post
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Hàm Index: Hiển thị danh sách bài viết từ Database
        public IActionResult Index(int? id)
        {
            IQueryable<Post> query = _context.Posts
                .Include(p => p.Category);

            if (id != null)
            {
                query = query.Where(p => p.CategoryId == id);
            }

            var posts = query
                .OrderByDescending(p => p.CreatedDate)
                .ToList();

            return View(posts);
        }

        // 1. Hàm hiển thị form tạo mới bài viết (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Chúng ta lấy danh sách Category để đổ vào ViewBag
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }



        [HttpPost]
        public IActionResult Create(Post model, IFormFile uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // 1. Định nghĩa đường dẫn lưu file: wwwroot/uploads
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                // 2. Tạo tên file duy nhất để không bị đè dữ liệu
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                // 3. Chép file vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // 4. Lưu đường dẫn vào CSDL để sau này hiển thị
                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.Posts.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Hiển thị form kèm dữ liệu cũ
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();

            // Chuẩn bị lại danh sách danh mục để người dùng có thể đổi chuyên mục
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Thực hiện cập nhật
        [HttpPost]
        public IActionResult Edit(Post model, IFormFile uploadImage)
        {
            // Bước 1: Kiểm tra xem người dùng có chọn file ảnh mới không
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // Thực hiện quy trình upload giống như trang Create
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // Cập nhật đường dẫn ảnh mới vào model
                model.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                // Bước quan trọng: Nếu không upload ảnh mới, chúng ta phải giữ lại ảnh cũ
                // Chúng ta cần lấy lại giá trị ImageUrl từ Database để tránh bị ghi đè thành rỗng
                var oldPost = _context.Posts.AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                if (oldPost != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldPost.ImageUrl;
                }
            }
            _context.Posts.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            // 1. Tìm bài viết theo Id
            var post = _context.Posts.Find(id);

            if (post != null)
            {
                // 2. Xóa khỏi bộ nhớ tạm
                _context.Posts.Remove(post);

                // 3. Cập nhật xuống SQL Server
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
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
