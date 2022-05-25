using System.ComponentModel;

namespace MvvmCross.ExpandableRecyclerView.Core
{
    /// <summary>
    /// Interface exposing properties used for MvxExpandableRecyclerView.
    /// </summary>
    public interface ITaskItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The header to group items by.
        /// </summary>
        object Header { get; set; }

        /// <summary>
        /// The underlying data that will be displayed.
        /// </summary>
        object Model { get; }

        /// <summary>
        /// The position of the item within a header.
        /// </summary>
        int? Sequence { get; set; }

        /// <summary>
        /// Used to identify if item has been selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Used to identify if item has been selected as part of a multi-select process.
        /// </summary>
        bool IsHighlighted { get; set; }
    }
}
