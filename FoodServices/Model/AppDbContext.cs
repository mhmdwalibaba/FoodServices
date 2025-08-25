using Microsoft.EntityFrameworkCore;
using MRKHServices.Persistence.Entites;
using System.ServiceProcess;

namespace FoodServices.Model
{
	public class AppDbContext:DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Service> services { get; set; }
		public DbSet<FoodServices.Model.ServiceType> serviceTypes { get; set; }
	}
}
