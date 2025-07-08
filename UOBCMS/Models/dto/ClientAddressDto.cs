namespace UOBCMS.Models.dto
{
    public class ClientAddressDto
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
                        return "Residential";
                    case "1":
                        return "Correspondance";
                    default:
                        return "";
                }
            }
        }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postal_code { get; set; }
        public string Zip { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
    }
}
