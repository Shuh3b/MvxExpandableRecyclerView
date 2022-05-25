using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Custom rules for task headers.
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
        /// Prevent items being swiped towards start direction.
        /// </summary>
        SwipeStartDisabled = 4,

        /// <summary>
        /// Prevent items being swiped towards end direction.
        /// </summary>
        SwipeEndDisabled = 8,

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