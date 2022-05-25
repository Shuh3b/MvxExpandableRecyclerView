using AndroidX.RecyclerView.Widget;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ExpandableRecyclerView.Core;

namespace MvvmCross.ExpandableRecyclerView.DroidX
{
    /// <summary>
    /// Helper class for dragging, swiping and updating items in <see cref="MvxExpandableRecyclerView"/>.
    /// </summary>
    public class ItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private readonly IItemTouchHelperCallback adapter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="adapter">Adapter.</param>
        public ItemTouchHelperCallback(IItemTouchHelperCallback adapter)
        {
            this.adapter = adapter;
        }

        /// <inheritdoc/>
        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            if (!(p1 is IMvxRecyclerViewHolder))
                return ItemTouchHelper.ActionStateIdle;

            var holder = (IMvxRecyclerViewHolder)p1;
            if (holder.DataContext is ITaskHeader)
                return ItemTouchHelper.ActionStateIdle;

            var item = (ITaskItem)holder.DataContext;
            if (item == null)
                return ItemTouchHelper.ActionStateIdle;

            var header = adapter.GetHeader(item);
            if (header == null)
                return ItemTouchHelper.ActionStateIdle;

            int dragFlags = MakeDragFlags(header.Rules);
            int swipeFlags = MakeSwipeFlags(header.Rules);

            return MakeMovementFlags(dragFlags, swipeFlags);
        }


        /// <inheritdoc/>
        public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
        {
            adapter.OnMove(p1, p2);
            return true;
        }

        /// <inheritdoc/>
        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            adapter.OnSwiped(p0, p1);
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
                return ItemTouchHelper.ActionStateIdle;

            return ItemTouchHelper.Up | ItemTouchHelper.Down;
        }

        private int MakeSwipeFlags(TaskHeaderRule rules)
        {
            if (rules.HasFlag(TaskHeaderRule.SwipeStartDisabled) && rules.HasFlag(TaskHeaderRule.SwipeEndDisabled))
                return ItemTouchHelper.ActionStateIdle;
            else if (rules.HasFlag(TaskHeaderRule.SwipeStartDisabled))
                return ItemTouchHelper.End;
            else if (rules.HasFlag(TaskHeaderRule.SwipeEndDisabled))
                return ItemTouchHelper.Start;
            else
                return ItemTouchHelper.Start | ItemTouchHelper.End;
        }
    }
}