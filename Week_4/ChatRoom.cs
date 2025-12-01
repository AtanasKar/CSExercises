using System;
using System.Collections.Generic;
using System.Linq;

public class ChatRoom
{
    public string Name { get; set; }
    public List<Contact> Participants { get; set; } = new List<Contact>();
    public List<Message> Messages { get; set; } = new List<Message>();

    public ChatRoom(string name)
    {
        Name = name;
    }

    public (string userName, int messageCount, string shortestMessage) GetStatistics()
    {
        if (Messages.Count == 0)
            return ("No messages", 0, "N/A");

        // Count messages per user
        var grouped = Messages
            .GroupBy(m => m.Author.Name)
            .Select(g => new
            {
                UserName = g.Key,
                Count = g.Count(),
                ShortestMsg = g.OrderBy(m => m.Text.Length).First().Text
            })
            .OrderByDescending(x => x.Count)
            .First();

        return (grouped.UserName, grouped.Count, grouped.ShortestMsg);
    }


    public void AddUser(Contact user)
    {
        if (!Participants.Contains(user))
            Participants.Add(user);
    }

    public void SendMessage(Contact author, string text)
    {
        if (!Participants.Contains(author))
        {
            Console.WriteLine($"User {author.Name} is not in the chat room!");
            return;
        }

        Messages.Add(new Message(author, text));
    }

    public void ShowChat()
    {
        Console.WriteLine($"\n--- Chat: {Name} ---");
        foreach (var msg in Messages)
        {
            Console.WriteLine(msg);
        }
        Console.WriteLine("----------------------\n");
    }

    public void AddContactWithPatternMatching(string? name)
    {
        Contact newContact = name switch
        {
            null => new Contact(null),                         // name is null → auto-generated
            "" => new Contact(null),                           // empty → auto-generated
            string s when s.Length < 3 => new Contact(null),   // too short → auto-generated
            string s => new Contact(s),                        // valid name
        };

        Participants.Add(newContact);
        Console.WriteLine($"Added contact: {newContact.Name}");
    }

    public void AddMessageWithPatternMatching(Contact author, string? text)
    {
        switch (text)
        {
            case null:
                Console.WriteLine("Cannot send an empty message.");
                return;

            case string msg when msg.Trim().Length == 0:
                Console.WriteLine("Message contains only whitespace.");
                return;

            case string msg when msg.Length > 200:
                Console.WriteLine("Message is too long (over 200 characters).");
                return;

            case string msg:
                Messages.Add(new Message(author, msg));
                Console.WriteLine("Message sent.");
                break;
        }
    }

}
