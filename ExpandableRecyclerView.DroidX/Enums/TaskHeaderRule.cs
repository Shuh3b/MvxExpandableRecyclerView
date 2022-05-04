using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Custom rules for task headers.
    /// </summary>
    [Flags] public enum TaskHeaderRule
    {
        /// <summary>
        /// Allows items to be dragged under header.
        /// </summary>
        DragEnabled,

        /// <summary>
        /// Disallows items to be dragged under header.
        /// </summary>
        DragDisabled,

        /// <summary>
        /// Header will be removed if no items are under header.
        /// </summary>
        Temporary,
    }
}