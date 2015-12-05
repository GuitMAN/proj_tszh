using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Feedback")]
    public class feedback
    {
        public int id { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "Ваша управляющая компания (ТСЖ): ")]
        public int id_uk { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "От кого: ")]
        public int id_user { get; set; }
        [Display(Name = "Тема письма")]
        [Required(ErrorMessage = "Отсутствует тема письма")]
        public string title { get; set; }
        [Display(Name = "Сообщение")]
        [Required(ErrorMessage = "Текст сообщения пуст")]
        public string message { get; set; }
        [Display(Name = "Дата и время отправки сообщения")]
        public DateTime datetime { get; set; }
        public bool status { get; set; } 
             
    }
}