using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FileSharingSite.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Display(Name = "Логин")]
        [StringLength(30, MinimumLength = 5)]
        [Required]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [StringLength(30, MinimumLength = 5)]
        [Required]
        public string Password { get; set; }


    }
}