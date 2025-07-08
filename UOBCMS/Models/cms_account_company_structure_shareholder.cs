using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DevExpress.XtraPrinting;

namespace UOBCMS.Models
{
    public class cms_account_company_structure_shareholder
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account_company_structure")] // This tells EF to use Acc_id as the FK
        public int Comp_struct_id { get; set; }
        public string Ename { get; set; }
        public string Cname { get; set; }
        public string Gender { get; set; }
        public DateTime Init_dt { get; set; }

        public string Id_type { get; set; }

        public string Id_typeString
        {
            get
            {
                switch (Id_type) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "1. ID Card";
                    case "2":
                        return "2. Passport";
                    case "3":
                        return "3. Travel Doc";
                    case "4":
                        return "4. Driving LC";
                    case "5":
                        return "5. CI";
                    case "6":
                        return "6. Com Search Report";
                    case "7":
                        return "7. Others(Free Text)";
                    default:
                        return "";
                }
            }
        }

        public string Id_no { get; set; }

        public string Nationality_or_place { get; set; }

        public string Type { get; set; }      

        public string TypeString
        {
            get
            {
                switch (Type) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "1. BO";
                    case "2":
                        return "2. Sr.Managing Offical";
                    case "3":
                        return "3. FI";
                    case "4":
                        return "4. Bank";
                    case "5":
                        return "5. Listing";
                    case "6":
                        return "6. Government";
                    case "7":
                        return "7. Others(Free text)";
                    default:
                        return "";
                }
            }
        }
        public DateTime Effective_date { get; set; }

        public string Remarks { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
        public virtual cms_account_company_structure Cms_account_company_structure { get; set; }
    }
}
