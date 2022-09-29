using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.RecyclerView.Widget;
using MvvmCross.Binding.Attributes;
using MvvmCross.DroidX.RecyclerView.AttributeHelpers;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.ExpandableRecyclerView.DroidX.Components;
using MvvmCross.Platforms.Android.Binding.Views;
using System;
using System.Collections;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// A bindable, expandable, draggable and swipe-able RecyclerView.
    /// </summary>
    public class MvxExpandableRecyclerBaseView : RecyclerView
    {
        private readonly StickyHeaderLayoutManager stickyHeaderLayoutManager;
        private readonly ItemTouchHelperCallback itemTouchHelperCallback;

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerBaseView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public MvxExpandableRecyclerBaseView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0, new MvxExpandableRecyclerAdapter<object>())
        { }

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerBaseView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        /// <param name="defStyle">DefStyle.</param>
        public MvxExpandableRecyclerBaseView(Context context, IAttributeSet attrs, int defStyle)
            : this(context, attrs, defStyle, new MvxExpandableRecyclerAdapter<object>())
        { }

        /// <summary>
        /// Create an instance of MvxExpandableRecyclerBaseView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        /// <param name="defStyle">DefStyle.</param>
        /// <param name="adapter"><see cref="IMvxExpandableRecyclerAdapter"/> to use.</param>
        public MvxExpandableRecyclerBaseView(Context context, IAttributeSet attrs, int defStyle, IMvxExpandableRecyclerAdapter adapter)
            : base(context, attrs, defStyle)
        {
            if (adapter == null)
            {
                return;
            }

            LayoutManager currentLayoutManager = GetLayoutManager();
            if (currentLayoutManager == null)
            {
                stickyHeaderLayoutManager = new StickyHeaderLayoutManager(context, attrs, adapter);
                SetLayoutManager(stickyHeaderLayoutManager);
            }

            itemTouchHelperCallback = new ItemTouchHelperCallback(adapter);
            ItemTouchHelper itemtouchhelper = new ItemTouchHelper(itemTouchHelperCallback);
            itemtouchhelper.AttachToRecyclerView(this);

            int itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
            IMvxTemplateSelector itemTemplateSelector = MvxRecyclerViewAttributeExtensions.BuildItemTemplateSelector(context, attrs, itemTemplateId);

            adapter.ItemTemplateSelector = itemTemplateSelector;
            Adapter = adapter;

            if (itemTemplateId == 0)
            {
                itemTemplateId = global::Android.Resource.Layout.SimpleListItem1;
            }

            if (itemTemplateSelector.GetType() == typeof(MvxDefaultTemplateSelector))
            {
                ItemTemplateId = itemTemplateId;
            }
        }

        /// <inheritdoc/>
        protected MvxExpandableRecyclerBaseView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        /// <inheritdoc/>
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            DetachedFromWindow();
        }

        /// <inheritdoc/>
        protected virtual void DetachedFromWindow()
        {
            GetLayoutManager()?.RemoveAllViews();
        }

        /// <summary>
        /// Adapter.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public new IMvxExpandableRecyclerAdapter Adapter
        {
            get => GetAdapter() as IMvxExpandableRecyclerAdapter;
            set
            {
                IMvxExpandableRecyclerAdapter existing = Adapter;
                if (existing == value)
                {
                    return;
                }

                if (value != null && existing != null)
                {
                    value.ItemsSource = existing.ItemsSource;
                    value.ItemTemplateSelector = existing.ItemTemplateSelector;
                    value.ItemClick = existing.ItemClick;
                    value.ItemLongClick = existing.ItemLongClick;
                    value.ItemSwipeLeft = existing.ItemSwipeLeft;
                    value.ItemSwipeRight = existing.ItemSwipeRight;
                    value.EnableDrag = existing.EnableDrag;
                    value.EnableSwipe = existing.EnableSwipe;

                    SwapAdapter((Adapter)value, false);
                }
                else
                {
                    SetAdapter(value as Adapter);
                }

                stickyHeaderLayoutManager.Adapter = value;
                itemTouchHelperCallback.Adapter = value;

                if (existing != null)
                {
                    existing.ItemsSource = null;
                }
            }
        }

        /// <summary>
        /// Get or set the ItemSource to use for the RecyclerView Adapter.
        /// <para>
        /// It is recommended to use a type inheriting from <see cref="IList"/>, such as
        /// <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>,
        /// <see cref="MvvmCross.ViewModels.MvxObservableCollection{T}"/> or
        /// <see cref="System.Collections.Generic.List{T}"/>.
        /// </para>
        /// </summary>
        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get => Adapter?.ItemsSource;
            set
            {
                IMvxExpandableRecyclerAdapter adapter = Adapter;
                if (adapter != null)
                {
                    adapter.ItemsSource = value;
                }
            }
        }

        /// <summary>
        /// Get or set the Item Template Id for cases where you only have one type of view in the RecyclerView.
        /// </summary>
        public int ItemTemplateId
        {
            get
            {
                if (!(ItemTemplateSelector is MvxDefaultTemplateSelector singleItemDefaultTemplateSelector))
                {
                    throw new InvalidOperationException(
                        "If you don't want to use single item-template RecyclerView Adapter you can't change it's" +
                        $"{nameof(IMvxTemplateSelector)} to anything other than {nameof(MvxDefaultTemplateSelector)}");
                }

                return singleItemDefaultTemplateSelector.ItemTemplateId;
            }
            set
            {
                if (!(ItemTemplateSelector is MvxDefaultTemplateSelector singleItemDefaultTemplateSelector))
                {
                    throw new InvalidOperationException(
                        "If you don't want to use single item-template RecyclerView Adapter you can't change it's" +
                        $"{nameof(IMvxTemplateSelector)} to anything other than {nameof(MvxDefaultTemplateSelector)}");
                }

                singleItemDefaultTemplateSelector.ItemTemplateId = value;

                if (Adapter != null)
                {
                    Adapter.ItemTemplateSelector = singleItemDefaultTemplateSelector;
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
            get => stickyHeaderLayoutManager.ShowStickyHeader;
            set => stickyHeaderLayoutManager.ShowStickyHeader = value;
        }

        /// <summary>
        /// Whether to enable RecyclerView dragging.
        /// </summary>
        public bool EnableDrag
        {
            get => Adapter?.EnableDrag ?? false;
            set
            {
                if (Adapter != null)
                {
                    Adapter.EnableDrag = value;
                }
            }
        }

        /// <summary>
        /// Whether to enable RecyclerView swiping.
        /// </summary>
        public bool EnableSwipe
        {
            get => Adapter?.EnableSwipe ?? false;
            set
            {
                if (Adapter != null)
                {
                    Adapter.EnableSwipe = value;
                }
            }
        }
    }
}