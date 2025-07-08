using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_interest
    {
        [Key]
        public int Id { get; set; }
        public int Account_mkt_id { get; set; }

        public string Ccy { get; set; }

        public string Bal_status { get; set; }

        public string Bal_statusString
        {
            get
            {
                switch (Bal_status)
                {
                    case "0":
                        return "DR";
                    case "1":
                        return "CR";
                    default:
                        return "";
                }
            }
        }

        public DateTime Eff_dt { get; set; }        

        public string Comment { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }

        public virtual ICollection<cms_account_market_interest_detail> Cms_account_market_interest_details { get; set; } = new List<cms_account_market_interest_detail>();
    }
}
