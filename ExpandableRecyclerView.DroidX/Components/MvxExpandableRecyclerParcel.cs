using Android.OS;
using Android.Runtime;
using Java.Interop;
using Java.Lang;
using System.Collections.Generic;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// <see cref="IParcelable"/> used to save and restore state of <see cref="MvxExpandableRecyclerBaseView"/> when rotating device.
    /// </summary>
    internal class MvxExpandableRecyclerParcel : Object, IParcelable
    {
        private static readonly ParcelableCreator<MvxExpandableRecyclerParcel> creator = new ParcelableCreator<MvxExpandableRecyclerParcel>((parcel) => new MvxExpandableRecyclerParcel(parcel));

        [ExportField("CREATOR")]
        public static ParcelableCreator<MvxExpandableRecyclerParcel> GetCreator() => creator;

        public MvxExpandableRecyclerParcel()
        { }

        public MvxExpandableRecyclerParcel(Parcel parcel)
        {
            SuperState = (IParcelable)parcel.ReadParcelable(Class.ClassLoader);
            StickyHeaderPosition = parcel.ReadInt();
            ShowStickyHeader = parcel.ReadInt() == 1;
            Bundle bundle = parcel.ReadBundle();
            if (bundle != null)
            {
                int count = bundle.GetInt("count", 0);
                IDictionary<int, bool> headers = new Dictionary<int, bool>();

                for (int i = 0; i < count; i++)
                {
                    headers[i] = bundle.GetBoolean(i.ToString());
                }
                Headers = headers;
            }
        }

        public IParcelable SuperState { get; set; }

        public int StickyHeaderPosition { get; set; }

        public bool ShowStickyHeader { get; set; }

        public IDictionary<int, bool> Headers { get; set; }

        public int DescribeContents() => 0;

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteParcelable(SuperState, flags);
            dest.WriteInt(StickyHeaderPosition);
            dest.WriteInt(ShowStickyHeader ? 1 : 0);

            Bundle bundle = new Bundle();
            bundle.PutInt("count", Headers?.Count ?? 0);
            foreach (KeyValuePair<int, bool> header in Headers)
            {
                bundle.PutBoolean(header.Key.ToString(), header.Value);
            }
            dest.WriteBundle(bundle);
        }
    }
}