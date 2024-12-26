using System.ComponentModel.DataAnnotations;

namespace Autorepair.Models
{
	public class Part
	{
		public Guid Id { get; set; }

		[Display(Name = "Наименование детали")]
		public string Name { get; set; } = string.Empty;

		[Display(Name = "Стоимость")]
		public int Price { get; set; }
	}
}
