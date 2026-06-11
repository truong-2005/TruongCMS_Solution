using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using CMS.Data.Entities;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // POST /api/Auth/CustomerRegister
        // 👉 Đăng ký khách hàng
        // ==================================================
        [HttpPost("CustomerRegister")]
        public async Task<IActionResult> CustomerRegister(CustomerRegister model)
        {
            // check email tồn tại
            var existEmail = await _context.Customers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (existEmail != null)
            {
                return BadRequest(new { message = "Email đã tồn tại" });
            }

            var customer = new Customer
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password, // lưu thô theo yêu cầu bài
                Phone = model.Phone,
                Address = model.Address
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đăng ký thành công",
                data = new
                {
                    customer.Id,
                    customer.FullName,
                    customer.Email
                }
            });
        }

        // ==================================================
        // POST /api/Auth/CustomerLogin
        // 👉 Đăng nhập khách hàng
        // ==================================================
        [HttpPost("CustomerLogin")]
        public async Task<IActionResult> CustomerLogin(CustomerLogin model)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email &&
                    x.Password == model.Password);

            if (customer == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
            }

            return Ok(new
            {
                message = "Đăng nhập thành công",
                data = new
                {
                    customer.Id,
                    customer.FullName,
                    customer.Email
                }
            });
        }
    }

    // =========================
    // DTO REGISTER
    // =========================
    public class CustomerRegister
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    // =========================
    // DTO LOGIN
    // =========================
    public class CustomerLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}