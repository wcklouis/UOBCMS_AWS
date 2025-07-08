using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_bank
    {
        [Key]
        public int Id { get; set; }
        public int Client_bank_id { get; set; }
        public int Acc_id { get; set; }

        public string DefaultLocalHKDBank { get; set; }

        public string DefaultLocalHKDBankString
        {
            get
            {
                switch (DefaultLocalHKDBank) // Assuming Status is a variable or property of an enum type
                {
                    case "N":
                        return "No";
                    case "Y":
                        return "Yes";
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

        public virtual cms_client_bank Cms_client_bank { get; set; }

        public virtual cms_bank Cms_bank { get; set; }

        public virtual cms_account_market Cms_account_market { get; set; }
    }
}
