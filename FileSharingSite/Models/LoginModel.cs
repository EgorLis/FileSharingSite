using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FileSharingSite.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ErrorMessage { get;set; }
    }
}