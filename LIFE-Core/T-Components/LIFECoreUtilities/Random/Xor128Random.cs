namespace LIFE.Components.Utilities.Random
{
    public class Xor128Random
    {
        private static int _x;
        private static int _y;
        private static int _z;
        private static int _w;

        public Xor128Random()
        {
            _x = 12312455;
            _y = 7365232;
            _z = 189318923;
            _w = 978356785;
        }

        public Xor128Random(int seed)
        {
            _x = seed;
            _y = seed;
            _z = seed;
            _w = seed;
        }

        public int Next()
        {
            int t;

            t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return _w = _w ^ (_w >> 19) ^ t ^ (t >> 8);
        }

        public int Next(int range)
        {
            return range & Next();
        }
    }
}