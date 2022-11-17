using Android.OS;
using System;

namespace MvvmCross.ExpandableRecyclerView.DroidX.Components
{
    /// <summary>
    /// Generic class that will create an instance of <see cref="IParcelable"/>.
    /// </summary>
    /// <typeparam name="TParcelable">Type of parcelable object.</typeparam>
    internal class ParcelableCreator<TParcelable> : Java.Lang.Object, IParcelableCreator where TParcelable : Java.Lang.Object, new()
    {
        /// <summary>
        /// Function for the creation of a parcel.
        /// </summary>
        private readonly Func<Parcel, TParcelable> createFunc;

        /// <summary>
        /// Initialize an instance of the GenericParcelableCreator.
        /// </summary>
        public ParcelableCreator(Func<Parcel, TParcelable> createFromParcelFunc)
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
        public Java.Lang.Object[] NewArray(int size) => new TParcelable[size];
    }
}
