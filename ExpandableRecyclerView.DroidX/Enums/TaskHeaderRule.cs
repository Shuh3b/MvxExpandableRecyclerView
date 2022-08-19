using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Custom rules for headers.
    /// </summary>
    [Flags]
    public enum TaskHeaderRule
    {
        /// <summary>
        /// No rules.
        /// </summary>
        None = 0,

        /// <summary>
        /// Prevent items from being dragged into header.
        /// </summary>
        DragInDisabled = 1,

        /// <summary>
        /// Prevent items from being dragged out of header.
        /// </summary>
        DragOutDisabled = 2,

        /// <summary>
        /// Prevent items being swiped towards the right.
        /// </summary>
        SwipeRightDisabled = 4,

        /// <summary>
        /// Prevent items being swiped towards the left.
        /// </summary>
        SwipeLeftDisabled = 8,

        /// <summary>
        /// Items under header will not be sequenced.
        /// </summary>
        SequenceDisabled = 16,

        /// <summary>
        /// Header will be removed if no items are under header.
        /// </summary>
        Temporary = 32,
    }
}