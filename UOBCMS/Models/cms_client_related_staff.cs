using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models
{
    public class cms_client_related_staff
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cms_client")] // This tells EF to use Client_id as the FK
        public int Client_id { get; set; }
        public string Staff_name { get; set; }

        public int Relationship {  get; set; }

        public string RelationshipString
        {
            get
            {
                switch (Relationship) // Assuming Status is a variable or property of an enum type
                {
                    case 0:
                        return "Spouse";
                    default:
                        return "";
                }
            }
        }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_client Cms_client { get; set; }
    }
}
