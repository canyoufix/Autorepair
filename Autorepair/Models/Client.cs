using System.ComponentModel.DataAnnotations;

namespace Autorepair.Models
{
	public class Client
	{
		public Guid Id { get; set; } // GUID устанавливается вручную

		[Display(Name = "Имя")]
		public string FirstName { get; set; } = string.Empty;

		[Display(Name = "Фамилия")]
		public string LastName { get; set; } = string.Empty;

		[Display(Name = "Номер телефона")]
		public string PhoneNumber { get; set; } = string.Empty;


		public List<Car> Cars { get; set; } = new List<Car>();
	}
}
