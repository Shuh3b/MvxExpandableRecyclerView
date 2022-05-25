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
        /// Updates the given <see cref="ITaskItem"/> with the given header.
        /// </summary>
        /// <param name="collection">List of items.</param>
        /// <param name="taskItem">The item to update.</param>
        /// <param name="taskHeader">The header to update the item's header to.</param>
        /// <exception cref="ArgumentNullException">Collection is null.</exception>
        public static void Update(this IList<ITaskItem> collection, ITaskItem taskItem, object taskHeader)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var item = collection.FirstOrDefault(i => i == taskItem);

            if (collection.Remove(taskItem))
            {
                item.Header = taskHeader;
                collection.Add(taskItem);
            }
        }

        /// <summary>
        /// Updates the given list of <see cref="ITaskItem"/> with the given header.
        /// </summary>
        /// <param name="collection">List of items.</param>
        /// <param name="taskItems">List of items to update.</param>
        /// <param name="taskHeader">The header to update the item's header to.</param>
        /// <exception cref="ArgumentNullException">Collection or given enumerable is null.</exception>
        public static void Update(this IList<ITaskItem> collection, IEnumerable<ITaskItem> taskItems, object taskHeader)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (taskItems == null)
                throw new ArgumentNullException(nameof(taskItems));

            foreach (var item in taskItems)
            {
                if (collection.Remove(item))
                {
                    item.Header = taskHeader;
                    collection.Add(item);
                }
            }
        }
    }
}
