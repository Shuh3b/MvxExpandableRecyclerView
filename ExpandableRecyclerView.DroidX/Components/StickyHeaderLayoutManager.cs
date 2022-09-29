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
using System.Linq;

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
            : base(context, OrientationHelper.Vertical, false)
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
            stickyHeaderView.Click += OnHeaderViewClick;

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
            }

            base.OnDetachedFromWindow(view, recycler);
        }

        /// <inheritdoc/>
        public override bool SupportsPredictiveItemAnimations() => false;

        /// <inheritdoc/>
        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            base.OnLayoutChildren(recycler, state);
            if (showStickyHeader && !adapter.IsDragging)
            {
                TranslateStickyHeader(); // Fixes edge-case where one header can overlap another, causing a visual glitch.
                UpdateStickyHeader();
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
                TranslateStickyHeader();
                UpdateStickyHeaderState();
            }

            return scroll;
        }

        /// <inheritdoc/>
        public override IParcelable OnSaveInstanceState()
        {
            MvxExpandableRecyclerParcel parcel = new MvxExpandableRecyclerParcel
            {
                SuperState = base.OnSaveInstanceState(),
                StickyHeaderPosition = stickyHeaderPosition,
                ShowStickyHeader = showStickyHeader,
                Headers = adapter.ItemCount > 0 ? adapter.GetHeaders() : new List<ITaskHeader>(),
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
                foreach (ITaskHeader header in parcel.Headers)
                {
                    ITaskHeader realHeader = adapter.GetHeader(header);
                    if (realHeader != null && realHeader.IsCollapsed != header.IsCollapsed)
                    {
                        adapter.OnHeaderClick(realHeader);
                    }
                }
            }

            base.OnRestoreInstanceState(parcel.SuperState);
        }

        private void ValidateParentView(RecyclerView view)
        {
            var parent = view.Parent;
            if (!(parent is FrameLayout) && !(parent is AndroidX.CoordinatorLayout.Widget.CoordinatorLayout))
            {
                throw new IllegalArgumentException("RecyclerView Parent must be either a FrameLayout or CoordinatorLayout");
            }
        }

        private void InitStickyHeader()
        {
            if (adapter.ItemCount > 0)
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
            bool isHeader = false;

            for (int childPos = 0; childPos < ChildCount; childPos++)
            {
                View child = GetChildAt(childPos);
                if (child.Bottom > contactPoint && child.Top <= contactPoint)
                {
                    childInContact = child;
                    break;
                }

                if (child.Top > 0 && child.Bottom <= contactPoint && (adapter.IsHeader(GetPosition(child)) || isHeader))
                {
                    isHeader = true;
                    pendingChildren.Add(child);
                }
            }

            int adapterPosition = GetPosition(childInContact);
            if (adapterPosition != RecyclerView.NoPosition && adapterPosition <= ItemCount && (adapter.IsHeader(adapterPosition) || pendingChildren.Any(i => adapter.IsHeader(GetPosition(i)))))
            {
                int height = 0;
                foreach (View child in pendingChildren)
                {
                    height += child.Height;
                }
                stickyHeaderView.TranslationY = childInContact.Top - stickyHeaderView.Height - height;
            }
            else
            {
                stickyHeaderView.TranslationY = 0;
            }
        }
    }
}