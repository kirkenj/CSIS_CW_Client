using System.Drawing;

namespace CSIS_CW_Client
{
    class ClientCircle
    {
        public ClientCircle()
        {
            this.X = 0;
            this.Y = 0;
            this.LifeTime = 0;
            this.RadiusPix = 0;
        }
        public ClientCircle(int x, int y, int lifetimeMilisec, int radius)
        {
            this.X = x;
            this.Y = y;
            this.LifeTime = lifetimeMilisec;
            this.RadiusPix = radius;
        }
        public ClientCircle(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.LifeTime = 1000;
            this.RadiusPix = 40;
        }
        public ClientCircle(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.LifeTime = 1000;
            this.RadiusPix = 40;
        }
        public ClientCircle(Point point, int lifetimeMilisec, int radius)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.LifeTime = lifetimeMilisec;
            this.RadiusPix = radius;
        }
        public int X { get; }
        public int Y { get; }
        public int LifeTime { get; set; }
        public int RadiusPix { get; }
        public bool Show
        {
            get
            {
                return (LifeTime > 0);
            }
        }
        public Point Point
        {
            get
            {
                return new Point(X, Y);
            }
        }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(this.X, this.Y, this.RadiusPix, this.RadiusPix);
            }
        }
        public static ClientCircle Parse(string str)
        {
            string buff = str.Remove(0, 1);
            buff = buff.Remove(buff.Length - 1, 1);
            string[] m = buff.Split(',');
            return new ClientCircle(
                new Point(int.Parse(m[0]), int.Parse(m[1])),
                int.Parse(m[2]),
                int.Parse(m[3])
                );
        }
        public bool InterrectsWithPoint(Point point)
        {
            return this.Rectangle.IntersectsWith(new Rectangle(point, Size.Empty));
        }
        new public string ToString()
        {
            return '{' + $"{X},{Y},{LifeTime},{RadiusPix}" + '}';
        }
    }
}
