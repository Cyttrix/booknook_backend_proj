using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FullStackAuth_WebAPI.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BookId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ThumbnailUrl { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
