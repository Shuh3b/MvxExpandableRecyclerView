﻿using MvvmCross.DroidX.RecyclerView;
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