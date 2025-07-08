using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_brokerage
    {
        [Key]
        public int Id { get; set; }
        public int Account_mkt_id { get; set; }

        public string Ccy { get; set; }

        public string Source { get; set; }

        public string SourceString
        {
            get
            {
                switch (Source)
                {
                    case "0":
                        return "OctO";
                    case "1":
                        return "Ayers Internet (US)";
                    case "2":
                        return "Internet";
                    case "3":
                        return "Ayers Mobile (US)";
                    case "4":
                        return "Manual Input Trade";
                    case "5":
                        return "FTS";
                    case "6":
                        return "Agent Solution";
                    case "7":
                        return "Ayers BSS";
                    case "8":
                        return "Ayers Internet";
                    case "9":
                        return "Ayers Mobile";
                    default:
                        return "";
                }
            }
        }

        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "P":
                        return "Progressive";
                    case "S":
                        return "Sliding";
                    default:
                        return "";
                }
            }
        }

        public string Clnt_specify { get; set; }

        public string Clnt_specifyString
        {
            get
            {
                switch (Clnt_specify)
                {
                    case "Y":
                        return "Yes";
                    case "N":
                        return "No";
                    default:
                        return "";
                }
            }
        }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public decimal Discount { get; set; }

        public decimal Additional_discount { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }

        public virtual ICollection<cms_account_market_brokerage_detail> Cms_account_market_brokerage_details { get; set; } = new List<cms_account_market_brokerage_detail>();
    }
}
