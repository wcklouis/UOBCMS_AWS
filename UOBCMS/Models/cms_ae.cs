using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_ae
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public int Branch_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        //public virtual cms_account Cms_account { get; set; }
        public virtual ICollection<cms_account> Cms_accounts { get; set; } = new List<cms_account>();

        public virtual cms_branch Cms_branch { get; set; }
    }
}
