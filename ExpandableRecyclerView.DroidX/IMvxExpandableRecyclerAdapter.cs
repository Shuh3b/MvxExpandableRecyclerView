using MvvmCross.Binding.Attributes;
using MvvmCross.DroidX.RecyclerView;
using System.Collections;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Interface exposing adapter properties used for <see cref="MvxExpandableRecyclerView"/>.
    /// </summary>
    public interface IMvxExpandableRecyclerAdapter : IMvxRecyclerAdapter, IMvxRecyclerAdapterBindableHolder, IItemTouchHelperCallback
    {
        ICommand ItemSwipeStart { get; set; }

        ICommand ItemSwipeEnd { get; set; }
    }
}