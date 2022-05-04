using MvvmCross.ExpandableRecyclerView.Core;
using System;
using System.Collections.Generic;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Abstract class used for headers in MvxExpandableRecyclerView.
    /// </summary>
    /// <typeparam name="TModel">Type of model for header.</typeparam>
    public class TaskHeader<TModel> : TaskItem<TModel>, ITaskHeader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="model">Header model.</param>
        public TaskHeader(string name, TModel model) : base(model)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override object Header 
        { 
            get => Model; 
            set => throw new InvalidOperationException("Header cannot be changed once set."); 
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public bool IsCollapsed => CollapsedItems != null;

        /// <inheritdoc/>
        public IList<ITaskItem> CollapsedItems { get; set; }

        /// <inheritdoc/>
        public TaskHeaderRule Rules { get; set; }

        /// <inheritdoc/>
        public new int? Sequence
        {
            get => null;
            set => throw new InvalidOperationException("Header sequence should never be changed.");
        }
    }
}