using Android.Content;
using Android.Runtime;
using Android.Util;
using MvvmCross.Binding.Attributes;
using MvvmCross.DroidX.RecyclerView;
using System;
using System.Windows.Input;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// A bindable, expandable, draggable and swipe-able RecyclerView.
    /// </summary>
    public class MvxExpandableRecyclerView : MvxRecyclerView
    {
        public MvxExpandableRecyclerView(Context context, IAttributeSet attrs) 
            : base(context, attrs, 0, new MvxExpandableRecyclerAdapter<object>())
        {
        }

        public MvxExpandableRecyclerView(Context context, IAttributeSet attrs, int defStyle) 
            : base(context, attrs, defStyle, new MvxExpandableRecyclerAdapter<object>())
        {
        }

        public MvxExpandableRecyclerView(Context context, IAttributeSet attrs, int defStyle, IMvxExpandableRecyclerAdapter adapter) 
            : base(context, attrs, defStyle, adapter)
        {
        }

        protected MvxExpandableRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
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
                var existing = Adapter;
                if (existing == value)
                    return;

                if (value != null && existing != null)
                {
                    value.ItemsSource = existing.ItemsSource;
                    value.ItemTemplateSelector = existing.ItemTemplateSelector;
                    value.ItemClick = existing.ItemClick;
                    value.ItemLongClick = existing.ItemLongClick;
                    value.ItemSwipeStart = existing.ItemSwipeStart;
                    value.ItemSwipeEnd = existing.ItemSwipeEnd;

                    SwapAdapter((Adapter)value, false);
                }
                else
                {
                    SetAdapter(value as Adapter);
                }

                if (existing != null)
                {
                    existing.ItemsSource = null;
                }
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the start direction.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeStart
        {
            get => Adapter?.ItemSwipeStart;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemSwipeStart = value;
                }
            }
        }

        /// <summary>
        /// Get or set the <see cref="ICommand"/> to trigger when an item was swiped towards the end direction.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ICommand ItemSwipeEnd
        {
            get => Adapter?.ItemSwipeEnd;
            set
            {
                if (Adapter != null)
                {
                    Adapter.ItemSwipeEnd = value;
                }
            }
        }
    }
}