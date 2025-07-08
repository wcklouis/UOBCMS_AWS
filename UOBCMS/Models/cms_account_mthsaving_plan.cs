using System.ComponentModel.DataAnnotations;
using UOBCMS.Models.IBO;

namespace UOBCMS.Models
{
    public class cms_account_mthsaving_plan
    {
        [Key]
        public int Id { get; set; }
        public int Acc_id { get; set; }

        public DateTime Eff_start_dt { get; set; }
        public DateTime Eff_end_dt { get; set; }

        public string Mkt_code { get; set; }
        public string Sec_code { get; set; }

        public string Ccy {  get; set; }
        public decimal Invest_amt { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }

        //public virtual Instrument Inst { get; set; }
    }
}
