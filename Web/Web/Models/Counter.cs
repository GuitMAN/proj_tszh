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
    }

    public class Counter_model
    { // Project of model
      //| Фамилия Имя Отчество | [ Газ ] | [Электро] | [ХВ] | [ГВ] | Улица | Дом | Квартира | [Дата]
      //  public int 
      //  public string Name;
        public IEnumerable<Counter> ListCounter;
        public IEnumerable<Counter_data> ListData;
    }
}