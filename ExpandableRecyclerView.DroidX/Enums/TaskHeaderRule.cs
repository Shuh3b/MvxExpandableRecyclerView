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
        /// Prevent items from being dragged.
        /// </summary>
        DragDisabled = 1,

        /// <summary>
        /// Prevent items from being dragged into header.
        /// </summary>
        DragInDisabled = 2,

        /// <summary>
        /// Prevent items from being dragged out of header.
        /// </summary>
        DragOutDisabled = 4,

        /// <summary>
        /// Prevent items being swiped towards the right.
        /// </summary>
        SwipeRightDisabled = 8,

        /// <summary>
        /// Prevent items being swiped towards the left.
        /// </summary>
        SwipeLeftDisabled = 16,

        /// <summary>
        /// Items under header will not be sequenced.
        /// </summary>
        SequenceDisabled = 32,

        /// <summary>
        /// Header will be removed if no items are under header.
        /// </summary>
        Temporary = 64,
    }
}