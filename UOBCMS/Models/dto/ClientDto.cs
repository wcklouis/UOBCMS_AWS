namespace UOBCMS.Models.dto
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Cif { get; set; }
        public string Ename { get; set; }
        public string Cname { get; set; }
        public string Status { get; set; }
        public string StatusString
        {
            get
            {
                switch (Status) // Assuming Status is a variable or property of an enum type
                {
                    case "A":
                        return "Active";
                    case "S":
                        return "Suspended";
                    default:
                        return "";
                }
            }
        }

        public string Nature { get; set; }

        public string NatureString
        {
            get
            {
                switch (Nature)
                {
                    case "B":
                        return "Broker";
                    case "C":
                        return "Client";
                    default:
                        return "";
                }
            }
        }

        public string Category { get; set; }

        public string CategoryString
        {
            get
            {
                switch (Category) // Assuming Status is a variable or property of an enum type
                {
                    case "1":
                        return "Company";
                    case "2":
                        return "Individual";
                    case "3":
                        return "Joint";
                    default:
                        return "";
                }
            }
        }

        public string Fullname { get; set; }

        public string Firstname { get; set; }

        public string Surname { get; set; }

        public string Tin { get; set; }

        public string Gender { get; set; }

        public string GenderString
        {
            get
            {
                switch (Gender) // Assuming Status is a variable or property of an enum type
                {
                    case "M":
                        return "Male";
                    case "F":
                        return "Female";
                    default:
                        return "";
                }
            }
        }

        public string Nationality { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }

        public List<ClientAccountDto> ClientAccounts { get; set; }


    }
}
