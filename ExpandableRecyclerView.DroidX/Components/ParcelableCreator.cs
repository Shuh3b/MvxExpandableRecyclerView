using Android.OS;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    internal class ParcelableCreator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object, new()
    {
        /// <summary>
        /// Function for the creation of a parcel.
        /// </summary>
        private readonly Func<Parcel, T> createFunc;

        /// <summary>
        /// Initialize an instance of the GenericParcelableCreator.
        /// </summary>
        public ParcelableCreator(Func<Parcel, T> createFromParcelFunc)
        {
            createFunc = createFromParcelFunc;
        }

        /// <summary>
        /// Create a parcelable from a parcel.
        /// </summary>
        public Java.Lang.Object CreateFromParcel(Parcel parcel) => createFunc(parcel);

        /// <summary>
        /// Create an array from the parcelable class.
        /// </summary>
        public Java.Lang.Object[] NewArray(int size) => new T[size];
    }
}
