using System.ComponentModel.DataAnnotations;

namespace Autorepair.Models
{
	public class Service
	{
		public Guid Id { get; set; }

		[Display(Name = "Машина")]
		public Guid CarId { get; set; } // Внешний ключ для машины

		[Display(Name = "Машина")]
		public Car? Car { get; set; } // Навигационное свойство для машины

		[Display(Name = "Услуга")]
		public Guid? PartId { get; set; }

		[Display(Name = "Услуга")]
		public Part? Part { get; set; }

		[Display(Name = "Доп. описание")]
		public string? ServiceDescription { get; set; } = string.Empty;

		[Display(Name = "Стоимость")]
		public int ServiceCost { get; set; }

		[Display(Name = "Дата выполнения")]
		public DateTime? ServiceCompletionDate { get; set; }

		public List<Service> Services { get; set; } = new List<Service>();
	}

}
