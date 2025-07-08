using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_brokerage_detail
    {
        [Key]
        public int Id { get; set; }

        public int Account_mkt_brokerage_id { get; set; }

        public int Seqno { get; set; }

        public decimal Fm_amt { get; set; }
        public decimal To_amt { get; set; }

        public decimal Rate { get; set; }
        public decimal Additional_Amt { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market_brokerage Cms_account_market_brokerage { get; set; }
    }
}
