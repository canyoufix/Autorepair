using System.ComponentModel.DataAnnotations;

namespace Autorepair.Models
{
	public class Login
	{
		[Required(ErrorMessage = "Введите адрес электронной почты.")]
		[EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Введите пароль.")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;
	}
}
