using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autorepair.Models
{
	public class Car
	{
		public Guid Id { get; set; }

		[Display(Name = "Клиент")]
		public Guid ClientId { get; set; }

		[Display(Name = "Клиент")]
		public Client? Client { get; set; }

		[Display(Name = "Марка")]
		public string Brand { get; set; } = string.Empty;

		[Display(Name = "Модель")]
		public string Model { get; set; } = string.Empty;

		[Display(Name = "Гос номер")]
		public string CarNumber { get; set; } = string.Empty;

		[Display(Name = "Год выпуска")]
		public int? Year { get; set; }

		public List<Order> Orders { get; set; } = new List<Order>();
	}
}
