using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Counter")]
    public class Counter
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Тип счетчика: газ, вода, электр.")]
        public int type { get; set; }
        [Display(Name = "Серийный номер")]
        public string serial { get; set; }
        public int UserId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Дата проверки")]
        [DisplayFormat(DataFormatString = "{0:d/m/yy}")]
        public DateTime? DateOfReview { get; set; }
        [Display(Name = "Место установки")]
        public string place { get; set; }
        [Display(Name = "Статус")]
        public bool status { get; set; }
    }

    [Table("Counter_data")]
    public class Counter_data
    {
        [ScaffoldColumn(false)]
        public int id { get; set; }
        [ScaffoldColumn(false)]
        [Column(TypeName = "datetime")]
        [Display(Name = "Дата внесения показаний")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? write { get; set; }
        [Display(Name = "Показания счетчика")]
        public decimal data { get; set; }
        public bool status { get; set; } //1 - отправлено 2 - принято 3 - отклонено
    }




    public class count_place
    {
        public decimal data { get; set; }
        public string place { get; set; }
    }

    public class Counter_model
    { // Project of model
      //| Фамилия Имя Отчество | [ Газ ] | [Электро] | [ХВ] | [ГВ] | Улица | Дом | Квартира | месяц
        public IEnumerable<int> id; //список инедтификаторов счетчиков с данными
        public string Name; //FIO users
        [Display(Name = "Показания счетчика газа")]
        public List<count_place> gasi { get; set; }
        [Display(Name = "Показания счетчика электроэнергии")]
        public List<count_place> energoi { get; set; }
        [Display(Name = "Показания счетчиков холодной воды")]
        public List<count_place> cwi { get; set; } 
        [Display(Name = "Показания счетчика горячей воды")]
        public List<count_place> hwi { get; set; }
        [Display(Name = "Улица")]
        public string street { get; set; }
        [Display(Name = "Дом")]
        public string house { get; set; }
        [Display(Name = "Квартира")]
        public string flat { get; set; }
        [Display(Name = "Месяц")]
        public DateTime month { get; set; }
        [Display(Name = "Статус")]
        public bool status { get; set; } //1 - отправлено 2 - принято 3 - отклонено
        //В будущем избавиться
        public IEnumerable<Counter> ListCounter;
        public IEnumerable<Counter_data> ListData;
    }

    public class ListCounters
    {
        public List<Counter_data> Counters { get; set; }
    };
}