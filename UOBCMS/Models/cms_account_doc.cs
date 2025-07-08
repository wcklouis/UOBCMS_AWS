using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_account_doc
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account")]
        public int Acc_id { get; set; }
        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "0":
                        return "Daily Statement";
                    case "1":
                        return "Monthly Statement";
                    case "2":
                        return "Contract Note";
                    default:
                        return "";
                }
            }
        }

        public int Copies { get; set; }

        public string Hold { get; set; }

        public string HoldString
        {
            get
            {
                switch (Hold)
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
    }
}
