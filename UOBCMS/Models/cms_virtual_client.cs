using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_virtual_client
    {
        [Key]
        public int Id { get; set; }
        public int  Virtual_client_id { get; set; }
        public int Client_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_parent_client { get; set; }

        public virtual cms_client Cms_child_client { get; set; }

        //public virtual cms_client Cms_child_client { get; set; }

        //public virtual ICollection<cms_client> Cms_child_clients { get; set; } = new List<cms_client>();
    }
}
