using System.ComponentModel;

namespace MvvmCross.ExpandableRecyclerView.Core
{
    /// <summary>
    /// Interface exposing properties used for MvxExpandableRecyclerView.
    /// </summary>
    public interface ITaskItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The underlying data that will be displayed.
        /// </summary>
        object Model { get; }

        /// <summary>
        /// The property to group <see cref="Model"/> by.
        /// </summary>
        object Header { get; set; }

        /// <summary>
        /// Used to identify if item has been selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Used to identify if item has been selected as part of a multi-select process.
        /// </summary>
        bool IsHighlighted { get; set; }

        /// <summary>
        /// The position of the item within the header.
        /// </summary>
        int? Sequence { get; set; }
    }

    /// <summary>
    /// Interface exposing properties used for MvxExpandableRecyclerView.
    /// </summary>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="THeader">Type of header.</typeparam>
    public interface ITaskItem<out TModel, THeader> : ITaskItem
    {
        /// <summary>
        /// The underlying data that will be displayed.
        /// </summary>
        new TModel Model { get; }

        /// <summary>
        /// The property to group <see cref="Model"/> by.
        /// </summary>
        new THeader Header { get; set; }
    }
}
