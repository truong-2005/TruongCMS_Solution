using System;

namespace CMS.Data.Entities
{
    public class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; }       // Tiêu đề banner
        public string? Description { get; set; } // Mô tả ngắn (nullable)
        public string? ImageUrl { get; set; }    // Đường dẫn hình ảnh banner (nullable)
        public string? LinkUrl { get; set; }     // Đường dẫn liên kết khi nhấn (nullable)
        public int Order { get; set; }          // Thứ tự hiển thị
        public bool IsActive { get; set; } = true; // Trạng thái hiển thị (kích hoạt hay không)
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}