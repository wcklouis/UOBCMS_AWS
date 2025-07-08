using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_client_bank
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }
        public int? Bank_id { get; set; }
        public string Ccy { get; set; }
        public string? Bank_accno { get; set; }
        public string? Bank_accname { get; set; }
        public string? Bene_bank_swift { get; set; }
        public string? Bene_bank_address { get; set; }
        public string? Bene_bank_accname { get; set; }
        public string? Bene_bank_name { get; set; }
        public string? Corr_bank_name { get; set; }
        public string? Corr_bank_swift { get; set; }

        public string Default_payment_type { get; set; }

        public string? Fps_payment_type { get; set; }

        public string Fps_payment_type_string
        {
            get
            {
                switch (Fps_payment_type)
                {
                    case "0":
                        return "PTA";
                    case "1":
                        return "PTM";
                    default:
                        return "";
                }
            }
        }

        public string Default_payment_type_string
        {
            get
            {
                switch (Default_payment_type) // Assuming Status is a variable or property of an enum type
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

        public string StatusString
        {
            get
            {
                switch (Status) // Assuming Status is a variable or property of an enum type
                {
                    case "A":
                        return "Active";
                    case "V":
                        return "Void";
                    default:
                        return "";
                }
            }
        }

        public string Type { get; set; }
        public string? Other_bank_details { get; set; }

        public string? Bene_bank_country { get; set; }
        public string? Corr_bank_country { get; set; }
        public string? Bene_bank_code { get; set; }
        public string? Corr_bank_code { get; set; }
        public string? Msg_to_bank { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual cms_bank Cms_bank { get; set; }

        public virtual ICollection<cms_account_bank> Cms_account_banks { get; set; } = new List<cms_account_bank>();
    }
}
