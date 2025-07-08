using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_client_north_bound
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")]
        public int Client_id { get; set; }

        public string Chi_name { get; set; }

        [ForeignKey("Cms_client_id")]
        public int Id_id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }

        public virtual cms_client_id Cms_client_id { get; set; }
    }
}
