using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class update_cms_client_email
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }
        
        public int client_email_id { get; set; }

        public string Type { get; set; }
        public string TypeString
        {
            get
            { 
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

        public string Pre_Type { get; set; }
        public string Pre_TypeString
        {
            get
            {
                switch (Pre_Type)
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
        public string Pre_Email { get; set; }


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
