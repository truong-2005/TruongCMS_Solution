using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization; // Thư viện hỗ trợ kiểm tra đăng nhập

namespace CMS.Backend.Controllers
{
    [Authorize] // Khóa trang này lại, chưa đăng nhập sẽ bị đá ra trang Login
    [Route("OrderDetail/[action]")]
    [Route("OrderDetails/[action]")] // Chống lỗi 404 cho cả số ít và số nhiều
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GIAO DIỆN ADMIN C#: TRANG CHI TIẾT ĐƠN HÀNG (Razor View)
        [Route("~/OrderDetail/{orderId?}")]
        [Route("~/OrderDetails/{orderId?}")]
        [HttpGet]
        public IActionResult Index(int? orderId)
        {
            if (orderId == null)
            {
                return RedirectToAction("Index", "Order"); // Nếu không có mã đơn, quay về danh sách đơn hàng
            }

            // 1. Truy vấn thông tin Đơn hàng kèm theo thông tin Khách hàng
            var order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng này.");
            }

            // Gán vào ViewBag để View sử dụng
            ViewBag.Order = order;

            // 2. Truy vấn danh sách Chi tiết đơn hàng kèm theo thông tin Sản phẩm
            var data = _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToList();

            return View(data);
        }

        // ────────────────────────────────────────────────────────
        // 🌟 ENDPOINT API: ĐÃ SỬA HẾT GẠCH ĐỎ - ĐỔ DỮ LIỆU SANG REACTJS
        // ────────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet("~/api/Order/Details/{orderId}")]
        public IActionResult GetOrderDetailApi(int orderId)
        {
            // Nạp thông tin đơn hàng kèm liên kết bảng Customer để lấy tên
            var order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null) return NotFound(new { message = "Không tìm thấy hóa đơn yêu cầu!" });

            // Lấy danh sách chậu cây cảnh thuộc đơn hàng này (Dùng cột UnitPrice chuẩn của Toàn)
            var details = _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
.Select(od => new {
    name = od.Product != null ? od.Product.Name : "Sản phẩm cây cảnh",
    price = od.UnitPrice, // 🌟 Đã sửa thành UnitPrice khớp với Entity
    quantity = od.Quantity
})
                .ToList();

            // 🌟 GIẢI PHÁP: Tự động tính toán tổng tiền bằng LINQ thay vì gọi cột TotalAmount không tồn tại
            decimal calculatedTotal = details.Sum(d => d.price * d.quantity);

            return Ok(new
            {
                id = order.Id,
                orderDate = order.OrderDate.ToString("dd/MM/yyyy HH:mm"), // 🌟 OrderDate chuẩn của Toàn
                totalPrice = calculatedTotal, // Trả về số tiền vừa tính toán
                receiverName = order.Customer != null ? (order.Customer.FullName ?? "thien") : "thien", // 🌟 Lấy từ thực thể Customer liên kết
                products = details
            });
        }
    }
}