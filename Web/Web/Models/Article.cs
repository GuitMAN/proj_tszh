using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Articles")]
    public class Article
    {
        [Key]
        public int id { get; set; }
        public int id_uk { get; set; }
        [Display(Name = "Пункт меню")]
        public string title { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Название статьи")]
        public string summary { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Основной текст")]
        public string content { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Дата публикации")]
        public DateTime publicDate { get; set; }
    }
}