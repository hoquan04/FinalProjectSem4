using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100000 ký tự")]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Navigation
        public ICollection<Product>? Products { get; set; }
    }
}
