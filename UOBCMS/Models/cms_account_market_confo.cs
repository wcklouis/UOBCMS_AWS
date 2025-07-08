using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market_confo
    {
        [Key]
        public int Id { get; set; }

        public int Account_mkt_id { get; set; }

        public string Ccy { get; set; }

        public int Instruction_code { get; set; }
        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type) // Assuming Status is a variable or property of an enum type
                {
                    case "B":
                        return "Buy";
                    case "S":
                        return "Sell";
                    case "C":
                        return "Combine";
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
