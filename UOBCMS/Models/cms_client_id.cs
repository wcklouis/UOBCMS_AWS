using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_client_id
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
                switch (Type)
                {
                    case "0":
                        return "Resident ID";
                    case "1":
                        return "BR No.";
                    case "2":
                        return "International ID";
                    case "3":
                        return "TIN";
                    default:
                        return "";
                }
            }
        }

        public string Idno { get; set; }
        public string Issue_country { get; set; }
        public DateTime? Issue_dt { get; set; }

        public DateTime? Exp_dt { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual cms_client_north_bound Cms_client_north_bound { get; set; } // Navigation property
    }
}
