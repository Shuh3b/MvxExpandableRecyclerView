using AndroidX.RecyclerView.Widget;
using MvvmCross.ExpandableRecyclerView.Core;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// Interface exposing methods used in <see cref="MvxExpandableRecyclerAdapter{THeader}"/> to help with item dragging, swiping, etc.
    /// </summary>
    public interface IItemTouchHelperCallback
    {
        /// <summary>
        /// Whether to enable RecyclerView dragging.
        /// </summary>
        bool EnableDrag { get; set; }

        /// <summary>
        /// Whether to enable RecyclerView swiping.
        /// </summary>
        bool EnableSwipe { get; set; }

        /// <summary>
        /// Allows draggable functionality on <see cref="MvxExpandableRecyclerBaseView"/>.
        /// </summary>
        /// <param name="fromViewHolder">View that's being dragged.</param>
        /// <param name="toViewHolder">View that's being dragged over.</param>
        bool OnMove(RecyclerView.ViewHolder fromViewHolder, RecyclerView.ViewHolder toViewHolder);

        /// <summary>
        /// Executes the corresponding bound property when view is swiped towards right or left direction.
        /// </summary>
        /// <param name="viewHolder">View that was swiped.</param>
        /// <param name="direction">Whether view was swiped towards the right or left direction.</param>
        /// <exception cref="ArgumentNullException">If swipe functionality is enabled and any of the swipe-able commands are null.</exception>
        void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction);

        /// <summary>
        /// This executes when an action is executed on a ViewHolder.
        /// </summary>
        /// <param name="viewHolder">The ViewHolder the action applies to.</param>
        /// <param name="actionState">Action taken on the ViewHolder.</param>
        void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState);

        /// <summary>
        /// This executes when <see cref="MvxExpandableRecyclerBaseView"/> has completed an action, i.e. dragging, swiped, etc.
        /// </summary>
        /// <param name="recyclerView"><see cref="MvxExpandableRecyclerBaseView"/>.</param>
        /// <param name="viewHolder">The ViewHolder the action was executed on.</param>
        void OnClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder);

        /// <summary>
        /// Get header associated with task item.
        /// </summary>
        /// <param name="item">Item to get header information from.</param>
        /// <returns>Header matching item's header.</returns>
        ITaskHeader GetHeader(ITaskItem item);
    }
}