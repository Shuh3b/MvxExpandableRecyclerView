using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.Logging;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Exceptions;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// View for displaying and "sticking" top most available header to the top of the screen.
    /// </summary>
    public class StickyHeaderView : FrameLayout, IMvxBindingContextOwner
    {
        private readonly IMvxAndroidBindingContext bindingContext;
        private int templateId;
        private ITaskHeader cachedDataContext;
        private bool isAttachedToWindow;
        private View content;

        /// <summary>
        /// Create an instance of StickyHeaderView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public StickyHeaderView(Context context, IAttributeSet attrs)
            : this(MvxAttributeHelpers.ReadTemplateId(context, attrs), context, attrs)
        { }

        /// <summary>
        /// Create an instance of StickyHeaderView.
        /// </summary>
        /// <param name="templateId">View Type Id.</param>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public StickyHeaderView(int templateId, Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            this.templateId = templateId;

            if (!(context is IMvxLayoutInflaterHolder))
            {
                throw new MvxException("The owning Context for a StickyHeaderView must implement LayoutInflater");
            }

            bindingContext = new MvxAndroidBindingContext(context, (IMvxLayoutInflaterHolder)context);
            this.DelayBind(() =>
            {
                if (Content == null && this.templateId != 0)
                {
                    MvxLogHost.GetLog<StickyHeaderView>()?.Log(LogLevel.Trace, "DataContext is {dataContext}", DataContext?.ToString() ?? "Null");
                    Content = bindingContext.BindingInflate(this.templateId, this);
                }
            });
        }

        /// <inheritdoc/>
        protected StickyHeaderView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        /// <summary>
        /// Android Binding Context.
        /// </summary>
        protected IMvxAndroidBindingContext AndroidBindingContext => bindingContext;

        /// <summary>
        /// Binding Context.
        /// </summary>
        public IMvxBindingContext BindingContext
        {
            get => bindingContext;
            set => throw new NotImplementedException("BindingContext is readonly in the list item");
        }

        /// <summary>
        /// View Type Id.
        /// </summary>
        public int TemplateId
        {
            get => templateId;
            set
            {
                templateId = value;
                if (templateId != 0)
                {
                    MvxLogHost.GetLog<StickyHeaderView>()?.Log(LogLevel.Trace, "DataContext is {dataContext}", DataContext?.ToString() ?? "Null");
                    this.ClearAllBindings();
                    RemoveAllViews();
                    Content = bindingContext.BindingInflate(templateId, this);
                }
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearAllBindings();
                if (cachedDataContext != null)
                {
                    cachedDataContext = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            isAttachedToWindow = true;
            if (cachedDataContext != null && DataContext == null)
            {
                DataContext = cachedDataContext;
            }
        }

        /// <inheritdoc/>
        protected override void OnDetachedFromWindow()
        {
            cachedDataContext = DataContext;
            DataContext = null;
            base.OnDetachedFromWindow();
            isAttachedToWindow = false;
        }

        /// <summary>
        /// Inflated view created using the given DataContext and TemplateId.
        /// </summary>
        protected View Content
        {
            get => content;
            set
            {
                content = value;
                OnContentSet();
            }
        }

        /// <summary>
        /// Set Content.
        /// </summary>
        protected virtual void OnContentSet()
        { }

        /// <summary>
        /// Header attached to this view.
        /// </summary>
        [MvxSetToNullAfterBinding]
        public ITaskHeader DataContext
        {
            get => bindingContext.DataContext as ITaskHeader;
            set
            {
                if (isAttachedToWindow)
                {
                    bindingContext.DataContext = value;
                }
                else
                {
                    cachedDataContext = value;
                    if (bindingContext.DataContext != null)
                    {
                        bindingContext.DataContext = null;
                    }
                }
            }
        }
    }
}