using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models.dto
{
    public class ClientEmailDto
    {
        public int Id { get; set; }
        public int Client_id { get; set; }
        public string Type { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case "0":
                        return "Primary Email Address 1";
                    case "1":
                        return "Alternative Email Address 1";
                    case "2":
                        return "Alternative Email Address 2";
                    case "3":
                        return "Primary Email Address 2";
                    case "4":
                        return "Primary Email Address 3";
                    case "5":
                        return "Primary Email Address 4";
                    case "6":
                        return "Primary Email Address 5";
                    default:
                        return "";
                }
            }
        }
        public string Email { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
    }
}
