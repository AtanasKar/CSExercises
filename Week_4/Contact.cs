using System;

public class Contact
{
    private string? name;   // <-- FIX: nullable field

    public string Name
    {
        get => name!;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                name = GenerateRandomName();
            else
                name = value;
        }
    }

    public Contact(string? name)
    {
        // Ensure we never pass a null value into the non-nullable Name property
        Name = name ?? string.Empty;
    }

    private string GenerateRandomName()
    {
        Random rnd = new Random();
        int number = rnd.Next(10000, 99999);
        return $"user{number}";
    }

    public override string ToString() => Name;
}
