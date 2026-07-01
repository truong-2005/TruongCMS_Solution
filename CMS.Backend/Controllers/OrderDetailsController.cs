

using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    [Authorize] // Bắt buộc phải đăng nhập mới được vào các hàm bên dưới
    // Controller để quản lý chi tiết đơn hàng
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DANH SÁCH CHI TIẾT ĐƠN HÀNG
        [HttpGet]
        public IActionResult Index()
        {
            var orderDetails = _context.OrderDetails.ToList();

            return View(orderDetails);
        }

        // CHI TIẾT
        public IActionResult Details(int id)
        {
            var orderDetail = _context.OrderDetails.Find(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }
    }
}