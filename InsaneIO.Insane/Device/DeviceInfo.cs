using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using InsaneIO.Insane.Cryptography.Enums;

namespace InsaneIO.Insane.Device
{
    
    public static class DeviceInfo
    {
        private const string WINDOWS = "Windows";
        private const string LINUX = "Linux";
        private const string OSX = "macOS/OSX";
        private const string UNKNOWN = "Unknown";


        private static String GetRealDeviceId()
        {

            return "DeviceID".ComputeHashEncoded(Base64Encoder.DefaultInstance, HashAlgorithm.Sha256);
        }

        private static string _RealDeviceId = GetRealDeviceId();

        public static string RealDeviceId
        {
            get
            {
                return _RealDeviceId;
            }
        }

        public static string DeviceId
        {
            get
            {
                return RealDeviceId.ComputeHashEncoded(Base64Encoder.DefaultInstance, HashAlgorithm.Sha256);
            }
        }

        public static string Manufacturer
        {
            get
            {
                return nameof(Manufacturer);
            }
        }

        public static string DeviceNameOrModel
        {
            get
            {
                return nameof(DeviceNameOrModel);
            }
        }

        public static string OSDescription
        {
            get
            {
                return RuntimeInformation.OSDescription;
            }
        }

        public static string Platform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return WINDOWS;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return LINUX;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return OSX;
                }

                return UNKNOWN;
            }
        }

        public static String FriendlyName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public static String Architecture
        {
            get
            {
                return RuntimeInformation.OSArchitecture.ToString();

            }
        }

        public static string ApplicationName
        {
            get
            {
                return Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            }
        }

        public static string ApplicationVersion
        {
            get
            {
                return Assembly.GetEntryAssembly()?.GetCustomAttributes<AssemblyFileVersionAttribute>().FirstOrDefault()?.Version.ToString() ?? string.Empty;
            }
        }

        public static string ApplicationDescription
        {
            get
            {
                return Assembly.GetEntryAssembly()?.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;
            }
        }

        public static String Summary()
        {

            return $"DeviceId: {DeviceId}{Environment.NewLine}Real DeviceId: {RealDeviceId}{Environment.NewLine}Manufacturer: {Manufacturer}{Environment.NewLine}Device Name or Model: {DeviceNameOrModel}{Environment.NewLine}OS Description: {OSDescription}{Environment.NewLine}Platform: {Platform}{Environment.NewLine}Architecture: {Architecture}{Environment.NewLine}Friendly name: {FriendlyName}{Environment.NewLine}Application Name: {ApplicationName}{Environment.NewLine}Application Description: {ApplicationDescription}{Environment.NewLine}Application Version: {ApplicationVersion}";
        }
    }
}
