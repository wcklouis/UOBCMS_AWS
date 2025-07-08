using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_company_structure_intermediary
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account_company_structure")] // This tells EF to use Acc_id as the FK
        public int Comp_struct_id { get; set; }
        public string Ename { get; set; }
        public string Cname { get; set; }

        public string Place_of_incorpration { get; set; }

        public DateTime Effective_date { get; set; }

        public string Remarks { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
        public virtual cms_account_company_structure Cms_account_company_structure { get; set; }
    }
}
