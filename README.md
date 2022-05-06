# Android MvxExpandableRecyclerView

This is an unofficial package that contains an expandable AndroidX RecyclerView supported for MvvmCross. This view allows us to bind a collection of items (objects, ViewModels, etc) to the `ItemsSource` property. It works similarly to a RecyclerView. However, this comes with out-of-the-box functionality such as grouping items with collapsible and expandable headers. Additional functionality can be implemented such as dragging items up and down and swiping them by attaching the provided `ItemTouchHelperCallback` to the RecyclerView. All original functionality of `MvxRecyclerView` is also available and it is highly encouraged that you read the [documentation](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview) before proceeding.

## Getting Started

Firstly, you need to ensure that you have the [MvxExpandableRecyclerView.Core](https://www.nuget.org/packages/MvxExpandableRecyclerView.Core/) and [MvxExpandableRecyclerView.DroidX](https://www.nuget.org/packages/MvxExpandableRecyclerView.DroidX/) NuGet packages installed in your `.Core` and `.Droid`/`.DroidX` projects respectively.

The item(s) that you want to display must inherit `ITaskItem`. This lets the view know how to group each item and which header to place each item in.

The general steps you would take to implement the view:

1. In our `.Core` project, we create our data class, `Person.cs` with a `Name` and `Birthday`. The `Header` will use `Birthday.Date` to group the item under the corresponding header and `Model` will be the class itself.

```csharp
public class Person : ITaskItem
{
  public Person(string name, DateTime birthday)
  {
    Name = name;
    Birthday = birthday;
  }

  public string Name { get; set; }
  public DateTime Birthday { get; set; }

  // This uses the Birthday.Date to group Person.
  public object Header { get => Birthday.Date; set => Birthday = (DateTime)value; }
  
  // This should reference the Person itself.
  public object Model => this;

  public int? Sequence { get; set; }
  public bool IsHighlighted { get; set; }
}
```

2. For the rest of the steps, everything will be done in our `.Droid`/`.DroidX` project. We then create our view for Person: `PersonItem.xml`. This uses a `TextView` which binds to `Person.Name`.

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:orientation="horizontal"
  android:layout_width="match_parent"
  android:layout_height="wrap_content">
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:gravity="center"
    app:MvxBind="Text Name"/>
</LinearLayout>
```

3. We then create another view, if we want to display our headers using something other than a `SimpleListItem1`[^1]. In this example: `TaskHeader.xml` displays an `ImageView` with a star icon and uses a `TextView` to bind to `TaskHeader.Name`.

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:orientation="horizontal"
  android:layout_width="match_parent"
  android:layout_height="wrap_content">
  <ImageView
    android:layout_width="wrap_content"
    android:layout_height="match_parent"
    android:src="@android:drawable/star_on"/>
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    app:MvxBind="Text Name"/>
</LinearLayout>
```

4. We then create a custom [Item Template Selector](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview#using-an-item-template-selector) to handle the views that get displayed for the corresponding item(s). In this example, if an item doesn't have a corresponding view, it will default to `PersonItem` view.

```csharp
public class BirthdayTemplateSelector : IMvxTemplateSelector 
{
  public int ItemTemplateId { get; set; }
  
  public int GetItemLayoutId(int fromViewType) 
  {
    return fromViewType switch
    {
      1 => Resource.Layout.TaskHeader,
      2 => Resource.Layout.PersonItem,
      _ => Resource.Layout.PersonItem,
    };
  }
  
  
  public int GetItemViewType(object forItemObject)
  {
    if (forItemObject is TaskHeader<DateTime>)
    {
      return 1;
    }
    else if (forItemObject is Person)
    {
      return 2;
    }
    else
    {
      return -1;
    }
  }
}
```

5. Finally, adding `MvxExpandableRecyclerView` to one of your `View.xml` is as easy as:

```xml
<MvvmCross.ExpandableRecyclerView.DroidX.MvxExpandableRecyclerView
  android:id="@+id/people_list_recyclerview"
  android:layout_width="match_parent"
  android:layout_height="match_parent"
  local:MvxTemplateSelector="Namespace.BirthdayTemplateSelector, Assembly.Name"
  local:MvxBind="ItemsSource People;
                 ItemClick ItemClickCommand; 
                 ItemLongClick ItemLongClickCommand
                 ItemSwipeStart SwipeStartCommand;
                 ItemSwipeEnd SwipeEndCommand;"/>
```

  __Important:__ MvxExpandableRecyclerView will require you to bind an `ObservableCollection<ITaskItem>` to `ItemsSource` and will need to have your custom `MvxTemplateSelector` for it to display your headers and items correctly.
  For more information, MvvmCross provides documentation for [MvxTemplateSelector](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview#using-an-item-template-selector). If you want to display complex objects for both/either headers and/or items, it is **strongly recommended to use `MvxTemplateSelector`** to show different types of views.

## Customising Headers

MvxExpandableRecylcerView (along with custom views for items) also allows for custom views for headers. This can be done by implementing a new class that inherits `TaskHeader<T>` and passing the type of object to group by. A new adapter will also be needed that inherits `MvxExpandableRecyclerAdapter<T>` with the same type of object and overriding the `GenerateHeader` method.

In our example, we want to display a special header for people whose birthday is today. 

1. In our `.Droid`/`.DroidX` project, we will create a new class that inherits `TaskHeader<DateTime>` where `DateTime` is the type we are grouping our items by.

```csharp
public class TodayHeader : TaskHeader<DateTime>
{
  public TodayHeader(string name, DateTime model) 
    : base(name, model)
  {
  }

  public string Message => "Today's a special day!";
}
```

2. We then create a view to display our new header `TodayHeader.xml`, which will show 2 stars instead of one.

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:orientation="horizontal"
  android:layout_width="match_parent"
  android:layout_height="wrap_content">
  <ImageView
    android:layout_width="wrap_content"
    android:layout_height="match_parent"
    android:src="@android:drawable/star_on"/>
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    app:MvxBind="Text Name"/>
  <ImageView
    android:layout_width="wrap_content"
    android:layout_height="match_parent"
    android:src="@android:drawable/star_on"/>
</LinearLayout>
```

3. In our custom template selector class, we add a new condition to show `TodayHeader.xml` when item is of type `TodayHeader`.

```csharp
public class BirthdayTemplateSelector : IMvxTemplateSelector 
{
  public int ItemTemplateId { get; set; }
  
  public int GetItemLayoutId(int fromViewType) 
  {
    return fromViewType switch
    {
      // We check for TodayHeader first in the below method.
      1 => Resource.Layout.TodayHeader,
      2 => Resource.Layout.TaskHeader,
      3 => Resource.Layout.PersonItem,
      _ => Resource.Layout.PersonItem,
    };
  }
  
  
  public int GetItemViewType(object forItemObject)
  {
    // We need check for TodayHeader first because we need to return the TodayHeader view and not the TaskHeader<DateTime> view.
    if (forItemObject is TodayHeader)
    {
      return 1;
    }
    else if (forItemObject is TaskHeader<DateTime>)
    {
      return 2;
    }
    else if (forItemObject is Person)
    {
      return 3;
    }
    else
    {
      return -1;
    }
  }
}
```

4. We then create a new class that inherits `MvxExpandableRecyclerAdapter<DateTime>`.

```chsarp
public class BirthdayExpandableRecyclerAdapter : MvxExpandableRecyclerAdapter<DateTime>
{
  // Code...
}
```

5. In the new adapter, we override the `GenerateHeader` to this:

```csharp
public class BirthdayExpandableRecyclerAdapter : MvxExpandableRecyclerAdapter<DateTime>
{
  // Code...
  
  public override ITaskHeader GenerateHeader(DateTime model)
  {
    if (model.Date == DateTime.Today)
    {
      // Return our new `TodayHeader` class.
      return new TodayHeader(model.Date.ToShortDateString(), model);
    }
    // Return the default `TaskHeader` class.
    return new TaskHeader<DateTime>(model.Date.ToShortDateString(), model);
  }
}
```

6. In our `View.cs` we attach our new adapter to `MvxExpandableRecyclerView`. You will need a `BindingContext` which can be retrieved from views that use [`MvxActivity`](https://github.com/MvvmCross/MvvmCross/blob/develop/MvvmCross/Platforms/Android/Views/MvxActivity.cs).

```chsharp
MvxExpandableRecyclerView expandableRecyclerView = _view.FindViewById<MvxExpandableRecyclerView>(Resource.Id.people_list_recyclerview);
expandableRecyclerView.Adapter = new BirthdayExpandableRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
```
## Dragging and Swiping Items

To enable dragging and swiping features, we need to use `ItemTouchHelperCallback` and attach it to our `MvxExpandableRecyclerView`.

```csharp
MvxExpandableRecyclerView expandableRecyclerView = _view.FindViewById<MvxExpandableRecyclerView>(Resource.Id.people_list_recyclerview);
ItemTouchHelperCallback itemMoveCallback = new ItemTouchHelperCallback(expandableRecyclerView.Adapter);
ItemTouchHelper itemTouchHelper = new ItemTouchHelper(itemMoveCallback);
itemTouchHelper.AttachToRecyclerView(expandableRecyclerView);
```

Swipe actions are bindable and can have 2 different actions depending on the direction of the swipe. `ItemSwipeStart` and `ItemSwipeEnd` are bindable and are done in the same way as `MvxRecyclerView`'s [`ItemClickCommand` and `ItemLongClickCommand`](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview#itemclick-and-itemlongclick-commands).

[^1]: If you don’t provide an item template selector `MvxExpandableRecyclerView` will fall back to using a `SimpleListItem1`, which is a built in Android Resource. It will also just call `ToString()` on your item that you are supplying. A custom view should be used for headers, if items aren't grouped using a `string`.
