using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_price_cap
    {
        [Key]
        public int Id { get; set; }
        public int Account_mkt_id { get; set; }

        public string Sec_Code { get; set; }

        public decimal Margin_pct { get; set; }

        public decimal Price_cap { get; set; }

        public string Desc { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }
    }
}
