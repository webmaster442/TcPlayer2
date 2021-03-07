namespace TcPlayer.Network
{
    internal static class Extensions
    {
        public static string ChopOffBefore(this string s, string Before)
        {
            int End = s.IndexOf(Before, System.StringComparison.OrdinalIgnoreCase);
            if (End > -1)
            {
                return s[(End + Before.Length)..];
            }
            return s;
        }


        public static string ChopOffAfter(this string s, string After)
        {
            int End = s.IndexOf(After, System.StringComparison.OrdinalIgnoreCase);
            if (End > -1)
            {
                return s.Substring(0, End);
            }
            return s;
        }

    }
}
