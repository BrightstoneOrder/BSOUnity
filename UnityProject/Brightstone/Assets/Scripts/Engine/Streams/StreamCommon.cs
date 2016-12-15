namespace Brightstone
{
    public static class StreamCommon
    {
        public const string DATA_EXTENSION = ".bsd";

        // Header Types:
        public const int HT_HEIGHTMAP = 1;


        public static void InvalidHeader(int expected, int got, string msg)
        {
            Log.Sys.Error("Invalid Header, got " + got + ", expected " + expected + ": " + msg);
        }

        public static void InvalidVersion(int expected, int got, string msg)
        {
            Log.Sys.Error("Invalid Version, got " + got + ", expected " + expected + ": " + msg);
        }
    }
}