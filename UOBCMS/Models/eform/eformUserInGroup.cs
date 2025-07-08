using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.eform
{
    public class eformUserInGroup
    {
        [Key]
        public int Id { get; set; }
        public int Usergpid { get; set; }
        public int Userid { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        // Navigation property for the many-to-many relationship

        public virtual eformUser eFormUser { get; set; } // Navigation property
        public virtual eformUserGroup eFormUserGroup { get; set; } // Navigation property
    }
}
