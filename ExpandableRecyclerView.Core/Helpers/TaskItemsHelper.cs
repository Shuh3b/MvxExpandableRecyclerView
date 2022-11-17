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
        /// Cast non-generic <see cref="ITaskItem"/> to a valid <see cref="ITaskItem{TModel, THeader}"/>.
        /// </summary>
        /// <typeparam name="TModel">Type to cast model to.</typeparam>
        /// <typeparam name="THeader">Type to cast header to.</typeparam>
        /// <param name="item"><see cref="ITaskItem"/> to cast from.</param>
        /// <returns>An <see cref="ITaskItem{TModel, THeader}"/>.</returns>
        public static ITaskItem<TModel, THeader> Cast<TModel, THeader>(this ITaskItem item)
        {
            return (ITaskItem<TModel, THeader>)item;
        }

        /// <summary>
        /// Get strongly typed model matching given type from task item.
        /// </summary>
        /// <typeparam name="TModel">Type to cast model to.</typeparam>
        /// <param name="item"><see cref="ITaskItem"/> to retrieve model from.</param>
        /// <returns>Strongly typed model from task item.</returns>
        public static TModel Model<TModel>(this ITaskItem item)
        {
            return (TModel)item?.Model;
        }

        /// <summary>
        /// Returns the first element's model in a list, or a default value if the list contains no elements.
        /// </summary>
        /// <typeparam name="TModel">Type of model to retrieve from list.</typeparam>
        /// <param name="collection">The <see cref="IList{T}"/> to retrieve the first element from.</param>
        /// <returns>The first element in the list that passes the test specified by the type. Otherwise returns default(TModel).</returns>
        public static TModel FirstOrDefaultModel<TModel>(this IList<ITaskItem> collection)
        {
            ITaskItem item = collection.FirstOrDefault(i => i.Model is TModel);

            return item?.Model is TModel model ? model : default;
        }

        /// <summary>
        /// Returns the first element's model in a list, or a default value if the list contains no elements.
        /// </summary>
        /// <typeparam name="TModel">Type of model to retrieve from list.</typeparam>
        /// <param name="collection">The <see cref="IList{T}"/> to retrieve the first element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The first element in the list that passes the test specified by the type and predicate. Otherwise returns default(TModel).</returns>
        public static TModel FirstOrDefaultModel<TModel>(this IList<ITaskItem> collection, Func<TModel, bool> predicate)
        {
            ITaskItem item = collection.FirstOrDefault(i => i.Model is TModel m && predicate?.Invoke(m) == true);

            return item?.Model is TModel model ? model : default;
        }

        /// <summary>
        /// Returns the last element's model in a list, or a default value if the list contains no elements.
        /// </summary>
        /// <typeparam name="TModel">Type of model to retrieve from list.</typeparam>
        /// <param name="collection">The <see cref="IList{T}"/> to retrieve the last element from.</param>
        /// <returns>The last element in the list that passes the test specified by the type. Otherwise returns default(TModel).</returns>
        public static TModel LastOrDefaultModel<TModel>(this IList<ITaskItem> collection)
        {
            ITaskItem item = collection.LastOrDefault(i => i.Model is TModel);

            return item?.Model is TModel model ? model : default;
        }

        /// <summary>
        /// Returns the last element's model in a list, or a default value if the list contains no elements.
        /// </summary>
        /// <typeparam name="TModel">Type of model to retrieve from list.</typeparam>
        /// <param name="collection">The <see cref="IList{T}"/> to retrieve the last element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The last element in the list that passes the test specified by the type and predicate. Otherwise returns default(TModel).</returns>
        public static TModel LastOrDefaultModel<TModel>(this IList<ITaskItem> collection, Func<TModel, bool> predicate)
        {
            ITaskItem item = collection.LastOrDefault(i => i.Model is TModel m && predicate?.Invoke(m) == true);

            return item?.Model is TModel model ? model : default;
        }

        /// <summary>
        /// Filters a list of task items and returns their models based on the provided type.
        /// </summary>
        /// <typeparam name="TModel">Type of model to retrieve from list.</typeparam>
        /// <param name="collection">The <see cref="IList{T}"/> to filter through.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input list that satisfy the type match.</returns>
        public static IEnumerable<TModel> WhereModel<TModel>(this IList<ITaskItem> collection)
        {
            return collection.Where(i => i.Model is TModel).Select(i => (TModel)i.Model);
        }

        /// <summary>
        /// Filters a list of task items and returns their models based on the provided type and predicate.
        /// </summary>
        /// <typeparam name="TModel">Type of model to retrieve from list.</typeparam>
        /// <param name="collection">The <see cref="IList{T}"/> to filter through.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input list that satisfy the type match and predicate.</returns>
        public static IEnumerable<TModel> WhereModel<TModel>(this IList<ITaskItem> collection, Func<TModel, bool> predicate)
        {
            return collection.Where(i => i.Model is TModel m && predicate?.Invoke(m) == true).Select(i => (TModel)i.Model);
        }

        /// <summary>
        /// Updates an <see cref="ITaskItem"/> with a specified header.
        /// </summary>
        /// <param name="collection">The <see cref="IList{T}"/> to retrieve the item from.</param>
        /// <param name="taskItem">The item to update.</param>
        /// <param name="taskHeader">The header to update the item's header to.</param>
        /// <param name="preserveSequence">Whether the sequence should be preserved when updating item.</param>
        public static void Update(this IList<ITaskItem> collection, ITaskItem taskItem, object taskHeader, bool preserveSequence = false)
        {
            ITaskItem item = collection.FirstOrDefault(i => i == taskItem);

            int? sequence = item.Sequence;
            if (collection.Remove(taskItem))
            {
                item.Header = taskHeader;
                if (preserveSequence)
                {
                    item.Sequence = sequence;
                }
                collection.Add(taskItem);
            }
        }

        /// <summary>
        /// Updates a list of <see cref="ITaskItem"/> with a specified header.
        /// </summary>
        /// <param name="collection">The <see cref="IList{T}"/> to retrieve the items from.</param>
        /// <param name="taskItems">The list of items to update.</param>
        /// <param name="taskHeader">The header to update the item's header to.</param>
        public static void Update(this IList<ITaskItem> collection, IEnumerable<ITaskItem> taskItems, object taskHeader)
        {
            foreach (ITaskItem item in taskItems.Where(i => collection.Remove(i)))
            {
                item.Header = taskHeader;
                collection.Add(item);
            }
        }
    }
}
