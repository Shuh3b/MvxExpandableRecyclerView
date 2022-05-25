using MvvmCross.DroidX.RecyclerView;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Interface exposing adapter properties used for <see cref="MvxExpandableRecyclerView"/>.
    /// </summary>
    public interface IMvxExpandableRecyclerAdapter : IMvxRecyclerAdapter, IMvxRecyclerAdapterBindableHolder, IItemTouchHelperCallback
    {
        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the start direction.
        /// </summary>
        ICommand ItemSwipeStart { get; set; }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the end direction.
        /// </summary>
        ICommand ItemSwipeEnd { get; set; }
    }
}