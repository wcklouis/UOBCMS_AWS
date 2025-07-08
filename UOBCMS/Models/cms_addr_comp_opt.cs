using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_addr_comp_opt
    {        
        [Key]
        [Column(Order = 0)]
        public string Country { get; set; }
        [Key]
        [Column(Order = 1)]
        public string ProvinceEn { get; set; }
        [Key]
        [Column(Order = 2)]
        public string StateEn { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CityEn { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DistrictEn { get; set; }
        public string ProvinceZh { get; set; }
        public string StateZh { get; set; }
        public string CityZh { get; set; }
        public string DistrictZh { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
    }
}
