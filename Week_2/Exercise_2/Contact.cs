namespace Exercise_2
{
    public class Contact
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string PhoneNumber { get; set; }

        public Contact(string name, string id, string phoneNumber)
        {
            Name = name;
            Id = id;
            PhoneNumber = phoneNumber;
        }
    }
}
