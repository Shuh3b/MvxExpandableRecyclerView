using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// Linear Layout Manager for handling sticky header behaviour for <see cref="MvxExpandableRecyclerBaseView"/>.
    /// </summary>
    public class StickyHeaderLayoutManager : LinearLayoutManager
    {
        private readonly StickyHeaderView stickyHeaderView;
        private IStickyHeaderHelper adapter;
        private int stickyHeaderPosition;
        private bool showStickyHeader = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        /// <param name="adapter">Adapter.</param>
        public StickyHeaderLayoutManager(Context context, IAttributeSet attrs, IStickyHeaderHelper adapter)
            : base(context, Vertical, false)
        {
            this.adapter = adapter;
            stickyHeaderView = new StickyHeaderView(context, attrs);
        }

        /// <inheritdoc/>
        protected StickyHeaderLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        /// <summary>
        /// <see cref="MvxExpandableRecyclerBaseView.Adapter"/>.
        /// </summary>
        public IStickyHeaderHelper Adapter
        {
            get => adapter;
            set
            {
                if (adapter != value)
                {
                    adapter = value;
                }
            }
        }

        /// <summary>
        /// Whether to show or hide sticky header.
        /// </summary>
        public bool ShowStickyHeader
        {
            get => showStickyHeader;
            set
            {
                if (showStickyHeader != value && stickyHeaderView != null)
                {
                    showStickyHeader = value;
                    stickyHeaderView.Visibility = showStickyHeader ? ViewStates.Visible : ViewStates.Gone;
                }
            }
        }

        /// <inheritdoc/>
        public override void OnAttachedToWindow(RecyclerView view)
        {
            ValidateParentView(view);
            ViewGroup parent = (ViewGroup)view.Parent;
            parent.AddView(stickyHeaderView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
            InitStickyHeader();

            stickyHeaderView.Click -= OnHeaderViewClick;
            stickyHeaderView.LongClick -= OnHeaderViewLongClick;
            stickyHeaderView.Click += OnHeaderViewClick;
            stickyHeaderView.LongClick += OnHeaderViewLongClick;

            base.OnAttachedToWindow(view);
        }

        /// <inheritdoc/>
        public override void OnDetachedFromWindow(RecyclerView view, RecyclerView.Recycler recycler)
        {
            if (stickyHeaderView != null)
            {
                stickyHeaderView.DataContext.IsSticky = false;
                stickyHeaderView.DataContext = null;
                stickyHeaderView.Click -= OnHeaderViewClick;
                stickyHeaderView.LongClick -= OnHeaderViewLongClick;
            }

            base.OnDetachedFromWindow(view, recycler);
        }

        /// <inheritdoc/>
        public override bool SupportsPredictiveItemAnimations() => false;

        /// <inheritdoc/>
        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            try
            {
                base.OnLayoutChildren(recycler, state);
            }
            catch (IndexOutOfBoundsException)
            { }

            if (showStickyHeader && !adapter.IsDragging)
            {
                UpdateStickyHeader();
                TranslateStickyHeader(); // Fixes edge-case where one header can overlap another, causing a visual glitch.
            }
        }

        /// <inheritdoc/>
        public override int ScrollVerticallyBy(int dy, RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            if (stickyHeaderView.TemplateId == 0)
            {
                InitStickyHeader();
            }

            int scroll = base.ScrollVerticallyBy(dy, recycler, state);
            if (showStickyHeader)
            {
                UpdateStickyHeaderState();
                TranslateStickyHeader();
            }

            return scroll;
        }

        /// <inheritdoc/>
        public override IParcelable OnSaveInstanceState()
        {
            IList<ITaskHeader> headers = adapter?.ItemCount > 0 ? adapter.GetHeaders() : new List<ITaskHeader>();
            IDictionary<int, bool> headerStates = new Dictionary<int, bool>();
            for (int i = 0; i < headers.Count; i++)
            {
                headerStates[i] = headers[i].IsCollapsed;
            }

            MvxExpandableRecyclerParcel parcel = new MvxExpandableRecyclerParcel
            {
                SuperState = base.OnSaveInstanceState(),
                StickyHeaderPosition = stickyHeaderPosition,
                ShowStickyHeader = showStickyHeader,
                Headers = headerStates,
            };

            return parcel;
        }

        /// <inheritdoc/>
        public override void OnRestoreInstanceState(IParcelable state)
        {
            MvxExpandableRecyclerParcel parcel = state as MvxExpandableRecyclerParcel;
            stickyHeaderPosition = parcel.StickyHeaderPosition;
            ShowStickyHeader = parcel.ShowStickyHeader;
            if (adapter?.ItemCount > 0)
            {
                IList<ITaskHeader> headers = adapter.GetHeaders();
                for (int i = 0; i < headers.Count; i++)
                {
                    if (parcel.Headers.ContainsKey(i) && parcel.Headers[i] != headers[i].IsCollapsed)
                    {
                        adapter.OnHeaderClick(headers[i], true);
                    }
                }
            }
            InitStickyHeader();

            base.OnRestoreInstanceState(parcel.SuperState);
        }

        private void ValidateParentView(RecyclerView view)
        {
            IViewParent parent = view.Parent;
            if (!(parent is FrameLayout) && !(parent is AndroidX.CoordinatorLayout.Widget.CoordinatorLayout))
            {
                throw new IllegalArgumentException("RecyclerView Parent must be either a FrameLayout or CoordinatorLayout");
            }
        }

        private void InitStickyHeader()
        {
            if (adapter?.ItemCount > 0)
            {
                int headerPosition = adapter.GetHeaderPosition(stickyHeaderPosition);
                int headerViewType = adapter.GetItemViewType(headerPosition);
                ITaskHeader dataContext = adapter.GetHeaderAt(headerPosition);
                stickyHeaderView.DataContext = dataContext;
                dataContext.IsSticky = true;
                stickyHeaderView.TemplateId = headerViewType;
            }
        }

        private void OnHeaderViewClick(object sender, EventArgs e)
        {
            if (adapter.ItemCount <= 0)
            {
                return;
            }

            int position = FindFirstVisibleItemPosition();
            if (position == RecyclerView.NoPosition)
            {
                return;
            }

            int headerPosition = adapter.GetHeaderPosition(position);
            ITaskHeader header = adapter.GetHeaderAt(headerPosition);
            adapter.OnHeaderClick(header);
            ScrollToPosition(headerPosition);
        }

        private void OnHeaderViewLongClick(object sender, EventArgs e)
        {
            if (adapter.ItemCount <= 0)
            {
                return;
            }

            int position = FindFirstVisibleItemPosition();
            if (position == RecyclerView.NoPosition)
            {
                return;
            }

            int headerPosition = adapter.GetHeaderPosition(position);
            ITaskHeader header = adapter.GetHeaderAt(headerPosition);
            adapter.OnHeaderLongClick(header);
        }

        private void UpdateStickyHeaderState()
        {
            if (adapter.IsDragging)
            {
                if (stickyHeaderView.Visibility != ViewStates.Gone)
                {
                    stickyHeaderView.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                UpdateStickyHeader();
            }
        }

        private void UpdateStickyHeader()
        {
            if (adapter.ItemCount <= 0)
            {
                return;
            }

            if (stickyHeaderView.Visibility != ViewStates.Visible)
            {
                stickyHeaderView.Visibility = ViewStates.Visible;
            }

            int headerPosition = adapter.GetHeaderPosition(FindFirstVisibleItemPosition());
            if (headerPosition == RecyclerView.NoPosition || headerPosition == stickyHeaderPosition)
            {
                return;
            }

            stickyHeaderPosition = headerPosition;
            stickyHeaderView.DataContext.IsSticky = false;
            stickyHeaderView.DataContext = adapter.GetHeaderAt(headerPosition);
            stickyHeaderView.DataContext.IsSticky = true;

            int headerViewType = adapter.GetItemViewType(headerPosition);
            if (stickyHeaderView.TemplateId != headerViewType)
            {
                stickyHeaderView.TemplateId = headerViewType;
            }
        }

        private void TranslateStickyHeader()
        {
            if (adapter.ItemCount <= 0)
            {
                return;
            }

            int contactPoint = stickyHeaderView.Bottom;
            View childInContact = null;
            List<View> pendingChildren = new List<View>();
            bool isHeaderPending = false;

            for (int childPos = 0; childPos < ChildCount; childPos++)
            {
                View child = GetChildAt(childPos);
                if (child.Bottom > contactPoint && child.Top <= contactPoint)
                {
                    childInContact = child;
                    break;
                }

                if (child.Top > 0 && child.Bottom < contactPoint && (adapter.IsHeader(GetPosition(child)) || isHeaderPending))
                {
                    isHeaderPending = true;
                    pendingChildren.Add(child);
                }
            }

            TranslateViewInContact(childInContact, isHeaderPending, pendingChildren);
        }

        private void TranslateViewInContact(View childInContact, bool isHeaderPending, IList<View> pendingChildren)
        {
            int adapterPosition = GetPosition(childInContact);
            if (adapterPosition != RecyclerView.NoPosition && adapterPosition <= ItemCount && (adapter.IsHeader(adapterPosition) || isHeaderPending))
            {
                int pendingHeight = 0;
                foreach (View child in pendingChildren)
                {
                    pendingHeight += child.Height;
                }
                stickyHeaderView.TranslationY = childInContact.Top - stickyHeaderView.Height - pendingHeight;
            }
            else
            {
                stickyHeaderView.TranslationY = 0;
            }
        }
    }
}