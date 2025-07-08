namespace UOBCMS.Models.dto
{
    public class ClientPhoneDto
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
                    case "1":
                        return "Home";
                    case "2":
                        return "Office";
                    case "3":
                        return "Mobile";
                    case "4":
                        return "Fax";
                    default:
                        return "";
                }
            }
        }

        public string Country_code { get; set; }
        public string Area_code { get; set; }
        public string Value { get; set; }
    }
}
