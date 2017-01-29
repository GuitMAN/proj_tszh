using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("UserProfiles")]
    public class UserProfile
    {
        public UserProfile(){}
        public UserProfile(UserProfile_form prof)
        {
       //     id = prof.id;
            UserId = prof.UserId;
            id_uk = prof.id_uk;
            login = prof.login;        
            SurName = prof.SurName;
            Name = prof.Name;
            Patronymic = prof.Patronymic;
            Personal_Account = prof.Personal_Account;
            Adress = prof.Adress;
            Apartment = prof.Apartment;
            Email = prof.Email;
            phone = prof.phone;
            mobile = prof.mobile;
        } 

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //      public int id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Управляющая компания")]
        public int id_uk { get; set; }
        [Display(Name = "Логин")]
        public string login { get; set; }
        [Display(Name = "Фамилия")]
        public string SurName { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }       
        [Display(Name = "Лицевой счет")]
        public string Personal_Account { get; set; }
        [Display(Name = "Домашний адрес")]
        public int Adress { get; set; }
        [Display(Name = "Квартира")]
        public string Apartment { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
        public string phone { get; set; }
        [Display(Name = "Сотовый")]
        public string mobile { get; set; }
    }


    public class UserProfile_nouk_form
    {
        public int UserId  { get; set; }
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        [Display(Name = "Фамилия")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        [Display(Name = "Имя")]
        public string Name  { get; set; }
        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        public string Patronymic  { get; set; }
        [Display(Name = "Лицевой счет")]
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        public string Personal_Account  { get; set; }
        [Display(Name = "Домашний адрес: город улица дом")]
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        public int Adress  { get; set; }
        [Display(Name = "Квартира")]
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        public string Apartment  { get; set; }
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Это поле обязательно к заполнению")]
        public string Email  { get; set; }
        [Display(Name = "Телефон")]
        public string phone  { get; set; }
    }

    //Модель первичной регистрации
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Логин")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress)]
        //[Display(Name = "E-mail")]
        //public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} должен содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

    }
    
    
    //Модель входа
    public class LoginModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} должен содержать не менее {2} символов.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Сохранить?")]
        public bool RememberMe { get; set; }
    }
    //Модель обновления пароля
    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} должен содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class Account_model
    {
        public int id { get; set; }
        public string Login { get; set; }
        public string[] Role { get; set; }
    }

    public class UserProfile_form
    {
        public UserProfile_form() { }
        public UserProfile_form(UserProfile prof)
        {
   //         id = prof.id;
            UserId = prof.UserId;
            id_uk = prof.id_uk;
            login = prof.login;        
            SurName = prof.SurName;
            Name = prof.Name;
            Patronymic = prof.Patronymic;
            Personal_Account = prof.Personal_Account;
            Adress = prof.Adress;
            Apartment = prof.Apartment;
            Email = prof.Email;
            phone = prof.phone;
            mobile = prof.mobile;
            Role = null;
        } 

   //     public int id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Управляющая компания")]
        public int id_uk { get; set; }
        [Display(Name = "Логин")]
        public string login { get; set; }
        [Display(Name = "Фамилия")]
        public string SurName { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }
        [Display(Name = "Лицевой счет")]
        public string Personal_Account { get; set; }
        [Display(Name = "Домашний адрес")]
        public int Adress { get; set; }
        [Display(Name = "Квартира")]
        public string Apartment { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
        public string phone { get; set; }
        [Display(Name = "Сотовый")]
        public string mobile { get; set; }
        public string[] Role { get; set; } 
    }


}



