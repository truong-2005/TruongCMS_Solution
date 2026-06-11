using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize] // Bắt buộc phải đăng nhập mới được vào các hàm bên dưới

public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================
    // DANH SÁCH KHÁCH HÀNG
    // =========================
    public IActionResult Index()
    {
        var data = _context.Customers.ToList();

        return View(data);
    }

    // =========================
    // XEM CHI TIẾT KHÁCH HÀNG
    // =========================
    public IActionResult Details(int id)
    {
        var customer = _context.Customers
            .Include(x => x.Orders)
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    // =========================
    // THÊM KHÁCH HÀNG
    // =========================

    // GET
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST
    [HttpPost]
    public IActionResult Create(Customer model)
    {
        // Kiểm tra email đã tồn tại chưa
        var checkEmail = _context.Customers
            .Any(x => x.Email == model.Email);

        if (checkEmail)
        {
            ModelState.AddModelError("Email", "Email đã tồn tại!");

            return View(model);
        }

        // =========================
        // MÃ HÓA PASSWORD
        // =========================
        model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // Lưu database
        _context.Customers.Add(model);

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // =========================
    // SỬA KHÁCH HÀNG
    // =========================

    // GET
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var customer = _context.Customers.Find(id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    // POST
    [HttpPost]
    public IActionResult Edit(Customer model, string NewPassword)
    {
        var customer = _context.Customers
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == model.Id);

        if (customer == null)
            return NotFound();

        // Nếu nhập password mới
        if (!string.IsNullOrEmpty(NewPassword))
        {
            model.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
        }
        else
        {
            // Giữ password cũ
            model.Password = customer.Password;
        }

        _context.Customers.Update(model);

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // =========================
    // XÓA KHÁCH HÀNG
    // =========================
    public IActionResult Delete(int id)
    {
        var customer = _context.Customers
            .Include(x => x.Orders)
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return NotFound();

        // Xóa đơn hàng trước
        if (customer.Orders != null && customer.Orders.Any())
        {
            _context.Orders.RemoveRange(customer.Orders);
        }

        // Xóa customer
        _context.Customers.Remove(customer);

        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}