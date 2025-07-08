using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_limit
    {
        [Key]
        public int Id { get; set; }
        public int Acc_id { get; set; }

        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "0":
                        return "Margin Limit";
                    case "1":
                        return "Credit Limit";
                    case "2":
                        return "Transaction Limit";
                    case "3":
                        return "Daily Limit";
                    default:
                        return "";
                }
            }
        }

        public string Ccy { get; set; }

        public decimal Lmt { get; set; }

        public decimal Pct { get; set; }

        public DateTime Exp_dt { get; set; }

        public String Status { get; set; }

        public string StatusString
        {
            get
            {
                switch (Status)
                {
                    case "A":
                        return "Active";
                    case "S":
                        return "Suspend";
                    default:
                        return "";
                }
            }
        }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }
    }
}
