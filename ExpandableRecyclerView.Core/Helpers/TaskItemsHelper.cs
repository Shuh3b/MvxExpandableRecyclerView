using System;
using System.Collections.Generic;
using System.Linq;

namespace MvvmCross.ExpandableRecyclerView.Core.Helpers
{
    /// <summary>
    /// Helper class for implementations that use <see cref="ITaskItem"/>.
    /// </summary>
    public static class TaskItemsHelper
    {
        /// <summary>
        /// Updates the given <see cref="ITaskItem"/> with the given header in this list.
        /// </summary>
        /// <param name="taskItems">List of items.</param>
        /// <param name="taskItem">The item to update.</param>
        /// <param name="taskHeader">The header to update the item's header to.</param>
        /// <exception cref="ArgumentNullException">List is null.</exception>
        public static void Update(this IList<ITaskItem> taskItems, ITaskItem taskItem, object taskHeader)
        {
            if (taskItems == null)
                throw new ArgumentNullException(nameof(taskItems));

            var item = taskItems.FirstOrDefault(i => i == taskItem);

            if (taskItems.Remove(taskItem))
            {
                item.Header = taskHeader;
                taskItems.Add(taskItem);
            }
        }
    }
}
