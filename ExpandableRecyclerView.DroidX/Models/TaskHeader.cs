using MvvmCross.Binding.Extensions;
using MvvmCross.ExpandableRecyclerView.Core;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Abstract class used for headers in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
    /// </summary>
    /// <typeparam name="TModel">Type of model for header.</typeparam>
    /// <typeparam name="THeader">Type of header to use.</typeparam>
    public abstract class TaskHeader<TModel, THeader> : TaskItem<TModel, THeader>, ITaskHeader
    {
        private bool isCollapsed;
        private ObservableCollection<ITaskItem> items;
        private bool isSticky;

        /// <summary>
        /// Constructor.
        /// <para>You will need to override Header and assign a property from Model or the Model itself.</para>
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="model">Header model.</param>
        protected TaskHeader(string name, TModel model)
            : base(model)
        {
            Name = name;
            Items = new ObservableCollection<ITaskItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public bool IsCollapsed { get => isCollapsed; set => SetProperty(ref isCollapsed, value); }

        /// <inheritdoc/>
        public ObservableCollection<ITaskItem> Items
        {
            get => items;
            set
            {
                if (SetProperty(ref items, value))
                {
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public bool IsSticky { get => isSticky; set => SetProperty(ref isSticky, value); }

        /// <inheritdoc/>
        public TaskHeaderRule Rules { get; set; }

        /// <inheritdoc/>
        public new int? Sequence
        {
            get => null;
            set => throw new InvalidOperationException("Header sequence should never be changed.");
        }

        /// <summary>
        /// <para>Event handler that's called when <see cref="Items"/> is modified.</para>
        /// <para>Ensure that the base implementation is called when overriding this method.</para>
        /// <para>It's discouraged to execute any expensive logic here when overriding.</para>
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        protected virtual void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Count));

            if (Rules.HasFlag(TaskHeaderRule.SequenceDisabled) || items.All(i => !i.Sequence.HasValue))
            {
                return;
            }

            var lastSequencedItem = items.LastOrDefault(i => i.Sequence.HasValue);
            int lastSequencedPosition = items.GetPosition(lastSequencedItem);

            if (lastSequencedPosition < 0)
            {
                return;
            }

            for (int i = 0; i <= lastSequencedPosition; i++)
            {
                items[i].Sequence = i;
            }
        }
    }
}