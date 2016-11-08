using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Admtszh")]
    public class Admtszh
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Управляющая компания")]
        public int id_uk { get; set; }    
        public int AdmtszhId { get; set; }
        [Display(Name = "Должность")]
        public string post { get; set; }
        [Display(Name = "Фамилия")]
        public string SurName { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }
    }

    public class newAdmtszh
    {
        //public int id { get; set; }
        
        public int id_uk { get; set; }
        public int AdmtszhId { get; set; }
        public string post { get; set; }    
        public string SurName { get; set; }      
        public string Name { get; set; }     
        public string Patronymic { get; set; }
    }




}