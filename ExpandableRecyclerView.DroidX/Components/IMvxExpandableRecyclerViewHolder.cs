using Android.Views;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ExpandableRecyclerView.Core;

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
    }
}
