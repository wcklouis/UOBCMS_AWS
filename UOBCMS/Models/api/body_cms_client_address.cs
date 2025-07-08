namespace UOBCMS.Models.api
{
    public class body_cms_client_address
    {
        public int Id { get; set; }
        public int Acc_id { get; set; }

        public string Accno { get; set; }
        public string Type { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postal_code { get; set; }
        public string Zip { get; set; }

        public bool applyToAllAcc { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
    }
}
