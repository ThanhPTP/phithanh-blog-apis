namespace PhiThanh.Core.Utils
{
    public static class RandomUtils
    {
        public static int GetRndNumber(int range)
        {
            Random rnd = new();
            int number = rnd.Next(range);
            return number;
        }
    }
}
