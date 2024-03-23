namespace Hasher
{
    public static class JavascriptEscape
    {
        public static string Escape(string text)
        {
            return text
                .Replace("\"", "\\\"")
                .Replace("'", "\\'")
                .Replace("\\", "\\\\");
        }
    }
}
