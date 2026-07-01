using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize] // Bắt buộc phải đăng nhập mới được vào các hàm bên dưới
// Controller để quản lý đơn hàng
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context; // Gán kết nối vào biến để sử dụng trong các phương thức của Controller
    }

    // Phương thức hiển thị danh sách đơn hàng
    public IActionResult Index()
    {
        var data = _context.Orders
            .Include(o => o.Customer)
            .ToList();

        return View(data);
    }

    // =========================
    // XEM CHI TIẾT ĐƠN HÀNG
    // =========================

    [HttpGet]
    public IActionResult Details(int id)
    {
        // Lấy thông tin đơn hàng kèm khách hàng
        var order = _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound("Không tìm thấy đơn hàng.");
        }

        // Gửi thông tin đơn hàng qua ViewBag
        ViewBag.Order = order;

        // Lấy danh sách chi tiết đơn hàng kèm sản phẩm
        var data = _context.OrderDetails
            .Include(od => od.Product)
            .Where(od => od.OrderId == id)
            .ToList();

        return View(data);
    }

    // =========================
    // CẬP NHẬT TRẠNG THÁI
    // =========================

    // GET
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var order = _context.Orders.Find(id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    public IActionResult Edit(Order model)
    {
        // Tìm đơn hàng theo id
        var order = _context.Orders.Find(model.Id);

        // Nếu tồn tại dữ liệu
        if (order != null)
        {
            // Cập nhật trạng thái
            order.Status = model.Status;

            // Cập nhật ghi chú
            order.Notes = model.Notes;

            // Lưu xuống database
            _context.SaveChanges();
        }

        // Quay về danh sách
        return RedirectToAction("Index");
    }
}