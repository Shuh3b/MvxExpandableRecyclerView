using AndroidX.RecyclerView.Widget;
using MvvmCross.ExpandableRecyclerView.Core;
using System.Collections.Generic;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// Interface exposing methods used in <see cref="StickyHeaderLayoutManager"/> to display a sticky header on top of <see cref="MvxExpandableRecyclerBaseView"/>.
    /// </summary>
    public interface IStickyHeaderHelper
    {
        /// <summary>
        /// The number of items in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// Determines if an item is currently being dragged.
        /// </summary>
        bool IsDragging { get; }

        /// <summary>
        /// Get item view type of item at specified position.
        /// </summary>
        /// <param name="position">Position of item.</param>
        /// <returns>Id of item view type.</returns>
        int GetItemViewType(int position);

        /// <summary>
        /// Get all headers in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
        /// </summary>
        /// <returns>Get a list of headers.</returns>
        IList<ITaskHeader> GetHeaders();

        /// <summary>
        /// Get header associated with task item.
        /// </summary>
        /// <param name="item">Item to get header information from.</param>
        /// <returns>Header matching item's header.</returns>
        ITaskHeader GetHeader(ITaskItem item);

        /// <summary>
        /// Get header associated with item at position.
        /// </summary>
        /// <param name="position">Position of item to use to find header.</param>
        /// <returns>Header associated with the item.</returns>
        ITaskHeader GetHeaderAt(int position);

        /// <summary>
        /// Get position of header associated with item at position.
        /// </summary>
        /// <param name="position">Position of item to use to find header position.</param>
        /// <returns>Position of header.</returns>
        int GetHeaderPosition(int position);

        /// <summary>
        /// Check if item at positon is a header.
        /// </summary>
        /// <param name="position">Position of item.</param>
        /// <returns><c>true</c> if item is header. Otherwise <c>false</c>.</returns>
        bool IsHeader(int position);

        /// <summary>
        /// Action for clicking on header.
        /// </summary>
        /// <param name="header">Header that was clicked.</param>
        /// <param name="suppress">Should suppress notify methods for <see cref="RecyclerView"/>.</param>
        void OnHeaderClick(ITaskHeader header, bool suppress = false);
    }
}
