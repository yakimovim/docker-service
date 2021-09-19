namespace Service.Utilities
{
    public class BytesUtilities
    {
        public static string Simplify(long sizeInBytes)
        {
            var suffixes = new[] { "KB", "MB", "GB", "TB" };

            var result = sizeInBytes.ToString();

            var size = (double)sizeInBytes;

            for (int i = 0; i < suffixes.Length; i++)
            {
                if (size < 1024.0) break;

                size = size / 1024.0;

                result = size.ToString("0.00") + " " + suffixes[i];
            }

            return result;
        }
    }
}
