# Android MvxExpandableRecyclerView

This is an unofficial package that contains an expandable AndroidX RecyclerView supported for MvvmCross. This view allows us to bind a collection of items (objects, ViewModels, etc) to the `ItemsSource` property. It works similarly to a RecyclerView. However, this comes with out-of-the-box functionality such as grouping items with collapsible/expandable headers. Additional functionality can be implemented such as dragging items up and down and swiping them by binding a `boolean` property to `EnableDrag` and `EnableSwipe` respectively.

All original functionality of `MvxRecyclerView` is also available and it is highly encouraged that you read the [documentation](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview) before proceeding.

## Getting Started

You need to ensure that you have the [MvxExpandableRecyclerView.Core](https://www.nuget.org/packages/MvxExpandableRecyclerView.Core/) and [MvxExpandableRecyclerView.DroidX](https://www.nuget.org/packages/MvxExpandableRecyclerView.DroidX/) NuGet packages installed in your `.Core` and `.Droid` projects respectively.

The general steps to implement this control:

1. We want to create an app where people are grouped by their appointment dates. Firstly, in our `.Core` project, we create an entity class to hold our data: `Person.cs`.

```csharp
public class Person
{
  public Person(string firstName, string lastName, DateTime? appointment)
  {
    FirstName = firstName;
    LastName = lastName;
    Appointment = appointment;
  }

  public string FirstName { get; set; }

  public string LastName { get; set; }

  public DateTime? Appointment { get; set; }
}
```

2. We then create a class that inherits `TaskItem<Person, DateTime?>` named `PersonItem.cs`. This will allow the ExpandableRecyclerView to know how to group our `Person.cs` objects.

```csharp
public class PersonItem : TaskItem<Person, DateTime?>
{
  public PersonItem(Person model) 
    : base(model)
  { }

  public override DateTime? Header { get => Model.Appointment; set => Model.Appointment = value; }
}
```

3. In our ViewModel (where we will add the ExpandableRecyclerView to the corresponding view), we will initialise a list that will hold `PersonItem`s for binding.

```csharp
public MvxObservableCollection<ITaskItem> People { get; private set; }
```

4. For the rest of the steps, everything will be done in our `.Droid` project. We create a view to display our `PersonItem.cs` objects inside the ExpandableRecyclerView. We'll name the view `PersonItem.xml`.

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:layout_width="match_parent"
  android:layout_height="wrap_content"
  android:orientation="horizontal"
  android:gravity="center">
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    app:MvxBind="Text Format('{0} {1} - {2:d}', Model.FirstName, Model.LastName, Model.Appointment);"/>
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:layout_marginHorizontal="4sp"
    android:textColor="?android:attr/colorAccent"
    app:MvxBind="Text Sequence;"/>
</LinearLayout>
```

Notice that "Model" is prepended to our binded properties. This allows us to access properties in the underlying entity class. In this example, "Model" refers to the `Person.cs` entity class and we are binding `Person.FirstName`, `Person.LastName` and `Person.Appointment` to the `TextView` We also have another `TextView` that binds to `TaskItem<TModel, THeader>.Sequence`, if you want to save the ordering of each item.

5. We then create another view for our headers, if we want to display something other than a `SimpleListItem1`[^1]. In this example: `TaskHeader.xml` displays an `ImageView` showing an arrow up or down depending on the `TaskHeader.IsCollapsed` property and uses a `TextView` to bind to `TaskHeader.Name`. There's also another `TextView` that display the number of items under the said header.

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:layout_width="match_parent"
  android:layout_height="wrap_content"
  android:orientation="horizontal"
  android:padding="8sp"
  android:gravity="center">
  <FrameLayout
    android:layout_width="wrap_content"
    android:layout_height="wrap_content">
    <ImageView
      android:layout_width="wrap_content"
      android:layout_height="wrap_content"
      android:src="@android:drawable/arrow_down_float"
      app:MvxBind="Visible IsCollapsed;"/>
    <ImageView
      android:layout_width="wrap_content"
      android:layout_height="wrap_content"
      android:src="@android:drawable/arrow_up_float"
      app:MvxBind="Visible !IsCollapsed;"/>
  </FrameLayout>
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    style="@style/Base.TextAppearance.AppCompat.Headline"
    app:MvxBind="Text Name;"/>
  <TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:layout_marginHorizontal="4sp"
    android:textColor="@android:color/holo_red_dark"
    app:MvxBind="Text Count;"/>
</LinearLayout>
```

6. We then create a custom [Item Template Selector](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview#using-an-item-template-selector) to handle displaying view for the corresponding item(s). In this example, if an item doesn't have a corresponding view, it will default to `PersonItem.xml`.

```csharp
public class AppointmentTemplateSelector : MvxTemplateSelector<ITaskItem>
{
  public override int GetItemLayoutId(int fromViewType)
  {
    return fromViewType switch
    {
      1 => Resource.Layout.TaskHeader,
      _ => Resource.Layout.PersonItem,
    };
  }

  protected override int SelectItemViewType(ITaskItem forItemObject)
  {
    if (forItemObject is ITaskHeader)
      return 1;
    else
      return -1;
  }
}
```

7. Finally, adding `MvxExpandableRecyclerView` to one of your `View.xml` is very simple. In this example we have `AppointmentView.xml` and add:

```xml
<MvvmCross.ExpandableRecyclerView.DroidX.MvxExpandableRecyclerView
	android:id="@+id/appointment_recyclerview"
	android:layout_width="match_parent"
	android:layout_height="match_parent"
	local:MvxTemplateSelector="AppointmentPlanner.Droid.Components.AppointmentTemplateSelector, AppointmentPlanner.Droid"
	local:MvxBind="ItemsSource People;
			ItemSwipeRight UnplanPersonCommand;
			ItemSwipeLeft RemovePersonCommand;"/>
```

__Important:__ MvxExpandableRecyclerView will require you to bind an `MvxObservableCollection<ITaskItem>` to `ItemsSource` and will need to have your custom `MvxTemplateSelector` for it to display your headers and items correctly.

For more information, MvvmCross provides documentation for [MvxTemplateSelector](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview#using-an-item-template-selector). If you want to display complex objects for both/either headers and/or items, it is **strongly recommended to use `MvxTemplateSelector`** to show different types of views.

## Dragging and Swiping Items

To enable dragging and swiping features, we need to modify our `xml` and bind `EnableDrag` and `EnableSwipe` to `true`.

```xml
<MvvmCross.ExpandableRecyclerView.DroidX.MvxExpandableRecyclerView
	android:id="@+id/appointment_recyclerview"
	android:layout_width="match_parent"
	android:layout_height="match_parent"
	local:MvxTemplateSelector="AppointmentPlanner.Droid.Components.AppointmentTemplateSelector, AppointmentPlanner.Droid"
	local:MvxBind="ItemsSource People;
			ItemSwipeRight UnplanPersonCommand;
			ItemSwipeLeft RemovePersonCommand;
			EnableDrag true;
			EnableSwipe true;"/>
```

Swipe actions are bindable and can have 2 different actions depending on the direction of the swipe. `ItemSwipeLeft` and `ItemSwipeRight` are bindable and are done in the same way as `MvxRecyclerView`'s [`ItemClickCommand` and `ItemLongClickCommand`](https://www.mvvmcross.com/documentation/platform/android/android-recyclerview#itemclick-and-itemlongclick-commands).

## Hide Sticky Header

A sticky header is always shown by default, however we can hide the sticky header by modifying our `xml` and bind `ShowStickyHeader` to `false`.

```xml
<MvvmCross.ExpandableRecyclerView.DroidX.MvxExpandableRecyclerView
	android:id="@+id/appointment_recyclerview"
	android:layout_width="match_parent"
	android:layout_height="match_parent"
	local:MvxTemplateSelector="AppointmentPlanner.Droid.Components.AppointmentTemplateSelector, AppointmentPlanner.Droid"
	local:MvxBind="ItemsSource People;
			ItemSwipeRight UnplanPersonCommand;
			ItemSwipeLeft RemovePersonCommand;
			EnableDrag true;
			EnableSwipe true;
			ShowStickyHeader false;"/>
```

[^1]: If you don’t provide an item template selector `MvxExpandableRecyclerView` will fall back to using a `SimpleListItem1`, which is a built in Android Resource. It will also just call `ToString()` on your item that you are supplying. A custom view should be used for headers, if items aren't grouped using a `string`.
