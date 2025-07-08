namespace UOBCMS.Models.api
{
    public class body_cms_client_phone
    {
        public int Id { get; set; }
        public int Acc_id { get; set; }

        public string Accno { get; set; }

        public string Type { get; set; }

        public string Sub_type { get; set; }

        public string Country_code { get; set; }

        public string Area_code { get; set; }
        public string Value { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
    }
}
