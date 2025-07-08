using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_branch
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual ICollection<cms_ae> Cms_aes { get; set; } = new List<cms_ae>();
    }
}
