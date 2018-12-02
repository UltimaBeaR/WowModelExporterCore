using ShellFileDialogs;
using System;

namespace WowModelExporterUnityPlugin
{
    public static class WowVrcFileDialogs
    {
        public static string Open()
        {
            return FileOpenDialog.ShowSingleSelectDialog(IntPtr.Zero, "Open .wowvrc file", null, null, _filters, 0);
        }

        public static string Save()
        {
            return FileSaveDialog.ShowDialog(IntPtr.Zero, "Save .wowvrc file", null, null, _filters, 0);
        }

        private static readonly Filter[] _filters = new[] { new Filter("wow -> vrc file", "wowvrc") };
    }
}
