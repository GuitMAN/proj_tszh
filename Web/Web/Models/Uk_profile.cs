using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("uk_profiles")]
    public class uk_profile
    {
        public int id { get; set; }
        [Display(Name = "Название УК или ТСЖ")]
        public string Name { get; set; }
        [Display(Name = "Дата регистрации")]
        public DateTime RegDate { get; set; }
        [Display(Name = "Имя поддомена")]
        public string host { get; set; }
        public string Email { get; set; }
    }

    [Table("uk_adress")]
    public class uk_adress
    {
        public int id { get; set; }
        [Display(Name = "Управляющая компания")]
        public int id_uk { get; set; }
        [Required(ErrorMessage = "Введите город")]
        [Display(Name = "Город")]
        public string City { get; set; }
        [Required(ErrorMessage = "Введите улицу")]
        [Display(Name = "Улица")]
        public string Street { get; set; }
        [Display(Name = "Дом")]
        [Required(ErrorMessage = "Введите дом")]
        public string House { get; set; }
    }

    public class seek_adress
    {
        [Required(ErrorMessage = "Введите город")]
        [Display(Name = "Город")]
        public string City { get; set; }
        [Required(ErrorMessage = "Введите улицу")]
        [Display(Name = "Улица")]
        public string Street { get; set; }
        [Display(Name = "Дом")]
        [Required(ErrorMessage = "Введите дом")]
        public string House { get; set; }
        [Display(Name = "Квартира")]
        [Required(ErrorMessage = "Введите квартиру")]
        public string Apartment { get; set; }
    }

}