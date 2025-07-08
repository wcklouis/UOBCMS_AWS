using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_account_north_bound
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account")]
        public int Acc_id { get; set; }

        public string Consent { get; set; }
        public string Type { get; set; }
        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "1":
                        return "1 = Individual";
                    case "2":
                        return "2 = Joint Account";
                    case "3":
                        return "Legal Entity - Funds";
                    case "4":
                        return "Legal Entity - Fund managers and others";
                    case "5":
                        return "Proprietary or Principal Trading";
                    default:
                        return "";
                }
            }
        }
        public string Start_bcan { get; set; }
        public string End_bcan { get; set; }
        public string Assigned_firm_id { get; set; }
        public string Ttep_firm_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }
    }
}
