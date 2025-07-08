using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace UOBCMS.Models
{
    public class cms_client
    {
        [Key]
        public int Id { get; set; }
        public string Cif { get; set; }
        public string Ename { get; set; }
        public string Cname { get; set; }
        public string Status { get; set; }

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
                        return "House";
                    case "4":
                        return "Staff";
                    case "5":
                        return "Joint";
                    case "6":
                        return "Instituational";
                    case "7":
                        return "Broker";
                    case "8":
                        return "Others";
                    default:
                        return "";
                }
            }
        }
        public string ResidentId { get; set; }
        public string Brno { get; set; }
        public string InternationalId { get; set; }
        public string Fullname { get; set; }
        public string Firstname { get; set; }

        public string Surname { get; set; }

        public string Middlename { get; set; }

        public string Jurisdiction_residence { get; set; }

        public string Tin { get; set; }

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

        public string Nationality { get; set; }

        public string Occupation { get; set; }
        public string Employer { get; set; }

        public DateTime Cob_dt { get; set; }

        public DateTime Init_dt { get; set; }

        public string Init_place { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        // Navigation property for the many-to-many relationship
        public virtual ICollection<cms_client_account> Cms_client_accounts { get; set; } = new List<cms_client_account>();
        public virtual ICollection<cms_client_bank> Cms_client_banks { get; set; } = new List<cms_client_bank>();

        public virtual ICollection<cms_client_phone> Cms_client_phones { get; set; } = new List<cms_client_phone>();

        public virtual ICollection<cms_client_id> Cms_client_ids { get; set; } = new List<cms_client_id>();

        public virtual cms_client_north_bound Cms_client_north_bound { get; set; } // Navigation property

        public virtual ICollection<cms_client_email> Cms_client_emails { get; set; } = new List<cms_client_email>();

        public virtual ICollection<cms_client_address> Cms_client_addresses { get; set; } = new List<cms_client_address>();

        public virtual ICollection<cms_virtual_client> Cms_child_clients { get; set; } = new List<cms_virtual_client>();

        public virtual ICollection<cms_virtual_client> Cms_virtual_clients { get; set; } = new List<cms_virtual_client>();

        public virtual ICollection<cms_client_additionalinfo> Cms_client_additionalinfos { get; set; } = new List<cms_client_additionalinfo>();

        public virtual ICollection<cms_client_related_staff> Cms_client_related_staffs { get; set; } = new List<cms_client_related_staff>();

        // Navigation property for addresses
        //public virtual ICollection<cms_account_address> Cms_account_addresses { get; set; } = new List<cms_account_address>();
    }
}
