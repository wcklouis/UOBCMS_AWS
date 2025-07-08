using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_client_email
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }
        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                /*switch (Type)
                {
                    case "0":
                        return "E-Statement";
                    case "1":
                        return "EMail_Addr";
                    case "2":
                        return "TCREMail";
                    case "3":
                        return "Primary Email";
                    default:
                        return "";
                }*/

                switch (Type)
                {
                    case "0":
                        return "Primary Email Address 1";
                    case "1":
                        return "Alternative Email Address 1";
                    case "2":
                        return "Alternative Email Address 2";
                    case "3":
                        return "Primary Email Address 2";
                    case "4":
                        return "Primary Email Address 3";
                    case "5":
                        return "Primary Email Address 4";
                    case "6":
                        return "Primary Email Address 5";
                    default:
                        return "";
                }
            }
        }
        public string Email { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual ICollection<cms_account_email> Cms_account_emails { get; set; } = new List<cms_account_email>();
    }
}
