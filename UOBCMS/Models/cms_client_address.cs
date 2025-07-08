using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_client_address
    {
        [Key]
        public int Id { get; set; }
        public int Client_id { get; set; }
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
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postal_code { get; set; }
        public string Zip { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual ICollection<cms_account_address> Cms_account_addresses { get; set; } = new List<cms_account_address>();        
    }
}
