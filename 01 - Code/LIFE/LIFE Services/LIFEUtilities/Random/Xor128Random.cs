namespace MCastAddressGenerator
{
    public class Xor128Random {
        private static int x;
        private static int y;
        private static int z;
        private static int w;

        public Xor128Random() {
            x = 12312455;
            y = 7365232;
            z = 189318923;
            w = 978356785;
        }

        public Xor128Random(int seed) {
            x = seed;
            y = seed;
            z = seed;
            w = seed;
        }

        public int Next()
        {
            int t;

            t = x ^ (x << 11);
            x = y; y = z; z = w;
            return w = w ^ (w >> 19) ^ (t ^ (t >> 8));
        }

        public int Next(int range) {
            return range & Next();
        }
    }
}
