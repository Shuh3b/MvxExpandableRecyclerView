using AndroidX.RecyclerView.Widget;
using MvvmCross.ExpandableRecyclerView.Core;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Interface exposing methods used in <see cref="MvxExpandableRecyclerAdapter{THeader}"/> to help with item dragging, swiping, etc.
    /// </summary>
    public interface IItemTouchHelperCallback
    {
        /// <summary>
        /// Allows draggable functionality on <see cref="MvxExpandableRecyclerView"/>.
        /// </summary>
        /// <param name="fromViewHolder">View that's being dragged.</param>
        /// <param name="toViewHolder">View that's being dragged over.</param>
        void OnMove(RecyclerView.ViewHolder fromViewHolder, RecyclerView.ViewHolder toViewHolder);

        /// <summary>
        /// Executes the corresponding bound property when view is swiped towards start or end direction.
        /// </summary>
        /// <param name="viewHolder">View that was swiped.</param>
        /// <param name="direction">Whether view was swiped towards start or end direction.</param>
        /// <exception cref="ArgumentNullException">If any of the swipe-able commands are null.</exception>
        void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction);

        /// <summary>
        /// This executes when <see cref="MvxExpandableRecyclerView"/> has completed an action, i.e. dragging, swiped, etc.
        /// </summary>
        /// <param name="recyclerView"><see cref="MvxExpandableRecyclerView"/>.</param>
        /// <param name="viewHolder">The view the action was executed on.</param>
        void OnClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder);

        /// <summary>
        /// Get header.
        /// </summary>
        /// <param name="item">Item to get header information from.</param>
        /// <returns>Header matching item's header.</returns>
        ITaskHeader GetHeader(ITaskItem item);
    }
}