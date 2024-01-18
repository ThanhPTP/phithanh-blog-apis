namespace PhiThanh.Core.Utils
{
    public static class StringUtils
    {
        public static string RandomString(int length)
        {
            Random random = new();
            return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length)
                               select s[random.Next(s.Length)]).ToArray());
        }
    }
}
