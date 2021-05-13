using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FileSharingSite.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Логин")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина не менее 5 символов.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$",
         ErrorMessage = "Допускаются только буквы латинского алфавита и цифры.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина не менее 5 символов.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$",
         ErrorMessage = "Допускаются только буквы латинского алфавита и цифры.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    }
}