using MvvmCross.ExpandableRecyclerView.Core;
using System.Collections.Generic;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Interface exposing header properties used for MvxExpandableRecyclerView.
    /// </summary>
    public interface ITaskHeader : ITaskItem
    {
        /// <summary>
        /// Header name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether header is collapsed or expanded.
        /// </summary>
        bool IsCollapsed { get; }

        /// <summary>
        /// This will contain items under header, if header is collapsed.
        /// </summary>
        IList<ITaskItem> CollapsedItems { get; set; }

        /// <summary>
        /// Custom rules for header.
        /// </summary>
        TaskHeaderRule Rules { get; set; }
    }
}