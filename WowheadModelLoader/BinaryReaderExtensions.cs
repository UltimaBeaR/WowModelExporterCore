using System.IO;

namespace WowheadModelLoader
{
    public static class BinaryReaderExtensions
    {
        // аналог ZamModelViewer.DataView.getBool()
        public static bool GetBool(this BinaryReader r)
        {
            return r.ReadByte() != 0;
        }

        // аналог ZamModelViewer.DataView.getString()
        public static string GetString(this BinaryReader r)
        {
            var size = r.ReadUInt16();
            return new string(r.ReadChars(size));
        }
    }
}
