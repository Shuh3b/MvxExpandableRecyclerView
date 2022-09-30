using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MvvmCross.Binding.Attributes;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.ExpandableRecyclerView.DroidX.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// A view encompassing both <see cref="MvxExpandableRecyclerBaseView"/> and <see cref="StickyHeaderView"/>.
    /// </summary>
    public class MvxExpandableRecyclerView : FrameLayout
    {
        private readonly MvxExpandableRecyclerBaseView expandableRecyclerBaseView;

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public MvxExpandableRecyclerView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            expandableRecyclerBaseView = new MvxExpandableRecyclerBaseView(context, attrs);

            InitialiseRecyclerView();
        }

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        /// <param name="defStyle">DefStyle.</param>
        public MvxExpandableRecyclerView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            expandableRecyclerBaseView = new MvxExpandableRecyclerBaseView(context, attrs, defStyle);

            InitialiseRecyclerView();
        }

        /// <inheritdoc/>
        protected MvxExpandableRecyclerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        /// <summary>
        /// Getter for acessing <see cref="AndroidX.RecyclerView.Widget.RecyclerView"/>.
        /// </summary>
        public RecyclerView RecyclerView => expandableRecyclerBaseView;

        /// <summary>
        /// <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public IMvxExpandableRecyclerAdapter Adapter
        {
            get => expandableRecyclerBaseView?.Adapter;
            set
            {
                if (expandableRecyclerBaseView?.Adapter != null)
                {
                    expandableRecyclerBaseView.Adapter = value;
                }
            }
        }

        /// <summary>
        /// <para>Get or set the ItemSource to use for the RecyclerView Adapter.</para>
        /// <para>
        /// It is recommended to use a type inheriting from <see cref="IList{ITaskItem}"/>, such as
        /// <see cref="System.Collections.ObjectModel.ObservableCollection{ITaskItem}"/>,
        /// <see cref="MvvmCross.ViewModels.MvxObservableCollection{ITaskItem}"/> or
        /// <see cref="System.Collections.Generic.List{ITaskItem}"/>.
        /// </para>
        /// </summary>
        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get => Adapter?.ItemsSource;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemsSource = value;
                }
            }
        }

        /// <summary>
        /// Get or set the Item Template Id for cases where you only have one type of view in the RecyclerView.
        /// </summary>
        public int ItemTemplateId
        {
            get => expandableRecyclerBaseView?.ItemTemplateId ?? 0;
            set
            {
                if (expandableRecyclerBaseView != null)
                {
                    expandableRecyclerBaseView.ItemTemplateId = value;
                }
            }
        }

        /// <summary>
        /// Get or set the ItemTemplateSelector.
        /// </summary>
        public IMvxTemplateSelector ItemTemplateSelector
        {
            get => Adapter?.ItemTemplateSelector;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemTemplateSelector = value;
                }
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was clicked.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemClick
        {
            get => Adapter?.ItemClick;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemClick = value;
                }
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was long clicked.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemLongClick
        {
            get => Adapter?.ItemLongClick;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemLongClick = value;
                }
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the right.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeRight
        {
            get => Adapter?.ItemSwipeRight;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemSwipeRight = value;
                }
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the left.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeLeft
        {
            get => Adapter?.ItemSwipeLeft;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemSwipeLeft = value;
                }
            }
        }

        /// <summary>
        /// Whether to show or hide sticky header.
        /// </summary>
        public bool ShowStickyHeader
        {
            get => expandableRecyclerBaseView.ShowStickyHeader;
            set => expandableRecyclerBaseView.ShowStickyHeader = value;
        }

        /// <summary>
        /// Whether to enable RecyclerView dragging.
        /// </summary>
        public bool EnableDrag
        {
            get => expandableRecyclerBaseView.EnableDrag;
            set => expandableRecyclerBaseView.EnableDrag = value;
        }

        /// <summary>
        /// Whether to enable RecyclerView swiping.
        /// </summary>
        public bool EnableSwipe
        {
            get => expandableRecyclerBaseView.EnableSwipe;
            set => expandableRecyclerBaseView.EnableSwipe = value;
        }

        private void InitialiseRecyclerView()
        {
            if (expandableRecyclerBaseView.Id == Id)
            {
                expandableRecyclerBaseView.Id = Resource.Id.mvx_expandable_recycler_base_view;
            }

            AddView(expandableRecyclerBaseView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }
    }
}