ChatRoom room = new ChatRoom("General");

while (true)
{
    Console.WriteLine("\n=== MENU ===");
    Console.WriteLine("1. Add contact");
    Console.WriteLine("2. Send message");
    Console.WriteLine("3. Show chat");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");
    string? input = Console.ReadLine();

    switch (input)
    {
        case "1":
            Console.Write("Enter contact name (or leave empty): ");
            string? name = Console.ReadLine();
            room.AddContactWithPatternMatching(name);
            break;

        case "2":
            if (room.Participants.Count == 0)
            {
                Console.WriteLine("No participants in the room.");
                break;
            }

            Console.WriteLine("Choose author: ");
            for (int i = 0; i < room.Participants.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {room.Participants[i].Name}");
            }

            Console.Write("Author number: ");
            string? authorInput = Console.ReadLine();
            if (!int.TryParse(authorInput, out int authorNumber) ||
                authorNumber < 1 ||
                authorNumber > room.Participants.Count)
            {
                Console.WriteLine("Invalid author selection.");
                break;
            }

            Contact author = room.Participants[authorNumber - 1];

            Console.Write("Message: ");
            string? msg = Console.ReadLine();

            room.AddMessageWithPatternMatching(author, msg);
            break;

        case "3":
            room.ShowChat();
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Unknown option!");
            break;
    }
}
