namespace Exercise_2
{
    public class ContactManager
    {
        private Dictionary<string, Contact> contacts;

        public ContactManager()
        {
            contacts = new Dictionary<string, Contact>();
        }

        public void AddContact(Contact contact)
        {
            contacts[contact.Name] = contact;
            contacts[contact.Id] = contact;
        }

        public Contact? GetContact(string nameOrId)
        {
            contacts.TryGetValue(nameOrId, out Contact? contact);
            return contact;
        }

        public Contact? GetContactByName(string name)
        {
            return GetContact(name);
        }

        public Contact? GetContactById(string id)
        {
            return GetContact(id);
        }

        public List<Contact> GetAllContacts()
        {
            return contacts.Values.Distinct().ToList();
        }
    }
}
