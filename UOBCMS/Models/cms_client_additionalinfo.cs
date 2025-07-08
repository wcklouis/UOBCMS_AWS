using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_client_additionalinfo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }

        [ForeignKey("Cms_additionalinfo")] // This tells EF to use Client_id as the FK
        public int Addinfo_id { get; set; }
        public string Value { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual cms_additionalinfo Cms_additionalinfo { get; set; }
    }
}
