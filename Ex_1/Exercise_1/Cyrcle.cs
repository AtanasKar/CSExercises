class Cyrcle : Shape, IEllipse
{
    public Cyrcle(double r)
    {
        this.r = r;
    }

    public override double Perimeter()
    {
        return 2 * Math.PI * r;
    }

    public override double Area()
    {
        return Math.PI * r * r;
    }

    public bool IsEllipse() { return true; }

    private double r;
}