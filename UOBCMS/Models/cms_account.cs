using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account
    {
        [Key]
        public int Id { get; set; }
        public string AccNo { get; set; }
        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "C":
                        return "Cash";
                    case "M":
                        return "Margin";
                    case "E":
                        return "CIES";
                    case "S":
                        return "Stock Option";
                    case "P":
                        return "Monthly Saving Plan";
                    case "F":
                        return "Future";
                    default:
                        return "";
                }
            }
        }
        public string Ename { get; set; }
        public string Cname { get; set; }
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

        public int Ae_id { get; set; }
        public string Aecode { get; set; }
        public DateTime Open_dt { get; set; }
        public DateTime? Close_dt { get; set; }
        public DateTime? Credit_exp_dt { get; set; }

        public DateTime? Inactive_dt { get; set; }
        public string? Inactive_reason { get; set; }
        public int Daily_stat_copies { get; set; }
        public int Monthly_stat_copies { get; set; }
        public int Contract_note_copies { get; set; }

        public string? Remark { get; set; }
        public string? Class { get; set; }
        public string Cob_mode { get; set; }

        public string Chqname { get; set; }

        public string Cob_modeString
        {
            get
            {
                switch (Cob_mode) // Assuming Status is a variable or property of an enum type
                {
                    case "0":
                        return "On-line";
                    case "1":
                        return "E-cert";
                    case "2":
                        return "Face to Face";
                    default:
                        return "";
                }
            }
        }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        // Navigation property for the many-to-many relationship

        public virtual cms_client_account Cms_client_account { get; set; }

        public virtual ICollection<cms_account_address> Cms_account_addresses { get; set; } = new List<cms_account_address>();

        public virtual ICollection<cms_account_email> Cms_account_emails { get; set; } = new List<cms_account_email>();

        public virtual ICollection<cms_account_phone> Cms_account_phones { get; set; } = new List<cms_account_phone>();

        public virtual ICollection<cms_account_bank> Cms_account_banks { get; set; } = new List<cms_account_bank>();

        public virtual ICollection<cms_account_doc> Cms_account_docs { get; set; } = new List<cms_account_doc>();



        public virtual ICollection<cms_account_mthsaving_plan> Cms_account_mthsaving_plans { get; set; } = new List<cms_account_mthsaving_plan>();

        public virtual ICollection<cms_account_additionalinfo> Cms_account_additionalinfos { get; set; } = new List<cms_account_additionalinfo>();

        public virtual ICollection<cms_account_limit> Cms_account_limits { get; set; } = new List<cms_account_limit>();

        public virtual ICollection<cms_account_market> Cms_account_markets { get; set; } = new List<cms_account_market>();

        public virtual ICollection<cms_account_social_media> Cms_account_social_medias { get; set; } = new List<cms_account_social_media>();

        public virtual ICollection<cms_account_controlling_person> Cms_account_controlling_persons { get; set; } = new List<cms_account_controlling_person>();
        public virtual ICollection<cms_account_auth_party> Cms_account_auth_parties { get; set; } = new List<cms_account_auth_party>();

        public virtual cms_account_w8 Cms_account_w8 { get; set; } // Navigation property

        public virtual cms_ae Cms_ae { get; set; } // Navigation property

        public virtual cms_account_dkq Cms_account_dkq { get; set; } // Navigation property

        public virtual cms_account_hkidr Cms_account_hkidr { get; set; } // Navigation property

        public virtual cms_account_quoteservice Cms_account_quoteservice { get; set; } // Navigation property

        public virtual cms_account_north_bound Cms_account_north_bound { get; set; } // Navigation property

        public virtual cms_account_company_structure Cms_account_company_structure { get; set; } // Navigation property
    }
}
