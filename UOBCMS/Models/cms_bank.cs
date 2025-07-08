using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_bank
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Bank_code { get; set; }
        public string Bank_name { get; set; }
        public string? Bank_swift { get; set; }

        public string Default_payment_type { get; set; }

        public string Default_payment_type_string
        {
            get
            {
                switch (Status) // Assuming Status is a variable or property of an enum type
                {
                    case "0":
                        return "E-Payment";
                    case "1":
                        return "FPS";
                    case "2":
                        return "TT";
                    default:
                        return "";
                }
            }
        }

        public string Status { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual ICollection<cms_client_bank> Cms_client_banks { get; set; } = new List<cms_client_bank>();

        public virtual ICollection<cms_account_bank> Cms_account_banks { get; set; } = new List<cms_account_bank>();
    }
}
