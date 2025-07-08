using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_interest_detail
    {
        [Key]
        public int Id { get; set; }

        public int Account_mkt_ccy_id { get; set; }

        public decimal Reach { get; set; }

        public string Int_type { get; set; }

        public string Int_typeString
        {
            get
            {
                switch (Int_type)
                {
                    case "0":
                        return "N/A";
                    case "1":
                        return "PRIME";
                    case "2":
                        return "LIBOR";
                    default:
                        return "";
                }
            }
        }

        public decimal Adj_rate { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market_interest Cms_account_market_interest { get; set; }

    }
}
