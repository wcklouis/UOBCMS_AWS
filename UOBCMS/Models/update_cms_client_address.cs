using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class update_cms_client_address
    {
        [Key]
        public int Id { get; set; }
        public int Client_id { get; set; }
        public int client_address_id { get; set; }
        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "0":
                        return "Residential";
                    case "1":
                        return "Correspondance";
                    default:
                        return "";
                }
            }
        }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Country { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Postal_code { get; set; }
        public string Zip { get; set; }


        public string Pre_Type { get; set; }

        public string Pre_TypeString
        {
            get
            {
                switch (Pre_Type)
                {
                    case "0":
                        return "Residential";
                    case "1":
                        return "Correspondance";
                    default:
                        return "";
                }
            }
        }
        public string Pre_Addr1 { get; set; }
        public string Pre_Addr2 { get; set; }
        public string Pre_Addr3 { get; set; }
        public string Pre_Addr4 { get; set; }
        public string Pre_Country { get; set; }
        public string Pre_District { get; set; }
        public string Pre_City { get; set; }
        public string Pre_Province { get; set; }
        public string Pre_Postal_code { get; set; }
        public string Pre_Zip { get; set; }

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
