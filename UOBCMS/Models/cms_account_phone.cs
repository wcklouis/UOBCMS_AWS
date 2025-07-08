using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_phone
    {
        [Key]
        public int Id { get; set; }
        public int Client_phone_id { get; set; }
        public int Acc_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }

        public virtual cms_client_phone Cms_client_phone { get; set; }
    }
}
