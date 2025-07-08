using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UOBCMS.Models
{
    public class cms_client_account
    {
        [Key]
        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }
        [Key]
        public int Acc_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        // Navigation properties

        public virtual cms_client Cms_client { get; set; }

        [JsonIgnore] // Prevent circular reference
        public virtual cms_account Cms_account { get; set; }
    }
}
