using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("vjtncj37_tszh.webpages_Roles")]
    public class webpages_Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    [Table("vjtncj37_tszh.webpages_UsersInRoles")]
    public class webpages_UsersInRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class UserInRole_model
    {
        public string RoleName { get; set; }
        public string UserName { get; set; }
    }

    public class UserInRole_model_
    {
        public string[] RoleName { get; set; }
        public string UserName { get; set; }
    }

}