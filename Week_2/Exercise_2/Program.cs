using System.Text.RegularExpressions;
using System.Xml;

namespace Exercise_2
{

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string inputFile = "input-02.txt";
                string content = File.ReadAllText(inputFile);

                ContactManager contactManager = ParseContacts(content);

                GenerateXmlFile(contactManager, "contacts.xml");

                Console.WriteLine("Contacts processed successfully!");
                Console.WriteLine($"Total contacts: {contactManager.GetAllContacts().Count}");

                Console.WriteLine("\nDemonstrating unified lookup:");
                Console.WriteLine($"Contact by name 'Иван': {contactManager.GetContact("Иван")?.PhoneNumber}");
                Console.WriteLine($"Contact by ID '608310': {contactManager.GetContact("608310")?.PhoneNumber}");
                Console.WriteLine($"Contact by name 'Георги': {contactManager.GetContact("Георги")?.PhoneNumber}");
                Console.WriteLine($"Contact by ID '706038': {contactManager.GetContact("706038")?.PhoneNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static ContactManager ParseContacts(string content)
        {
            ContactManager contactManager = new ContactManager();

            Regex phoneRegex = new Regex(@"\+395\s*\d{3}\s*\d{2}\s*\d{2}");
            Regex idRegex = new Regex(@"\b\d{6}\b");
            Regex nameRegex = new Regex(@"[А-Яа-я]+");

            var phoneMatches = phoneRegex.Matches(content);
            var idMatches = idRegex.Matches(content);
            var nameMatches = nameRegex.Matches(content);

            var phones = new List<string>();
            var ids = new List<string>();
            var names = new List<string>();

            foreach (Match match in phoneMatches)
            {
                phones.Add(match.Value.Replace(" ", ""));
            }

            foreach (Match match in idMatches)
            {
                ids.Add(match.Value);
            }

            foreach (Match match in nameMatches)
            {
                string name = match.Value.Trim();
                if (!string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }
            }

            int minCount = Math.Min(Math.Min(phones.Count, ids.Count), names.Count);
            
            for (int i = 0; i < minCount; i++)
            {
                Contact contact = new Contact(names[i], ids[i], phones[i]);
                contactManager.AddContact(contact);
            }

            return contactManager;
        }

        static void GenerateXmlFile(ContactManager contactManager, string fileName)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            XmlElement root = doc.CreateElement("Contacts");
            doc.AppendChild(root);

            foreach (Contact contact in contactManager.GetAllContacts())
            {
                XmlElement contactElement = doc.CreateElement("Contact");
                
                XmlElement nameElement = doc.CreateElement("Name");
                nameElement.InnerText = contact.Name;
                contactElement.AppendChild(nameElement);

                XmlElement idElement = doc.CreateElement("Id");
                idElement.InnerText = contact.Id;
                contactElement.AppendChild(idElement);

                XmlElement phoneElement = doc.CreateElement("PhoneNumber");
                phoneElement.InnerText = contact.PhoneNumber;
                contactElement.AppendChild(phoneElement);

                root.AppendChild(contactElement);
            }

            doc.Save(fileName);
        }
    }
}
