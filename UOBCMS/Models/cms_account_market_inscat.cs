using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_inscat
    {
        [Key]
        public int Id { get; set; }

        public int Account_mkt_id { get; set; }

        public string Product { get; set; }

        public string Category { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }
    }
}
