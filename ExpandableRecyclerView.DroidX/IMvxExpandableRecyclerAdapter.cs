using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ExpandableRecyclerView.Core;
using MvvmCross.ExpandableRecyclerView.DroidX.Components;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Interface exposing adapter properties used for <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
    /// </summary>
    public interface IMvxExpandableRecyclerAdapter : IMvxRecyclerAdapter, IMvxRecyclerAdapterBindableHolder, IItemTouchHelperCallback, IStickyHeaderHelper
    {
        /// <summary>
        /// Whether to enable MvxExpandableRecyclerView default item selecting via <see cref="ITaskItem.IsSelected"/>. 
        /// This will not affect the command bound to <see cref="IMvxRecyclerAdapter.ItemClick"/>.
        /// </summary>
        bool EnableSelect { get; set; }

        /// <summary>
        /// Whether to enable MvxExpandableRecyclerView default item highlighting via <see cref="ITaskItem.IsHighlighted"/>. 
        /// This will not affect the command bound to <see cref="IMvxRecyclerAdapter.ItemLongClick"/>.
        /// </summary>
        bool EnableHighlight { get; set; }

        /// <summary>
        /// <para>Get or set the <see cref="ICommand"/> to trigger when a header is long clicked.</para>
        /// <para>If <see cref="HeaderLongClick"/> is <c>null</c> and <see cref="EnableHighlight"/> is set to <c>true</c>, the RecyclerView will set <see cref="ITaskItem.IsHighlighted"/> for all items under this header.</para>
        /// <para>If <see cref="HeaderLongClick"/> is not <c>null</c>, it will override the highlight behaviour, even if <see cref="EnableHighlight"/> is set to <c>true</c>.</para>
        /// </summary>
        ICommand HeaderLongClick { get; set; }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the right.
        /// </summary>
        ICommand ItemSwipeRight { get; set; }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the left.
        /// </summary>
        ICommand ItemSwipeLeft { get; set; }

        /// <summary>
        /// Item that was last interacted with.
        /// </summary>
        ITaskItem SelectedItem { get; }
    }
}