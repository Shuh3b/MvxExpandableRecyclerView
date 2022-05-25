using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Extensions.Logging;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.DroidX.RecyclerView.Model;
using MvvmCross.ExpandableRecyclerView.Core;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.WeakSubscription;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// An adapter that works with <see cref="MvxExpandableRecyclerView"/>, to create a bindable, expandable, draggable and/or swipe-able RecyclerView.
    /// <para>Custom headers can be added/edited/removed by inheriting this object, overriding <see cref="GenerateHeader(object)"/> and attaching the derived adapter to <see cref="MvxExpandableRecyclerView"/>.</para>
    /// <code>
    /// var expandableRecyclerView = _view.FindViewById<MvxExpandableRecyclerView>(Resource.Id.expandablerecyclerview);
    /// recyclerView.Adapter = new CustomExpandableRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
    /// </code>
    /// </summary>
    /// <typeparam name="THeader">Header type to use.</typeparam>
    public class MvxExpandableRecyclerAdapter<THeader> : MvxRecyclerAdapter, IMvxExpandableRecyclerAdapter
    {
        private IList<ITaskItem> viewItemsSource, itemsSource;
        private IDisposable subscription;
        private IMvxTemplateSelector itemTemplateSelector;
        private ICommand itemSwipeStart, itemSwipeEnd;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MvxExpandableRecyclerAdapter()
            : this(null) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bindingContext">Binding context.</param>
        public MvxExpandableRecyclerAdapter(IMvxAndroidBindingContext bindingContext)
            : base(bindingContext) { }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the start direction.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeStart
        {
            get => itemSwipeStart;
            set
            {
                if (ReferenceEquals(itemSwipeStart, value))
                    return;

                itemSwipeStart = value;
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the end direction.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeEnd
        {
            get => itemSwipeEnd;
            set
            {
                if (ReferenceEquals(itemSwipeEnd, value))
                    return;

                itemSwipeEnd = value;
            }
        }

        /// <inheritdoc/>
        [MvxSetToNullAfterBinding]
        public override IEnumerable ItemsSource
        {
            get => itemsSource;
            set => SetItemsSource(value);
        }

        /// <inheritdoc/>
        [MvxSetToNullAfterBinding]
        public override IMvxTemplateSelector ItemTemplateSelector
        {
            get => itemTemplateSelector;
            set
            {
                if (ReferenceEquals(itemTemplateSelector, value))
                    return;

                itemTemplateSelector = value;

                if (viewItemsSource != null && itemsSource != null)
                    NotifyDataSetChanged();
            }
        }

        /// <inheritdoc/>
        public override int ItemCount => viewItemsSource?.Count ?? 0;

        /// <inheritdoc/>
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dataContext = GetItem(position);
            if (holder is IMvxRecyclerViewHolder viewHolder)
            {
                if (dataContext is ITaskHeader)
                {
                    viewHolder.DataContext = dataContext;
                    viewHolder.Click -= OnHeaderViewClick;
                    viewHolder.Click += OnHeaderViewClick;
                }
                else
                {
                    viewHolder.DataContext = dataContext;
                    viewHolder.Click -= base.OnItemViewClick;
                    viewHolder.LongClick -= base.OnItemViewLongClick;
                    viewHolder.Click += base.OnItemViewClick;
                    viewHolder.LongClick += base.OnItemViewLongClick;
                }

                if (viewHolder.Id == Android.Resource.Layout.SimpleListItem1)
                {
                    ((TextView)holder.ItemView).Text = dataContext?.ToString();
                }
            }
            OnMvxViewHolderBound(new MvxViewHolderBoundEventArgs(position, dataContext, holder));
        }

        /// <inheritdoc/>
        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            if (holder is IMvxRecyclerViewHolder viewHolder)
            {
                if (viewHolder.DataContext is ITaskHeader)
                {
                    viewHolder.Click -= OnHeaderViewClick;
                }
                else
                {
                    viewHolder.Click -= OnItemViewClick;
                    viewHolder.LongClick -= OnItemViewLongClick;
                }
                viewHolder.OnViewRecycled();
            }
        }

        /// <inheritdoc/>
        public override object GetItem(int viewPosition)
        {
            var itemsSourcePosition = GetItemsSourcePosition(viewPosition);

            if (viewItemsSource is IList items)
            {
                if (itemsSourcePosition >= 0 && itemsSourcePosition < items.Count)
                    return items[itemsSourcePosition];
                MvxAndroidLog.Instance.Log(LogLevel.Error,
                    "MvxExpandableRecyclerView GetItem index out of range. viewPosition: {ViewPosition}, itemsSourcePosition: {ItemsSourcePosition}, itemCount: {ItemsSourceCount}",
                    viewPosition, itemsSourcePosition, viewItemsSource.Count());
                return null;
            }

            return viewItemsSource?.ElementAt(itemsSourcePosition);
        }

        /// <inheritdoc/>
        protected override int GetViewPosition(object item)
        {
            var itemsSourcePosition = viewItemsSource.GetPosition(item);
            return GetViewPosition(itemsSourcePosition);
        }

        /// <inheritdoc/>
        protected override void SetItemsSource(IEnumerable value)
        {
            if (Looper.MainLooper != Looper.MyLooper())
                MvxAndroidLog.Instance.Log(LogLevel.Error,
                    "ItemsSource property set on a worker thread. This leads to crash in the RecyclerView. It must be set only from the main thread.");

            if (ReferenceEquals(itemsSource, value) && !ReloadOnAllItemsSourceSets)
                return;

            subscription?.Dispose();
            subscription = null;

            if (value != null && !(value is IList<ITaskItem>))
                MvxAndroidLog.Instance.Log(LogLevel.Error,
                    "ItemsSource property should inherit IList<ITaskItem>. If not, this will lead to items not displaying in the RecyclerView.");

            var val = value as IList<ITaskItem>;

            if (val is INotifyCollectionChanged newObservable)
                subscription = newObservable.WeakSubscribe(OnItemsSourceCollectionChanged);

            if (val == null)
            {
                itemsSource = null;
                viewItemsSource = null;
            }
            else
            {
                itemsSource = val;
                viewItemsSource = GenerateHeaders(val);
                GenerateInitialHeaders();
            }

            NotifyDataSetChanged();
        }

        /// <inheritdoc/>
        protected override void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (subscription == null || itemsSource == null)
                return;

            if (Looper.MainLooper == Looper.MyLooper())
                NotifyDataSetChanged(e);
            else
                MvxAndroidLog.Instance.Log(LogLevel.Error,
                    $@"ItemsSource collection content changed on a worker thread.
This leads to crash in the RecyclerView as it will not be aware of changes
immediatly and may get a deleted item or update an item with a bad item template.
All changes must be synchronized on the main thread.");
        }

        /// <inheritdoc/>
        public override void NotifyDataSetChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                NotifyDataSetReset();
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    NotifyDataSetAdded(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    NotifyDataSetMoved(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    NotifyDataSetReplaced(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    NotifyDataSetRemoved(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    NotifyDataSetReset();
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            subscription?.Dispose();
            subscription = null;
            itemSwipeStart = null;
            itemSwipeEnd = null;
            itemTemplateSelector = null;
        }

        /// <inheritdoc/>
        public virtual void OnMove(RecyclerView.ViewHolder fromViewHolder, RecyclerView.ViewHolder toViewHolder)
        {
            int toPosition = toViewHolder.AdapterPosition;

            if (toPosition <= 0)
                return;

            int fromPosition = fromViewHolder.AdapterPosition;

            var item = viewItemsSource[fromPosition];
            viewItemsSource.RemoveAt(fromPosition);
            viewItemsSource.Insert(toPosition, item);
            NotifyItemMoved(fromPosition, toPosition);
        }

        /// <inheritdoc/>
        public virtual void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            if (viewHolder is IMvxRecyclerViewHolder holder)
            {
                if (direction == ItemTouchHelper.Start)
                {
                    if (ItemSwipeStart == null)
                        throw new ArgumentNullException(nameof(ItemSwipeStart),
                            $"Either implement swipe feature and bind to {nameof(ItemSwipeStart)} or disable swiping towards start direction.");

                    ExecuteCommandOnItem(ItemSwipeStart, holder.DataContext);
                }
                else if (direction == ItemTouchHelper.End)
                {
                    if (ItemSwipeEnd == null)
                        throw new ArgumentNullException(nameof(ItemSwipeEnd),
                            $"Either implement swipe feature and bind to {nameof(ItemSwipeEnd)} or disable swiping towards end direction.");

                    ExecuteCommandOnItem(ItemSwipeEnd, holder.DataContext);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void OnClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            int itemPosition = viewHolder.AdapterPosition;

            if (itemPosition <= 0)
                return;

            var item = viewItemsSource[itemPosition];
            var aboveItem = viewItemsSource[itemPosition - 1];

            var previousHeader = GetHeader(item);
            var newHeader = GetHeader(aboveItem);

            if (previousHeader.Header.Equals(newHeader.Header))
            {
                int position = itemPosition - viewItemsSource.GetPosition(previousHeader) - 1;
                item.Sequence = position;
                previousHeader.Items.Remove(item);
                previousHeader.Items.Insert(position, item);
                return;
            }

            if (TaskHeaderDragInDisabled(newHeader, item))
                return;

            item.Header = newHeader.Header;
            previousHeader.Items.Remove(item);

            if (newHeader.IsCollapsed)
            {
                item.Sequence = null;
                newHeader.Items.Add(item);
                viewItemsSource.Remove(item);
                NotifyItemRemoved(itemPosition);
            }
            else
            {
                int position = itemPosition - viewItemsSource.GetPosition(newHeader) - 1;
                item.Sequence = position;
                newHeader.Items.Insert(position, item);
                NotifyItemChanged(itemPosition);
            }

            RemoveTemporaryTaskHeader(previousHeader);
        }

        /// <summary>
        /// Get headers.
        /// </summary>
        /// <returns>List of headers.</returns>
        public IList<ITaskHeader> GetHeaders()
        {
            return viewItemsSource.Where(i => i is ITaskHeader).Cast<ITaskHeader>().ToList();
        }

        /// <summary>
        /// Get header.
        /// </summary>
        /// <param name="item">Item to get header information from.</param>
        /// <returns>Header matching item's header.</returns>
        public ITaskHeader GetHeader(ITaskItem item)
        {
            return viewItemsSource.FirstOrDefault(i => i is ITaskHeader header && (header.Header.Equals(item.Header) || header.Items.Contains(item))) as ITaskHeader;
        }

        /// <summary>
        /// Get items in header.
        /// </summary>
        /// <param name="header">Header.</param>
        /// <returns>List of items with matching header.</returns>
        public IList<ITaskItem> GetItemsInHeader(ITaskHeader header)
        {
            return header.Items;
        }

        /// <summary>
        /// Get number of visible items shown under header.
        /// </summary>
        /// <param name="header">Header.</param>
        /// <returns>Number of visible items with matching header.</returns>
        public int CountVisibleItemsInHeader(ITaskHeader header)
        {
            return header.IsCollapsed ? 0 : header.Items.Count;
        }

        /// <summary>
        /// Add headers to list, regardless of whether they have items associated with them or not.
        /// </summary>
        /// <returns>List of headers to display.</returns>
        protected virtual IEnumerable<THeader> AddInitialHeaders()
        {
            return new List<THeader>();
        }

        /// <summary>
        /// Generate headers based on item's distinct header.
        /// </summary>
        /// <param name="items">The items to show under distinct headers.</param>
        /// <returns>List of items containing headers and items.</returns>
        protected virtual IList<ITaskItem> GenerateHeaders(IEnumerable<ITaskItem> items)
        {
            var groupedItems = items.Where(i => !(i is ITaskHeader)).OrderBy(i => i.Header).GroupBy(i => i.Header);
            List<ITaskItem> taskItems = new List<ITaskItem>();

            foreach (var group in groupedItems)
            {
                var generatedHeader = GenerateHeader((THeader)group.Key);

                if (group.Key == null)
                {
                    foreach (var item in group)
                    {
                        item.Header = generatedHeader.Header;
                    }
                }

                if (taskItems.FirstOrDefault(item => item is ITaskHeader header && header.Header.Equals(generatedHeader.Header)) is ITaskHeader existingHeader)
                {
                    var headerItems = existingHeader.Items.ToList();
                    existingHeader.Items.Clear();
                    taskItems.RemoveAll(item => headerItems.Contains(item));
                    headerItems.AddRange(group);

                    int position = taskItems.GetPosition(existingHeader) + 1;

                    foreach (var item in headerItems.OrderByDescending(i => i.Sequence.HasValue).ThenBy(i => i.Sequence))
                    {
                        existingHeader.Items.Add(item);
                    }

                    if (!existingHeader.IsCollapsed)
                        taskItems.InsertRange(position, existingHeader.Items);
                }
                else
                {
                    foreach (var item in group.OrderByDescending(i => i.Sequence.HasValue).ThenBy(i => i.Sequence))
                    {
                        generatedHeader.Items.Add(item);
                    }

                    taskItems.Add(generatedHeader);

                    if (!generatedHeader.IsCollapsed)
                        taskItems.AddRange(generatedHeader.Items);
                }
            }

            return taskItems;
        }

        /// <summary>
        /// Generate a header with a given model.
        /// <para>This method can be overriden to fine-tune the grouping logic to add/edit/remove headers and/or specify header rules for specific headers.</para>
        /// <para>IMPORTANT: If header is nullable, make sure to handle nullable type by using a non-null value. E.g. Header type with <c>int?</c> will use <c>-1</c> when <c>null</c>.</para>
        /// </summary>
        /// <param name="model">Model used for generating header.</param>
        /// <returns>Header object used in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.</returns>
        protected virtual ITaskHeader GenerateHeader(THeader model)
        {
            if (model == null)
                MvxAndroidLog.Instance.Log(LogLevel.Error,
                    $@"The {nameof(model)} for header was null. Binding to ItemsSource will fail because grouping cannot be applied to null values.
If header is nullable, make sure to override {nameof(GenerateHeader)}() and handle nullable types by using a non-null value. E.g. Header type with int? will use -1 when null.");

            return new SimpleTaskHeader<THeader>(model?.ToString(), model);
        }

        private void OnHeaderViewClick(object sender, EventArgs e)
        {
            if (sender is MvxRecyclerViewHolder holder && holder.DataContext is ITaskHeader header)
            {
                int position = holder.AdapterPosition + 1;

                if (header.IsCollapsed)
                {
                    var itemsToAdd = GetItemsInHeader(header);
                    int pos = position;
                    foreach (var item in itemsToAdd)
                    {
                        viewItemsSource.Insert(pos++, item);
                    }
                    header.IsCollapsed = false;
                    NotifyItemRangeInserted(position, itemsToAdd.Count);
                }
                else
                {
                    var itemsToRemove = GetItemsInHeader(header);
                    foreach (var item in itemsToRemove)
                    {
                        viewItemsSource.Remove(item);
                    }
                    header.IsCollapsed = true;
                    NotifyItemRangeRemoved(position, itemsToRemove.Count);
                }
            }
        }

        private void NotifyDataSetAdded(NotifyCollectionChangedEventArgs e)
        {
            var newItems = e.NewItems.Cast<ITaskItem>().ToList();
            foreach (var item in newItems)
            {
                var generatedHeader = GenerateHeader((THeader)item.Header);

                var header = AddHeader(generatedHeader) ? generatedHeader : GetHeader(generatedHeader);

                int headerPosition = viewItemsSource.GetPosition(header);

                if (header.IsCollapsed)
                {
                    if (item.Sequence.HasValue && item.Sequence.Value <= header.Items.Count)
                        header.Items.Insert(item.Sequence.Value, item);
                    else
                        header.Items.Add(item);
                }
                else
                {
                    int position = item.Sequence.HasValue && item.Sequence.Value <= header.Items.Count
                        ? headerPosition + item.Sequence.Value + 1
                        : headerPosition + CountVisibleItemsInHeader(header) + 1;
                    header.Items.Insert(position - headerPosition - 1, item);
                    viewItemsSource.Insert(position, item);
                    NotifyItemInserted(GetViewPosition(position));
                }
            }
        }

        private void NotifyDataSetMoved(NotifyCollectionChangedEventArgs e)
        {
            for (var i = 0; i < e.NewItems.Count; i++)
                NotifyItemMoved(GetViewPosition(e.OldStartingIndex + i), GetViewPosition(e.NewStartingIndex + i));
        }

        private void NotifyDataSetReplaced(NotifyCollectionChangedEventArgs e)
        {
            NotifyItemRangeChanged(GetViewPosition(e.NewStartingIndex), e.NewItems.Count);
        }

        private void NotifyDataSetRemoved(NotifyCollectionChangedEventArgs e)
        {
            var oldItems = e.OldItems.Cast<ITaskItem>().ToList();
            foreach (var item in oldItems)
            {
                item.Sequence = null;
                var header = GetHeader(item);

                if (header.IsCollapsed)
                {
                    header.Items.Remove(item);
                }
                else
                {
                    var position = viewItemsSource.GetPosition(item);
                    header.Items.Remove(item);
                    viewItemsSource.Remove(item);
                    NotifyItemRemoved(GetViewPosition(position));
                }

                RemoveTemporaryTaskHeader(item);
            }
        }

        private void NotifyDataSetReset()
        {
            ReloadOnAllItemsSourceSets = true;
            SetItemsSource(itemsSource);
            ReloadOnAllItemsSourceSets = false;
        }

        private void GenerateInitialHeaders()
        {
            if (viewItemsSource == null)
                viewItemsSource = new List<ITaskItem>();

            foreach (var initialHeader in AddInitialHeaders())
            {
                var header = GenerateHeader(initialHeader);
                AddHeader(header);
            }
        }

        private bool AddHeader(ITaskHeader header)
        {
            if (viewItemsSource.Any(h => h.Header.Equals(header.Header)))
                return false;

            viewItemsSource.Add(header);
            var headers = GetHeaders();

            if (headers.Count <= 1)
            {
                int headerPosition = viewItemsSource.GetPosition(header);
                NotifyItemInserted(headerPosition);
                return true;
            }

            var orderedHeaders = headers.OrderBy(h => h.Header);

            int aboveHeaderPosition = orderedHeaders.GetPosition(header) - 1;
            int newHeaderPosition;
            if (aboveHeaderPosition < 0)
            {
                newHeaderPosition = 0;
            }
            else
            {
                var aboveHeader = orderedHeaders.ElementAt(aboveHeaderPosition);
                newHeaderPosition = viewItemsSource.GetPosition(aboveHeader) + CountVisibleItemsInHeader(aboveHeader) + 1;
            }
            viewItemsSource.Remove(header);
            viewItemsSource.Insert(newHeaderPosition, header);
            NotifyItemInserted(newHeaderPosition);
            return true;
        }

        private bool TaskHeaderDragInDisabled(ITaskHeader newHeader, ITaskItem item)
        {
            if (newHeader?.Rules.HasFlag(TaskHeaderRule.DragInDisabled) == true)
            {
                var currentHeader = GetHeader(item);
                int currentHeaderPosition = viewItemsSource.GetPosition(currentHeader);
                bool positiveDirection = currentHeaderPosition < viewItemsSource.GetPosition(newHeader);

                int newPosition;
                int newPositionInHeader;
                if (positiveDirection)
                {
                    newPosition = currentHeaderPosition + CountVisibleItemsInHeader(currentHeader);
                    newPositionInHeader = newPosition - currentHeaderPosition - 1;
                }
                else
                {
                    newPosition = currentHeaderPosition;
                    newPositionInHeader = newPosition - currentHeaderPosition;
                }

                currentHeader.Items.Remove(item);
                currentHeader.Items.Insert(newPositionInHeader, item);

                int position = viewItemsSource.GetPosition(item);
                viewItemsSource.Remove(item);
                viewItemsSource.Insert(newPosition, item);
                NotifyItemMoved(position, newPosition);
                return true;
            }
            return false;
        }

        private bool TaskHeaderDragOutDisabled(ITaskHeader header)
        {
            return header?.Rules.HasFlag(TaskHeaderRule.DragOutDisabled) == true;
        }

        private bool RemoveTemporaryTaskHeader(ITaskItem item)
        {
            var itemHeader = GetHeader(item);
            if (itemHeader?.Rules.HasFlag(TaskHeaderRule.Temporary) == true)
            {
                var items = GetItemsInHeader(itemHeader);
                if (items?.Count > 0)
                    return false;

                int position = viewItemsSource.GetPosition(itemHeader);
                viewItemsSource.Remove(itemHeader);
                NotifyItemRemoved(position);
                return true;
            }
            return false;
        }
    }
}