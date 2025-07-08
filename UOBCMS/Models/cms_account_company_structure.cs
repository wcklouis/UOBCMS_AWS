using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DevExpress.Compatibility.System.Web;
using Microsoft.CodeAnalysis.Text;
using TencentCloud.Dlc.V20210125.Models;
using TencentCloud.Tcr.V20190924.Models;

namespace UOBCMS.Models
{
    public class cms_account_company_structure
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_account")] // This tells EF to use Acc_id as the FK
        public int Acc_id { get; set; }
        public string FinancialInstitutionSubCat { get; set; }        
        public string FinancialInstitutionSubCatString
        {
            get
            {
                switch (FinancialInstitutionSubCat) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "1. Exchange and Clearing House";
                    case "2":
                        return "2. Brokerage House";
                    case "3":
                        return "3. Asset Management House";
                    case "4":
                        return "4. Family Office";
                    case "5":
                        return "5. Registered/ Licensed Fund";
                    case "6":
                        return "6. Insurer";
                    case "7":
                        return "7. Private Bank";
                    case "8":
                        return "8. Bank";
                    case "9":
                        return "9. MPF or ORSO Provider";
                    case "10":
                        return "10. Central Bank or any Multilateral Agency";
                    case "11":
                        return "11. Others(Free Text)";
                    default:
                        return "";
                }
            }
        }
        public string NonFinancialInstitutionSubCat { get; set; }
        public string NonFinancialInstitutionSubCatString
        {
            get
            {
                switch (NonFinancialInstitutionSubCat) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "1. Limited by Share";
                    case "2":
                        return "2. Limited by Guarantee";
                    case "3":
                        return "3. Listed Company";
                    case "4":
                        return "4. Association";
                    case "5":
                        return "5. Charity";
                    case "6":
                        return "6. Trust";
                    case "7":
                        return "7. Trustee";
                    case "8":
                        return "8. Partnership";
                    case "9":
                        return "9. Limited Partnership";
                    case "10":
                        return "10. Other Un-incoporated Bodies";
                    case "11":
                        return "11. Others(Free Text)";
                    default:
                        return "";
                }
            }
        }
        public string PlaceOfRegulatory { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
        public virtual cms_account Cms_account { get; set; }

        public virtual ICollection<cms_account_company_structure_director> Cms_account_company_structure_directors { get; set; } = new List<cms_account_company_structure_director>();
        public virtual ICollection<cms_account_company_structure_shareholder> Cms_account_company_structure_shareholders { get; set; } = new List<cms_account_company_structure_shareholder>();

        public virtual ICollection<cms_account_company_structure_intermediary> Cms_account_company_structure_intermediaries { get; set; } = new List<cms_account_company_structure_intermediary>();
    }
}
