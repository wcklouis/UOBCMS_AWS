using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.api
{
    public class body_cms_account_quoteservice
    {
        [Key]
        public int Id { get; set; }

        public string Accno { get; set; }

        public int Acc_id { get; set; }

        public DateTime End_dt { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
    }
}
