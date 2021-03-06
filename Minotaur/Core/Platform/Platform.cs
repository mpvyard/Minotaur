﻿using System;
using System.Runtime.InteropServices;

namespace Minotaur.Core.Platform
{
    public static class Platform
    {
        public static readonly bool Is64Bits = IntPtr.Size == sizeof(long);

        public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static readonly bool IsMacOsX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static readonly bool IsPosix = IsLinux || IsMacOsX;
    }
    
    public interface IPlatform { }
    public struct Win32 : IPlatform { }
    public struct Posix : IPlatform { }
}
