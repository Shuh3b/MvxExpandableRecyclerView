using Android.Runtime;
using Android.Views;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// ViewHolder to hold items that show right and left background layouts when item is swiped.
    /// </summary>
    public class MvxSwipeRecyclerViewHolder : MvxRecyclerViewHolder
    {
        private const string foregroundId = "swipe_foreground";
        private const string swipeRightId = "swipe_right_background";
        private const string swipeLeftId = "swipe_left_background";
        private View foreground, swipeLeftBackground, swipeRightBackground;
        private bool isSwipingRight;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemView">View of item.</param>
        /// <param name="context">Context of the item.</param>
        public MvxSwipeRecyclerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            foreground = itemView.FindViewWithTag(foregroundId);
            swipeRightBackground = itemView.FindViewWithTag(swipeRightId);
            swipeLeftBackground = itemView.FindViewWithTag(swipeLeftId);

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
        protected MvxSwipeRecyclerViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        { }

        /// <summary>
        /// The view to display for the item.
        /// </summary>
        public View Foreground => foreground ?? ItemView;

        /// <summary>
        /// <c>true</c> if item is swiping towards the right. <c>false</c> if swiping left.
        /// </summary>
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
        protected override void Dispose(bool disposing)
        {
            foreground = null;
            swipeLeftBackground = null;
            swipeRightBackground = null;

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