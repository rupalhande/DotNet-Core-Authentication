using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public string Role { get; set; }
    public string Address { get; set; } = "";

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpire { get; set; }

}
