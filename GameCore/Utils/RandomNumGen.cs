using System;

namespace GameCore.Utils
{
    public static class RandomNumGen
    {
        private static int seed = Environment.TickCount;
        private static Random ran = new Random(seed);


        public static Random Ran
        {
            get { return ran; }
        }

        public static int Seed
        {
            get { return seed; }
        }

        public static void SetSeed(int aSeed)
        {
            seed = aSeed;
            ran = new Random(aSeed);
        }
    }
}