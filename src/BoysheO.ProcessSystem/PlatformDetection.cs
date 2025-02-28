using System.Runtime.InteropServices;

namespace WorkFlowUtil
{
    /// <summary>
    /// This source file is shared by both Grpc.Core and Grpc.Tools to avoid duplication
    /// of platform detection code.
    /// </summary>
    public static class PlatformDetection
    {
        public enum OSKind
        {
            Unknown,
            Windows,
            Linux,
            MacOSX
        };

        public enum CpuArchitecture
        {
            Unknown,
            X86,
            X64,
            Arm64
        };

        public static OSKind GetOSKind()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSKind.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSKind.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSKind.MacOSX;
            }
            else
            {
                return OSKind.Unknown;
            }
        }

        public static CpuArchitecture GetProcessArchitecture()
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    return CpuArchitecture.X86;
                case Architecture.X64:
                    return CpuArchitecture.X64;
                case Architecture.Arm64:
                    return CpuArchitecture.Arm64;
                // We do not support other architectures,
                // so we simply return "unrecognized".
                default:
                    return CpuArchitecture.Unknown;
            }
        }
    }
}