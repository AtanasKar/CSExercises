using System;

public class Message
{
    public Contact Author { get; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; }
    public bool IsEdited { get; private set; }

    public Message(Contact author, string text)
    {
        Author = author;
        Text = text;
        CreatedAt = DateTime.Now;
        IsEdited = false;
    }

    public void EditMessage(string newText)
    {
        if (newText != Text)
        {
            Text = newText;
            IsEdited = true;
        }
    }

    // Deconstruct #1: Author + Text
    public void Deconstruct(out Contact author, out string text)
    {
        author = Author;
        text = Text;
    }

    // Deconstruct #2: Author + Text + CreatedAt
    public void Deconstruct(out Contact author, out string text, out DateTime createdAt)
    {
        author = Author;
        text = Text;
        createdAt = CreatedAt;
    }

    // Deconstruct #3: Date + Time (separate)
    public void Deconstruct(out DateTime date, out TimeSpan time)
    {
        date = CreatedAt.Date;
        time = CreatedAt.TimeOfDay;
    }

    public override string ToString()
    {
        return $"[{CreatedAt}] {Author}: {Text}" + (IsEdited ? " (edited)" : "");
    }
}
