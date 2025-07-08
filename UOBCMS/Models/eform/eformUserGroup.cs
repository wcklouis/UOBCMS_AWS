using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.eform
{
    public class eformUserGroup
    {
        [Key]
        public int Id { get; set; }
        public string Usergpname { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        // Navigation property for the many-to-many relationship
        public virtual ICollection<eformUserInGroup> eFormUserInGroups { get; set; } = new List<eformUserInGroup>();
    }
}
