using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MvvmCross.ExpandableRecyclerView.Core
{
    /// <summary>
    /// Abstract class used for items in MvxExpandableRecyclerView.
    /// </summary>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="THeader">Type of header.</typeparam>
    public abstract class TaskItem<TModel, THeader> : ITaskItem<TModel, THeader>
    {
        private int? sequence;
        private bool isSelected;
        private bool isHighlighted;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">Data to use.</param>
        protected TaskItem(TModel model)
        {
            Model = model;
        }

        /// <inheritdoc/>
        public TModel Model { get; }

        /// <summary>
        /// <inheritdoc/>
        /// <para>You will need to override <see cref="Header"/> and assign a property from <see cref="Model"/> or the <see cref="Model"/> itself.</para>
        /// </summary>
        public abstract THeader Header { get; set; }

        /// <inheritdoc/>
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        /// <inheritdoc/>
        public bool IsHighlighted { get => isHighlighted; set => SetProperty(ref isHighlighted, value); }

        /// <inheritdoc/>
        public int? Sequence { get => sequence; set => SetProperty(ref sequence, value); }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <inheritdoc/>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> raiser)
        {
            string propName = ((MemberExpression)raiser?.Body).Member.Name;
            OnPropertyChanged(propName);
        }

        /// <inheritdoc/>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        object ITaskItem.Header { get => Header; set => Header = (THeader)value; }

        /// <inheritdoc/>
        object ITaskItem.Model => Model;
    }
}
