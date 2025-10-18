public class Triangle<T> where T : struct, IComparable<T>
{
    private T a { get; set; }
    private T b { get; set; }
    private T c { get; set; }

    private Triangle(T a, T b, T c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public static bool GetInstance(T a, T b, T c, out Triangle<T>? triangle)
    {
        triangle = null;

        if (typeof(T) != typeof(int) && typeof(T) != typeof(float))
        {
            return false;
        }

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        double dc = Convert.ToDouble(c);

        bool isTriangle = (da + db > dc) && (da + dc > db) && (db + dc > da);
        if (isTriangle)
        {
            triangle = new Triangle<T>(a, b, c);
            return true;
        }

        return false;
    }
}