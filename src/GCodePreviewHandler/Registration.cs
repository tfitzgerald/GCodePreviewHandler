using System.Runtime.InteropServices;

namespace GCodePreviewHandler
{
    public static class Registration
    {
        [ComRegisterFunction]
        public static void Register(Type t) { }

        [ComUnregisterFunction]
        public static void Unregister(Type t) { }
    }
}
