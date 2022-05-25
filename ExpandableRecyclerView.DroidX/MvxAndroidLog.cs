using Microsoft.Extensions.Logging;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    internal static class MvxAndroidLog
    {
        internal static ILogger Instance => MvxLogHost.GetLog("ExpandableRecyclerView");
    }

    internal static class MvxLogHost
    {
        private static ILogger defaultLogger;

        public static ILogger Default => defaultLogger ??= GetLog("Default");

        public static ILogger<T> GetLog<T>()
        {
            if (Mvx.IoCProvider.TryResolve<ILoggerFactory>(out var loggerFactory))
                return loggerFactory.CreateLogger<T>();

            return null;
        }

        public static ILogger GetLog(string categoryName)
        {
            if (Mvx.IoCProvider.TryResolve<ILoggerFactory>(out var loggerFactory))
                return loggerFactory.CreateLogger(categoryName);

            return null;
        }
    }
}