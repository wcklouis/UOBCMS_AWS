using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_additionalinfo
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual ICollection<cms_account_additionalinfo> Cms_account_additionalinfos { get; set; } = new List<cms_account_additionalinfo>();
        public virtual ICollection<cms_client_additionalinfo> Cms_client_additionalinfos { get; set; } = new List<cms_client_additionalinfo>();
    }
}
