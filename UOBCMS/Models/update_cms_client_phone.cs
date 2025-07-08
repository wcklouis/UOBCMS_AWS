using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class update_cms_client_phone
    {
        [Key]
        public int Id { get; set; }
        public int Client_id { get; set; }

        public int client_phone_id { get; set; }

        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "1":
                        return "Home";
                    case "2":
                        return "Office";
                    case "3":
                        return "Mobile";
                    case "4":
                        return "Fax";
                    default:
                        return "";
                }
            }
        }

        public string Country_code { get; set; }
        public string Area_code { get; set; }
        public string Value { get; set; }


        public string pre_type { get; set; }
        public string Pre_TypeString
        {
            get
            {
                switch (pre_type)
                {
                    case "1":
                        return "Home";
                    case "2":
                        return "Office";
                    case "3":
                        return "Mobile";
                    case "4":
                        return "Fax";
                    default:
                        return "";
                }
            }
        }
        public string Pre_Country_code { get; set; }
        public string Pre_Area_code { get; set; }
        public string Pre_Value { get; set; }

        public string submitted_by { get; set; }
        public DateTime submitted_dt { get; set; }
        public string approved_by { get; set; }
        public DateTime? approved_dt { get; set; } // approval time
        public string approval_status { get; set; } // Pending, Approved, Rejected
        public string ApprovalStatus
        {
            get
            {
                switch (approval_status)
                {
                    case "P":
                        return "Pending";
                    case "A":
                        return "Approved";
                    case "R":
                        return "Rejected";
                    default:
                        return "";
                }
            }
        }

        public string comments { get; set; }
    }

}
