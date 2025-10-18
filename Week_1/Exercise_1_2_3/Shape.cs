abstract class Shape
{
    private int _color = unchecked((int)0xFF0000FF); // Default to blue

    public int color
    {
        get
        {
            switch (_color)
            {
                case unchecked((int)0xFF0000FF): // ARGB: A=255, R=0, G=0, B=255
                    return Colors.Blue;

                case unchecked((int)0xFFFF0000): // ARGB: A=255, R=255, G=0, B=0
                    return Colors.Red;

                case unchecked((int)0xFF00FF00): // ARGB: A=255, R=0, G=255, B=0
                    return Colors.Green;

                default:
                    Console.WriteLine("Invalid value");
                    return 0;
            }
        }

        set
        {
            if (value == Colors.Blue)
            {
                _color = unchecked((int)0xFF0000FF); // ARGB: A=255, R=0, G=0, B=255
            }
            else if (value == Colors.Red)
            {
                _color = unchecked((int)0xFFFF0000); // ARGB: A=255, R=255, G=0, B=0
            }
            else if (value == Colors.Green)
            {
                _color = unchecked((int)0xFF00FF00); // ARGB: A=255, R=0, G=255, B=0
            }
            else
            {
                Console.WriteLine("Invalid value");
            }
        }
    }

    public class Colors
    {
        public static int Blue = 1;
        public static int Red = 2;
        public static int Green = 3;
    }

    public abstract double Perimeter();
    public abstract double Area();
}