﻿using AndroidX.RecyclerView.Widget;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ExpandableRecyclerView.Core;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// Helper class for dragging, swiping and updating items in <see cref="MvxExpandableRecyclerAdapter{THeader}"/>.
    /// </summary>
    public class ItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private IItemTouchHelperCallback adapter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="adapter">Adapter.</param>
        public ItemTouchHelperCallback(IItemTouchHelperCallback adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// <see cref="MvxExpandableRecyclerBaseView.Adapter"/>.
        /// </summary>
        public IItemTouchHelperCallback Adapter
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

        /// <inheritdoc/>
        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            if (!(p1 is IMvxRecyclerViewHolder))
            {
                return ItemTouchHelper.ActionStateIdle;
            }

            IMvxRecyclerViewHolder holder = (IMvxRecyclerViewHolder)p1;
            if (holder.DataContext is ITaskHeader)
            {
                return ItemTouchHelper.ActionStateIdle;
            }

            ITaskItem item = (ITaskItem)holder.DataContext;
            if (item == null)
            {
                return ItemTouchHelper.ActionStateIdle;
            }

            ITaskHeader header = adapter.GetHeader(item);
            if (header == null)
            {
                return ItemTouchHelper.ActionStateIdle;
            }

            int dragFlags = adapter.EnableDrag ? MakeDragFlags(header.Rules) : ItemTouchHelper.ActionStateIdle;
            int swipeFlags = adapter.EnableSwipe ? MakeSwipeFlags(header.Rules) : ItemTouchHelper.ActionStateIdle;

            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        /// <inheritdoc/>
        public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
        {
            return adapter.OnMove(p1, p2);
        }

        /// <inheritdoc/>
        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            adapter.OnSwiped(p0, p1);
        }

        /// <inheritdoc/>
        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            base.OnSelectedChanged(viewHolder, actionState);
            adapter.OnSelectedChanged(viewHolder, actionState);
        }

        /// <inheritdoc/>
        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);
            adapter.OnClearView(recyclerView, viewHolder);
        }

        private int MakeDragFlags(TaskHeaderRule rules)
        {
            if (rules.HasFlag(TaskHeaderRule.DragOutDisabled))
            {
                return ItemTouchHelper.ActionStateIdle;
            }

            return ItemTouchHelper.Up | ItemTouchHelper.Down;
        }

        private int MakeSwipeFlags(TaskHeaderRule rules)
        {
            if (rules.HasFlag(TaskHeaderRule.SwipeLeftDisabled) && rules.HasFlag(TaskHeaderRule.SwipeRightDisabled))
            {
                return ItemTouchHelper.ActionStateIdle;
            }
            else if (rules.HasFlag(TaskHeaderRule.SwipeLeftDisabled))
            {
                return ItemTouchHelper.Right;
            }
            else if (rules.HasFlag(TaskHeaderRule.SwipeRightDisabled))
            {
                return ItemTouchHelper.Left;
            }
            else
            {
                return ItemTouchHelper.Left | ItemTouchHelper.Right;
            }
        }
    }
}