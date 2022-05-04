using Microsoft.Extensions.Logging;
using MvvmCross.Logging;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    internal static class MvxAndroidLog
    {
        internal static ILogger Instance => MvxLogHost.GetLog("ExpandableRecyclerView");
    }
}