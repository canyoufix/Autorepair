using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Autorepair.Models
{
	public class Employee
	{
		public Guid Id { get; set; }

		[Display(Name = "Имя")]
		public string FirstName { get; set; } = string.Empty;

		[Display(Name = "Фамилия")]
		public string LastName { get; set; } = string.Empty;

		[Display(Name = "Должность")]
		public string Position { get; set; } = string.Empty;

		[Display(Name = "Номер телефона")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Display(Name = "Зарплата")]
		public int Salary { get; set; }
	}
}
