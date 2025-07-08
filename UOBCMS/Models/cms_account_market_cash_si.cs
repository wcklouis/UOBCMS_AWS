using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_cash_si
    {
        [Key]
        public int Id { get; set; }

        public int Account_mkt_id { get; set; }

        public string Ccy { get; set; }

        public string Method { get; set; }
        public string Default { get; set; }

        public string DefaultString
        {
            get
            {
                switch (Default) // Assuming Status is a variable or property of an enum type
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

        public string Status { get; set; }

        public string StatusString
        {
            get
            {
                switch (Status) // Assuming Status is a variable or property of an enum type
                {
                    case "A":
                        return "Active";
                    case "S":
                        return "Suspended";
                    default:
                        return "";
                }
            }
        }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }
    }
}
