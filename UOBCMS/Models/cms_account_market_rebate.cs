using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_rebate
    {
        [Key]
        public int Id { get; set; }
        public int Account_mkt_id { get; set; }

        public string Ccy { get; set; }

        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "0":
                        return "CLIENT";
                    case "1":
                        return "AE";
                    case "3":
                        return "REBATE";
                    case "4":
                        return "INTRO";
                    default:
                        return "";
                }
            }
        }

        public string Calcmtd { get; set; }

        public decimal Rate { get; set; }

        public string Acc { get; set; }

        public string Desc { get; set; }

        public string Rebate_ccy { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }
    }
}
