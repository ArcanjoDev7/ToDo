using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.Domain.Models
{
    [Index(nameof(Email),IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Email { get; set; } 
        public required string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateTime {  get; set; }


        [ForeignKey("UserId")]
        public List<User> Users { get; set; } = new();
    }
}