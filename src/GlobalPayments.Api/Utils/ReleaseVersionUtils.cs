using System;
using System.Reflection;

namespace GlobalPayments.Api.Utils {
    public static class ReleaseVersionUtils {
        public static string GetReleaseVersion() {
            if (string.Equals(System.Environment.GetEnvironmentVariable("SDK_TESTING"), "true", StringComparison.OrdinalIgnoreCase)) {
                return string.Empty;
            }
            try {
                return Assembly.Load(new AssemblyName("GlobalPayments.Api"))?.GetName()?.Version?.ToString() ?? string.Empty;
            }
            catch {
                return string.Empty;
            }
        }
    }
}
