class Rectangle : Shape, IEllipse
{
    protected double a;
    protected double b;

    public Rectangle(double a, double b)
    {
        this.a = a;
        this.b = b;
    }

    public override double Perimeter()
    {
        return 2 * a + 2 * b;
    }

    public override double Area()
    {
        return a * b;
    }

    public bool IsEllipse(){ return false; }
}