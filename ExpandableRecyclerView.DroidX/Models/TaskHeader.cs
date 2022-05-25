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
    /// <typeparam name="THeader">Type of header to use for </typeparam>
    public abstract class TaskHeader<TModel, THeader> : TaskItem<TModel, THeader>, ITaskHeader
    {
        private bool isCollapsed;
        private ObservableCollection<ITaskItem> items;

        /// <summary>
        /// Constructor.
        /// You will need to override <see cref="Header"/> and assign <see cref="Model"/> or a property from it.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="model">Header model.</param>
        protected TaskHeader(string name, TModel model)
            : base(model)
        {
            Name = name;
            Init();
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
                    OnPropertyChanged(nameof(Count));
            }
        }

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public TaskHeaderRule Rules { get; set; }

        /// <inheritdoc/>
        public new int? Sequence
        {
            get => null;
            set => throw new InvalidOperationException("Header sequence should never be changed.");
        }

        private void Init()
        {
            Items = new ObservableCollection<ITaskItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Count));

            if (Rules.HasFlag(TaskHeaderRule.SequenceDisabled))
                return;

            if (items.All(i => !i.Sequence.HasValue))
                return;

            var lastSequencedItem = items.LastOrDefault(i => i.Sequence.HasValue);
            int lastSequencedPosition = items.GetPosition(lastSequencedItem);

            if (lastSequencedPosition < 0)
                return;

            for (int i = 0; i <= lastSequencedPosition; i++)
            {
                items[i].Sequence = i;
            }
        }
    }
}