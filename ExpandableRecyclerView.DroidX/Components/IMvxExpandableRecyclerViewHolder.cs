using Android.Views;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ExpandableRecyclerView.Core;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// Interface exposing properties and methods used in <see cref="MvxExpandableRecyclerAdapter{THeader}"/> and <see cref="ItemTouchHelperCallback"/> to help with displaying different layouts when swiping.
    /// </summary>
    public interface IMvxExpandableRecyclerViewHolder : IMvxRecyclerViewHolder
    {
        /// <summary>
        /// The view to display for the item.
        /// </summary>
        View Foreground { get; }

        /// <summary>
        /// <c>true</c> if item is swiping towards the right. <c>false</c> if swiping left.
        /// </summary>
        bool IsSwipingRight { get; set; }

        /// <summary>
        /// <see cref="ITaskItem"/> attached to this view.
        /// </summary>
        new ITaskItem DataContext { get; set; }

        /// <summary>
        /// Action that selects this view holder's <see cref="DataContext"/> as part of a multi-select process.
        /// </summary>
        event Action<ITaskItem> HighlightClick;
    }
}
