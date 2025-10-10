class Square : Rectangle
{
    public Square(double side) : base(side, side) {}

    public override double Perimeter()
    {
        return 4 * a;
    }

    public override double Area()
    {
        return a * a;
    }

    public static double Area(double a)
    {
        return a * a;
    }
}