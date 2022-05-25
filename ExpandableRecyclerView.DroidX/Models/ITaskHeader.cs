using MvvmCross.ExpandableRecyclerView.Core;
using System.Collections.ObjectModel;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Interface exposing header properties used in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
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
        bool IsCollapsed { get; set; }

        /// <summary>
        /// The items under this header.
        /// </summary>
        ObservableCollection<ITaskItem> Items { get; set; }

        /// <summary>
        /// The number of items under this header.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Custom rules for header.
        /// </summary>
        TaskHeaderRule Rules { get; set; }
    }
}