using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_account_additionalinfo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account")] // This tells EF to use Client_id as the FK
        public int Acc_id { get; set; }

        [ForeignKey("Cms_additionalinfo")] // This tells EF to use Client_id as the FK
        public int Addinfo_id { get; set; }

        public string Value { get; set; }

        public string ValueString
        {
            get
            {
                switch (Value)
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

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }

        public virtual cms_additionalinfo Cms_additionalinfo { get; set; }
    }
}
