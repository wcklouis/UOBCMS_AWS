using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_client_phone
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }
        public string Type { get; set; }

        public string Sub_type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "1":
                        return "Tel 1";
                    case "2":
                        return "Tel 2";
                    case "3":
                        return "Tel 3";
                    case "4":
                        return "Tel 4";
                    default:
                        return "";
                }
            }
        }

        public string SubTypeString
        {
            get
            {
                switch (Sub_type)
                {
                    case "1":
                        return "Home";
                    case "2":
                        return "Office";
                    case "3":
                        return "Mobile";
                    default:
                        return "";
                }
            }
        }

        public string Country_code { get; set; }
        public string Area_code { get; set; }
        public string Value { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual ICollection<cms_account_phone> Cms_account_phones { get; set; } = new List<cms_account_phone>();
    }
}
