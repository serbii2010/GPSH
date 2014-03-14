namespace modelirovanieKursach
{
    public static class func
    {
        public static double function(double x)
        {
            if (x<0 || x>9)
            {
                return 0;
            }
            if (x < 1)
            {
                return 5 * x;
            }
            if (x < 2)
            {
                return -4*(x-1) + 5 ;
            }
            if (x < 3)
            {
                return 5 * (x-2) +1;
            }
            if (x < 4)
            {
                return -5 * (x-3) + 6;
            }
            if (x < 5)
            {
                return 2*(x-4)+1;
            }
            if (x < 6)
            {
                return x-2;
            }
            if (x < 7)
            {
                return -3 * (x-6) + 4;
            }
            if (x < 8)
            {
                return 4 * (x-7)+1;
            }
            if (x < 9)
            {
                return -5 * (x-8) + 5;
            }
            return 0;
        }

        public static double integral(double x0, double x1, double dx)
        {
            dx *= 0.01;
            if (dx <= 0)
            {
                return 0;
            }
            double x = x0;
            double S = 0;
            while (x < x1)
            {
                S += function(x)*dx;
                x += dx;
            }
            return S;
        }
    }
}
