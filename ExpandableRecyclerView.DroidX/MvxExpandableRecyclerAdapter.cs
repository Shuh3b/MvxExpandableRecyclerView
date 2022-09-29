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
    /// An adapter that works with <see cref="MvxExpandableRecyclerBaseView"/>, to create a bindable, expandable, draggable and/or swipe-able RecyclerView.
    /// <para>Custom headers can be added/edited/grouped by inheriting this class, overriding <see cref="GenerateHeader(THeader)"/> and attaching the derived class to <see cref="MvxExpandableRecyclerView"/> or <see cref="MvxExpandableRecyclerBaseView"/>.</para>
    /// <code>
    /// MvxExpandableRecyclerView expandableRecyclerView = _view.FindViewById&lt;MvxExpandableRecyclerView&gt;(Resource.Id.expandablerecyclerview);
    /// expandableRecyclerView.Adapter = new CustomExpandableRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
    /// </code>
    /// </summary>
    /// <typeparam name="THeader">Header type to use.</typeparam>
    public class MvxExpandableRecyclerAdapter<THeader> : MvxRecyclerAdapter, IMvxExpandableRecyclerAdapter
    {
        private IList<ITaskItem> viewItemsSource, itemsSource;
        private IDisposable subscription;
        private IMvxTemplateSelector itemTemplateSelector;
        private ICommand itemSwipeRight, itemSwipeLeft;
        private ITaskItem selectedItem;
        private bool isDragging;

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerAdapter.
        /// </summary>
        public MvxExpandableRecyclerAdapter()
            : this(null)
        { }

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerAdapter.
        /// </summary>
        /// <param name="bindingContext">Binding context.</param>
        public MvxExpandableRecyclerAdapter(IMvxAndroidBindingContext bindingContext)
            : base(bindingContext)
        { }

        /// <summary>
        /// Item that was last interacted with.
        /// </summary>
        public ITaskItem SelectedItem => selectedItem;

        /// <inheritdoc/>
        public bool IsDragging => isDragging;

        /// <inheritdoc/>
        public bool EnableDrag { get; set; }

        /// <inheritdoc/>
        public bool EnableSwipe { get; set; }

        /// <inheritdoc/>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeRight
        {
            get => itemSwipeRight;
            set
            {
                if (ReferenceEquals(itemSwipeRight, value))
                {
                    return;
                }

                itemSwipeRight = value;
            }
        }

        /// <inheritdoc/>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeLeft
        {
            get => itemSwipeLeft;
            set
            {
                if (ReferenceEquals(itemSwipeLeft, value))
                {
                    return;
                }

                itemSwipeLeft = value;
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
                {
                    return;
                }

                itemTemplateSelector = value;

                if (viewItemsSource != null && itemsSource != null)
                {
                    NotifyDataSetChanged();
                }
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
                viewHolder.DataContext = dataContext;
                if (dataContext is ITaskHeader)
                {
                    viewHolder.Click -= OnHeaderViewClick;
                    viewHolder.Click += OnHeaderViewClick;
                }
                else
                {
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
            int itemsSourcePosition = GetItemsSourcePosition(viewPosition);

            if (viewItemsSource is IList items)
            {
                if (itemsSourcePosition >= 0 && itemsSourcePosition < items.Count)
                {
                    return items[itemsSourcePosition];
                }

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
            int itemsSourcePosition = viewItemsSource.GetPosition(item);
            return GetViewPosition(itemsSourcePosition);
        }

        /// <inheritdoc/>
        protected override void SetItemsSource(IEnumerable value)
        {
            if (Looper.MainLooper != Looper.MyLooper())
            {
                MvxAndroidLog.Instance.Log(LogLevel.Error, "ItemsSource property set on a worker thread. This leads to crash in the RecyclerView. It must be set only from the main thread.");
            }

            if (ReferenceEquals(itemsSource, value) && !ReloadOnAllItemsSourceSets)
            {
                return;
            }

            subscription?.Dispose();
            subscription = null;

            if (value != null && !(value is IList<ITaskItem>))
            {
                MvxAndroidLog.Instance.Log(LogLevel.Error, "ItemsSource property should inherit IList<ITaskItem>. If not, this will lead to items not displaying in the RecyclerView.");
            }

            IList<ITaskItem> val = value as IList<ITaskItem>;

            if (val is INotifyCollectionChanged newObservable)
            {
                subscription = newObservable.WeakSubscribe(OnItemsSourceCollectionChanged);
            }

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
            {
                return;
            }

            if (Looper.MainLooper == Looper.MyLooper())
            {
                NotifyDataSetChanged(e);
            }
            else
            {
                MvxAndroidLog.Instance.Log(LogLevel.Error,
                    $@"ItemsSource collection content changed on a worker thread.
This leads to crash in the RecyclerView as it will not be aware of changes
immediatly and may get a deleted item or update an item with a bad item template.
All changes must be synchronized on the main thread.");
            }
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
            subscription?.Dispose();
            subscription = null;
            itemSwipeLeft = null;
            itemSwipeRight = null;
            itemTemplateSelector = null;
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public virtual bool OnMove(RecyclerView.ViewHolder fromViewHolder, RecyclerView.ViewHolder toViewHolder)
        {
            int toPosition = toViewHolder.AdapterPosition;
            int fromPosition = fromViewHolder.AdapterPosition;

            if (toPosition <= 0 || fromPosition == toPosition)
            {
                return false;
            }

            isDragging = true;

            ITaskItem item = viewItemsSource[fromPosition];
            viewItemsSource.Remove(item);
            viewItemsSource.Insert(toPosition, item);
            NotifyItemMoved(fromPosition, toPosition);
            return true;
        }

        /// <inheritdoc/>
        public virtual void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            if (viewHolder is IMvxRecyclerViewHolder holder)
            {
                if (direction == ItemTouchHelper.Left)
                {
                    if (ItemSwipeLeft == null)
                    {
                        throw new ArgumentNullException(nameof(ItemSwipeLeft), $"Either implement swipe feature and bind to {nameof(ItemSwipeLeft)} or disable swiping towards start direction.");
                    }

                    ExecuteCommandOnItem(ItemSwipeLeft, holder.DataContext);
                }
                else if (direction == ItemTouchHelper.Right)
                {
                    if (ItemSwipeRight == null)
                    {
                        throw new ArgumentNullException(nameof(ItemSwipeRight), $"Either implement swipe feature and bind to {nameof(ItemSwipeRight)} or disable swiping towards end direction.");
                    }

                    ExecuteCommandOnItem(ItemSwipeRight, holder.DataContext);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            // TODO: Implement.
        }

        /// <inheritdoc/>
        public virtual void OnClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            isDragging = false;

            int itemPosition = viewHolder.AdapterPosition;

            if (itemPosition <= 0)
            {
                selectedItem = null;
                return;
            }

            ITaskItem item = viewItemsSource[itemPosition];
            selectedItem = item;
            ITaskItem aboveItem = viewItemsSource[itemPosition - 1];

            ITaskHeader previousHeader = GetHeader(item);
            ITaskHeader newHeader = GetHeader(aboveItem);

            if (previousHeader.Header.Equals(newHeader.Header))
            {
                int sequence = itemPosition - viewItemsSource.GetPosition(previousHeader) - 1;
                item.Sequence = sequence;
                previousHeader.Items.Remove(item);
                previousHeader.Items.Insert(sequence, item);
                return;
            }

            if (TaskHeaderDragInDisabled(newHeader, item, recyclerView))
            {
                return;
            }

            item.Header = newHeader.Header;
            previousHeader.Items.Remove(item);

            if (newHeader.IsCollapsed)
            {
                item.Sequence = null;
                newHeader.Items.Add(item);
                viewItemsSource.Remove(item);
                Notify(recyclerView, NotifyItemRemoved, itemPosition);
            }
            else
            {
                int sequence = itemPosition - viewItemsSource.GetPosition(newHeader) - 1;
                item.Sequence = sequence;
                newHeader.Items.Insert(sequence, item);
                Notify(recyclerView, NotifyItemChanged, itemPosition);
            }

            RemoveTemporaryTaskHeader(previousHeader, recyclerView);
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
        /// Get header associated with item at position.
        /// </summary>
        /// <param name="position">Position of item to use to find header.</param>
        /// <returns>Header associated with the item.</returns>
        public ITaskHeader GetHeaderAt(int position)
        {
            return GetHeader(viewItemsSource[position]);
        }

        /// <summary>
        /// Get position of header associated with item at position.
        /// </summary>
        /// <param name="position">Position of item to use to find header position.</param>
        /// <returns>Position of header.</returns>
        public int GetHeaderPosition(int position)
        {
            ITaskHeader heaader = GetHeaderAt(position);
            return viewItemsSource.GetPosition(heaader);
        }

        /// <summary>
        /// Check if item at positon is a header.
        /// </summary>
        /// <param name="position">Position of item.</param>
        /// <returns><c>true</c> if item is header. Otherwise <c>false</c>.</returns>
        public bool IsHeader(int position)
        {
            return viewItemsSource[position] is ITaskHeader;
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

        /// <inheritdoc/>
        public void OnHeaderClick(ITaskHeader header)
        {
            int position = viewItemsSource.GetPosition(header) + 1;

            if (header.IsCollapsed)
            {
                var itemsToAdd = header.Items;
                int pos = position;
                foreach (ITaskItem item in itemsToAdd)
                {
                    viewItemsSource.Insert(pos++, item);
                }
                header.IsCollapsed = false;
                NotifyItemRangeInserted(position, itemsToAdd.Count);
            }
            else
            {
                var itemsToRemove = header.Items;
                foreach (ITaskItem item in itemsToRemove)
                {
                    viewItemsSource.Remove(item);
                }
                header.IsCollapsed = true;
                NotifyItemRangeRemoved(position, itemsToRemove.Count);
            }
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

            Dictionary<ITaskHeader, List<ITaskItem>> sortedHeaderAndItems = new Dictionary<ITaskHeader, List<ITaskItem>>();

            foreach (var group in groupedItems)
            {
                ITaskHeader generatedHeader = GenerateHeader((THeader)group.Key);

                if (group.Key == null)
                {
                    foreach (ITaskItem item in group)
                    {
                        item.Header = generatedHeader.Header;
                    }
                }

                if (sortedHeaderAndItems.Keys.Any(h => h.Header.Equals(generatedHeader.Header)))
                {
                    sortedHeaderAndItems.First(h => h.Key.Header.Equals(generatedHeader.Header)).Value.AddRange(group);
                }
                else
                {
                    sortedHeaderAndItems[generatedHeader] = new List<ITaskItem>(group);
                }
            }

            foreach (var headerItems in sortedHeaderAndItems)
            {
                headerItems.Key.Items.ReplaceWith(headerItems.Value.OrderByDescending(i => i.Sequence.HasValue).ThenBy(i => i.Sequence));
                taskItems.Add(headerItems.Key);
                if (!headerItems.Key.IsCollapsed)
                {
                    taskItems.AddRange(headerItems.Key.Items);
                }
            }

            return taskItems;
        }

        /// <summary>
        /// Generate a header with a given model.
        /// <para>This method can be overriden to fine-tune the grouping logic to add/edit/group headers and/or specify header rules.</para>
        /// <para>IMPORTANT: If header is nullable, make sure to handle nullable type by using a non-null value. E.g. Header type with <c>int?</c> will use <c>-1</c> when <c>null</c>.</para>
        /// </summary>
        /// <param name="model">Model used for generating header.</param>
        /// <returns>Header object used in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.</returns>
        protected virtual ITaskHeader GenerateHeader(THeader model)
        {
            if (model == null)
            {
                MvxAndroidLog.Instance.Log(LogLevel.Error, $@"The {nameof(model)} for header was null. Binding to ItemsSource will fail because grouping cannot be applied to null values.
If header is nullable, make sure to override {nameof(GenerateHeader)}() and handle nullable types by using a non-null value. E.g. Header type with int? will use -1 when null.");
            }

            return new SimpleTaskHeader<THeader>(model?.ToString(), model);
        }

        private void OnHeaderViewClick(object sender, EventArgs e)
        {
            if (sender is MvxRecyclerViewHolder holder && holder.DataContext is ITaskHeader header)
            {
                OnHeaderClick(header);
            }
        }

        private void NotifyDataSetAdded(NotifyCollectionChangedEventArgs e)
        {
            IList<ITaskItem> newItems = e.NewItems.Cast<ITaskItem>().ToList();
            foreach (ITaskItem item in newItems)
            {
                ITaskHeader generatedHeader = GenerateHeader((THeader)item.Header);

                ITaskHeader header = AddHeader(generatedHeader) ? generatedHeader : GetHeader(generatedHeader);

                int headerPosition = viewItemsSource.GetPosition(header);

                if (header.IsCollapsed)
                {
                    if (item.Sequence.HasValue && item.Sequence.Value <= header.Items.Count)
                    {
                        header.Items.Insert(item.Sequence.Value, item);
                    }
                    else
                    {
                        header.Items.Add(item);
                    }
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
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                NotifyItemMoved(GetViewPosition(e.OldStartingIndex + i), GetViewPosition(e.NewStartingIndex + i));
            }
        }

        private void NotifyDataSetReplaced(NotifyCollectionChangedEventArgs e)
        {
            NotifyItemRangeChanged(GetViewPosition(e.NewStartingIndex), e.NewItems.Count);
        }

        private void NotifyDataSetRemoved(NotifyCollectionChangedEventArgs e)
        {
            IList<ITaskItem> oldItems = e.OldItems.Cast<ITaskItem>().ToList();
            foreach (ITaskItem item in oldItems)
            {
                item.Sequence = null;
                ITaskHeader header = GetHeader(item);
                header.Items.Remove(item);

                if (!header.IsCollapsed)
                {
                    int position = viewItemsSource.GetPosition(item);
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
            {
                viewItemsSource = new List<ITaskItem>();
            }

            foreach (THeader initialHeader in AddInitialHeaders())
            {
                ITaskHeader header = GenerateHeader(initialHeader);
                AddHeader(header);
            }
        }

        private bool AddHeader(ITaskHeader header)
        {
            if (viewItemsSource.Any(h => h.Header.Equals(header.Header)))
            {
                return false;
            }

            viewItemsSource.Add(header);
            IList<ITaskHeader> headers = GetHeaders();

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
                ITaskHeader aboveHeader = orderedHeaders.ElementAt(aboveHeaderPosition);
                newHeaderPosition = viewItemsSource.GetPosition(aboveHeader) + CountVisibleItemsInHeader(aboveHeader) + 1;
            }

            viewItemsSource.Remove(header);
            viewItemsSource.Insert(newHeaderPosition, header);
            NotifyItemInserted(newHeaderPosition);
            return true;
        }

        private bool TaskHeaderDragInDisabled(ITaskHeader newHeader, ITaskItem item, RecyclerView recyclerView = null)
        {
            if (newHeader?.Rules.HasFlag(TaskHeaderRule.DragInDisabled) == true)
            {
                ITaskHeader currentHeader = GetHeader(item);
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
                Notify(recyclerView, NotifyItemMoved, position, newPosition);
                return true;
            }

            return false;
        }

        private bool RemoveTemporaryTaskHeader(ITaskItem item, RecyclerView recyclerView = null)
        {
            ITaskHeader itemHeader = GetHeader(item);
            if (itemHeader?.Rules.HasFlag(TaskHeaderRule.Temporary) == true)
            {
                var items = itemHeader.Items;
                if (items?.Count > 0)
                {
                    return false;
                }

                int position = viewItemsSource.GetPosition(itemHeader);
                viewItemsSource.Remove(itemHeader);
                Notify(recyclerView, NotifyItemRemoved, position);
                return true;
            }

            return false;
        }

        #region Helper methods to prevent edge-case scenario where app sometimes crashes when dragging item above header and RecyclerView is scrolling. This happens in the "OnClearView" method where "NotifyItem****" methods are called before layout is computed.

        /// <summary>
        /// This helper method prevents crashes when dataset has been modified before the RecyclerView has finished computing its layout.
        /// </summary>
        /// <param name="recyclerView"><see cref="RecyclerView"/>.</param>
        /// <param name="notify">Notify method.</param>
        /// <param name="position">Position of ViewHolder.</param>
        private void Notify(RecyclerView recyclerView, Action<int> notify, int position)
        {
            if (recyclerView?.IsComputingLayout == true)
            {
                recyclerView.Post(() => notify?.Invoke(position));
            }
            else
            {
                notify?.Invoke(position);
            }
        }

        /// <summary>
        /// This helper method prevents crashes when dataset range has been modified before the RecyclerView has finished computing its layout.
        /// </summary>
        /// <param name="recyclerView"><see cref="RecyclerView"/>.</param>
        /// <param name="notifyRange">Notify range method.</param>
        /// <param name="pos0">First parameter for given method.</param>
        /// <param name="pos1">Second parameter for given method.</param>
        private void Notify(RecyclerView recyclerView, Action<int, int> notifyRange, int pos0, int pos1)
        {
            if (recyclerView?.IsComputingLayout == true)
            {
                recyclerView.Post(() => notifyRange?.Invoke(pos0, pos1));
            }
            else
            {
                notifyRange?.Invoke(pos0, pos1);
            }
        }

        #endregion
    }
}