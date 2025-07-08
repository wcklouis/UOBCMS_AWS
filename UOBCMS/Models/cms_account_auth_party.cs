using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_auth_party
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account")] // This tells EF to use Acc_id as the FK
        public int Acc_id { get; set; }
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
        public virtual cms_account Cms_account { get; set; }
    }
}
