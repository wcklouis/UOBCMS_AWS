using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_social_media
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account")] // This tells EF to use Client_id as the FK
        public int Acc_id { get; set; }
        public string Type { get; set; }

        public string Name { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "0":
                        return "Wechat";
                    case "1":
                        return "Whatsapp";
                    default:
                        return "";
                }
            }
        }
        public string No { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }
    }
}
