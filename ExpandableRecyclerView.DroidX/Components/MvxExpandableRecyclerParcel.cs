using Android.OS;
using Android.Runtime;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// <see cref="IParcelable"/> used to save and restore state of <see cref="MvxExpandableRecyclerBaseView"/> when rotating device.
    /// </summary>
    internal class MvxExpandableRecyclerParcel : Object, IParcelable
    {
        public MvxExpandableRecyclerParcel()
        { }

        public MvxExpandableRecyclerParcel(Parcel parcel)
        {
            SuperState = (IParcelable)parcel.ReadParcelable(Class.ClassLoader);
            StickyHeaderPosition = parcel.ReadInt();
            ShowStickyHeader = parcel.ReadBoolean();
            parcel.ReadList(Headers.ToList(), Class.ClassLoader);
        }

        public IParcelable SuperState { get; set; }

        public int StickyHeaderPosition { get; set; }

        public bool ShowStickyHeader { get; set; }

        public IList<ITaskHeader> Headers { get; set; }

        public int DescribeContents() => 0;

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            try
            {
                dest.WriteParcelable(SuperState, flags);
                dest.WriteInt(StickyHeaderPosition);
                dest.WriteBoolean(ShowStickyHeader);
                dest.WriteList(Headers.ToList());
            }
            catch
            {
                // TODO: A null exception occurs when phone screen turns off. Fix bug and remove try-catch.
            }
        }
    }
}