using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("MenuItems")]
    public class MenuItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public int parent_id { get; set; }
        public int menu_id { get; set; }
        public string alias { get; set; }
    }
}