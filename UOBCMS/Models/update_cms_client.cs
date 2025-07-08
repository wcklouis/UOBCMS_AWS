using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace UOBCMS.Models
{
    public class update_cms_client
    {
        [Key]
        public int Id { get; set; }
        public int client_id { get; set; } // 对应 cms_client 的 Id
        public string pre_cif { get; set; }
        public string Cif { get; set; }
        public string pre_ename { get; set; }
        public string Ename { get; set; }
        public string pre_cname { get; set; }
        public string Cname { get; set; }
        public string pre_status { get; set; }
        public string PreStatusString
        {
            get
            {
                switch (pre_status) // Assuming Status is a variable or property of an enum type
                {
                    case "A":
                        return "Active";
                    case "S":
                        return "Suspended";
                    default:
                        return "";
                }
            }
        }

        public string Status { get; set; }
        public string StatusString
        {
            get
            {
                switch (Status) // Assuming Status is a variable or property of an enum type
                {
                    case "A":
                        return "Active";
                    case "S":
                        return "Suspended";
                    default:
                        return "";
                }
            }
        }

        public string pre_nature { get; set; }
        public string PreNatureString
        {
            get
            {
                switch (pre_nature)
                {
                    case "B":
                        return "Broker";
                    case "C":
                        return "Client";
                    default:
                        return "";
                }
            }
        }

        public string Nature { get; set; }
        public string NatureString
        {
            get
            {
                switch (Nature)
                {
                    case "B":
                        return "Broker";
                    case "C":
                        return "Client";
                    default:
                        return "";
                }
            }
        }

        public string pre_category { get; set; }
        public string PreCategoryString
        {
            get
            {
                switch (pre_category) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "Company";
                    case "2":
                        return "Individual";
                    case "3":
                        return "Joint";
                    default:
                        return "";
                }
            }
        }

        public string Category { get; set; }
        public string CategoryString
        {
            get
            {
                switch (Category) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "Company";
                    case "2":
                        return "Individual";
                    case "3":
                        return "Joint";
                    default:
                        return "";
                }
            }
        }

        public string pre_fullname { get; set; }
        public string pre_Firstname { get; set; }
        public string pre_Surname { get; set; }

        public string Fullname { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }

        public string Pre_Tin { get; set; }
        public string Tin { get; set; }

        public string Pre_Gender { get; set; }
        public string PreGenderString
        {
            get
            {
                switch (Pre_Gender) // Assuming Status is a variable or property of an enum type
                {
                    case "M":
                        return "Male";
                    case "F":
                        return "Female";
                    default:
                        return "";
                }
            }
        }

        public string Gender { get; set; }

        public string GenderString
        {
            get
            {
                switch (Gender) // Assuming Status is a variable or property of an enum type
                {
                    case "M":
                        return "Male";
                    case "F":
                        return "Female";
                    default:
                        return "";
                }
            }
        }

        public string Pre_Nationality { get; set; }
        public string Nationality { get; set; }

        public string pre_Occupation { get; set; }
        public string Occupation { get; set; }

        public string pre_Employer { get; set; }
        public string Employer { get; set; }

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
