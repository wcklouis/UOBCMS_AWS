using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_quoteservice
    {
        [Key]
        public int Id { get; set; }

        public int Acc_id { get; set; }

        public DateTime End_dt { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }
    }
}
