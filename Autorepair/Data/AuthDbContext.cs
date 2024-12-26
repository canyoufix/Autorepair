using Microsoft.EntityFrameworkCore;

public class User
{
	public int Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
}

public class AuthDbContext : DbContext
{
	public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

	public DbSet<User> Users { get; set; }
}
