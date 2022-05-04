using AndroidX.RecyclerView.Widget;
using MvvmCross.DroidX.RecyclerView;

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
            if (p1 is IMvxRecyclerViewHolder holder && holder.DataContext is ITaskHeader)
            {
                return 0;
            }

            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
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
    }
}