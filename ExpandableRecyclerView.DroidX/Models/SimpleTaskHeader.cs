using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Class used to show simple headers in <see cref="MvxExpandableRecyclerBaseView"/>.
    /// </summary>
    /// <typeparam name="TModel">Type of model used for header.</typeparam>
    public class SimpleTaskHeader<TModel> : TaskHeader<TModel, TModel>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="model">Header model.</param>
        public SimpleTaskHeader(string name, TModel model)
            : base(name, model)
        { }

        /// <inheritdoc/>
        public override TModel Header
        {
            get => Model;
            set => throw new InvalidOperationException("Header cannot be changed once set.");
        }
    }
}
