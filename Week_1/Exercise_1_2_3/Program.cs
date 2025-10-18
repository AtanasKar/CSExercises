class Program
{
    public static void Main()
    {
        Triangle<int>? triangle;
        if (Triangle<int>.GetInstance(5, 6, 7, out triangle))
        {
            Console.WriteLine("Triangle created successfully");
        }
        else
        {
            Console.WriteLine("Invalid triangle");
        }
    }
}