using Autorepair.Models;
using Microsoft.EntityFrameworkCore;

public class AutorepairDbContext : DbContext
{
    public AutorepairDbContext (DbContextOptions<AutorepairDbContext> options)
        : base(options)
    {
    }

    public DbSet<Autorepair.Models.Car> Car { get; set; } = default!;

    public DbSet<Autorepair.Models.Client> Client { get; set; } = default!;

    public DbSet<Autorepair.Models.Employee> Employee { get; set; } = default!;

    public DbSet<Autorepair.Models.Order> Order { get; set; } = default!;

    public DbSet<Autorepair.Models.Part> Part { get; set; } = default!;

    public DbSet<Autorepair.Models.Service> Service { get; set; } = default!;


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Order>()
			.HasOne(o => o.Employee) // Связь с Employee
			.WithMany() // Если нет навигационного свойства в Employee
			.HasForeignKey(o => o.EmployeeId)
			.OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
	}

}
