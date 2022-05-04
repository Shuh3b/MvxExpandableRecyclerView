namespace MvvmCross.ExpandableRecyclerView.Core
{
    /// <summary>
    /// Abstract class used for items in MvxExpandableRecyclerView.
    /// </summary>
    /// <typeparam name="TModel">Type of model.</typeparam>
    public abstract class TaskItem<TModel> : ITaskItem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">Data to use.</param>
        protected TaskItem(TModel model)
        {
            Model = model;
        }

        /// <inheritdoc/>
        public abstract object Header { get; set; }

        /// <summary>
        /// The underlying data that will be displayed.
        /// Strongly typed.
        /// </summary>
        public TModel Model { get; }

        /// <inheritdoc/>
        public int? Sequence { get; set; }

        /// <inheritdoc/>
        public bool IsHighlighted { get; set; }

        /// <inheritdoc/>
        object ITaskItem.Model => Model;
    }

    /// <summary>
    /// Abstract class used for items in MvxExpandableRecyclerView.
    /// </summary>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="THeader">Type of header.</typeparam>
    public abstract class TaskItem<TModel, THeader> : ITaskItem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">Data to use.</param>
        protected TaskItem(TModel model)
        {
            Model = model;
        }

        /// <summary>
        /// The header to group items by.
        /// Strongly typed.
        /// </summary>
        public abstract THeader Header { get; set; }

        /// <summary>
        /// The underlying data that will be displayed.
        /// Strongly typed.
        /// </summary>
        public TModel Model { get; }

        /// <inheritdoc/>
        public int? Sequence { get; set; }

        /// <inheritdoc/>
        public bool IsHighlighted { get; set; }

        /// <inheritdoc/>
        object ITaskItem.Header { get => Header; set => Header = (THeader)value; }

        /// <inheritdoc/>
        object ITaskItem.Model => Model;
    }
}
