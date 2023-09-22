﻿using Android.Runtime;
using Android.Views;
using MvvmCross.Commands;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ExpandableRecyclerView.Core;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// ViewHolder to hold items that show right and left background layouts when item is swiped.
    /// </summary>
    public class MvxExpandableRecyclerViewHolder : MvxRecyclerViewHolder, IMvxExpandableRecyclerViewHolder
    {
        private const string ForegroundTag = "swipe_foreground";
        private const string SwipeRightTag = "swipe_right_background";
        private const string SwipeLeftTag = "swipe_left_background";
        private View foreground, swipeLeftBackground, swipeRightBackground;
        private bool isSwipingRight;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemView">View of item.</param>
        /// <param name="context">Context of the item.</param>
        public MvxExpandableRecyclerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            foreground = itemView.FindViewWithTag(ForegroundTag);
            swipeRightBackground = itemView.FindViewWithTag(SwipeRightTag);
            swipeLeftBackground = itemView.FindViewWithTag(SwipeLeftTag);

            if (swipeRightBackground != null)
            {
                swipeRightBackground.Visibility = ViewStates.Invisible;
            }

            if (swipeLeftBackground != null)
            {
                swipeLeftBackground.Visibility = ViewStates.Invisible;
            }

            SetBackground(swipeRightBackground != null || swipeLeftBackground == null);
        }

        /// <inheritdoc/>
        protected MvxExpandableRecyclerViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        { }

        /// <inheritdoc/>
        public new ITaskItem DataContext
        {
            get => (ITaskItem)base.DataContext;
            set => base.DataContext = value;
        }

        /// <inheritdoc/>
        public View Foreground => foreground ?? ItemView;

        /// <inheritdoc/>
        public bool IsSwipingRight
        {
            get => isSwipingRight;
            set
            {
                if (value == isSwipingRight)
                {
                    return;
                }

                SetBackground(value);
            }
        }

        /// <inheritdoc/>
        public event Action<ITaskItem> HighlightClick
        {
            add => DataContext.SetHighlightCommand(new MvxCommand(() => value?.Invoke(DataContext)));
            remove => DataContext.SetHighlightCommand(null);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            foreground = null;
            swipeLeftBackground = null;
            swipeRightBackground = null;
            DataContext.SetHighlightCommand(null);

            base.Dispose(disposing);
        }

        private void SetBackground(bool swipingRight)
        {
            if (swipeRightBackground != null)
            {
                swipeRightBackground.Visibility = swipingRight ? ViewStates.Visible : ViewStates.Invisible;
            }

            if (swipeLeftBackground != null)
            {
                swipeLeftBackground.Visibility = swipingRight ? ViewStates.Invisible : ViewStates.Visible;
            }

            isSwipingRight = swipingRight;
        }
    }
}