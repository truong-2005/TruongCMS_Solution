using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data; // Đảm bảo khớp với Namespace chứa ApplicationDbContext trong Solution của bạn
using System.Threading.Tasks;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // 1. Cấu hình đường dẫn API: api/CategoriesProducts
    [Route("api/[controller]")]
    // 2. Kích hoạt tính năng tự động kiểm tra lỗi dữ liệu (Validation)
    [ApiController]
    // 3. Kế thừa ControllerBase để tối ưu bộ nhớ cho API thuần dữ liệu JSON
    public class CategoriesProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // 4. Hàm khởi tạo: Nạp cơ sở dữ liệu SQL Server vào Controller thông qua DI
        public CategoriesProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// API lấy toàn bộ danh mục sản phẩm thời trang (Giao thức GET)
        /// Đường dẫn gọi dữ liệu: GET https://localhost:xxxx/api/CategoriesProducts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Bước A: Quét bảng dữ liệu CategoriesProducts dưới SQL Server lên
                var categories = await _context.CategoriesProducts
                    .OrderBy(c => c.DisplayOrder) // Ưu tiên sắp xếp theo thứ tự hiển thị
                    .Select(c => new {
                        // Bước B: Kỹ thuật gọt tỉa (Projection) - chỉ lấy các trường cần thiết ra FrontEnd
                        c.Id,
                        c.Name,
                        c.Description,
                        c.DisplayOrder,
                        c.IsActive
                    })
                    .ToListAsync(); // Chuyển đổi bất đồng bộ sang dạng danh sách mảng

                // Bước C: Trả về mã thành công HTTP 200 OK đính kèm chuỗi JSON sạch
                return Ok(categories);
            }
            catch (System.Exception ex)
            {
                // Bảo vệ hệ thống: Nếu sập kết nối SQL thì trả về lỗi 500 kèm lời nhắn lý do lỗi
                return StatusCode(500, new
                {
                    message = "Lỗi kết nối cơ sở dữ liệu hệ thống",
                    detail = ex.Message
                });
            }
        }
    }
}