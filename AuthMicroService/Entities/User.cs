using System.ComponentModel.DataAnnotations;

namespace AuthMicroService.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        [MaxLength(50)]
        public string? UserName { get; set; }
        [MaxLength(100)]
        public string? PasswordHash { get; set; }
        [MaxLength(50)]
        public string? Email { get; set; }

        // Auditing
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        //Navigation
        public ICollection<UserRoleMapping> UserRoles { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
