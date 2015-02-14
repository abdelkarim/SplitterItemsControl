# Overview

![splitteritemscontrol_demo](https://cloud.githubusercontent.com/assets/1153480/6123989/6e38b314-b109-11e4-8f0a-080eb2206872.gif)

The **SplitterItemsControl** allows you to host a set of items with a splitter between them. The splitter is used to adjust the space reserved for each pair of items.

The provided features are similar to what the [GridSplitter](https://msdn.microsoft.com/en-us/library/system.windows.controls.gridsplitter(v=vs.110).aspx) does in a Grid panel, except that the functionality is packaged in an **ItemsControl**.

# Features
The following features are exposed in the **SplitterItemsControl**:
 * Adjust the orientation of the control, either vertically or horizontally.
 * Adjust the behavior of the splitters, by using the **DraggingMode** property.
     - Continous: in this mode the item sizes are updated in real time, as soon as the user drags the splitter.
     - Deferred: in this mode a preview is displayed to indicate the change in sizes. The change is applied once when the user realeases the splitter.
 * MVVM friendly, since we inherit directly from the `ItemsControl`, the control plays nicely with the MVVM pattern. You can either directly instantiate items in XAML, or use the `ItemsSource` property.
 * The control uses the same mechanism of the Grid columns and rows to divide the space, you can create:
     - items with the minimum allowable size, eg: `<lib:SplitterItem MinLength="120" />`
     - items with fixed size, eg: `<lib:SplitterItem Length="250" />`
     - items with size that is a weighted proportion of the availbe size, eg: `<lib:SplitterItem Length="0.5*" />`

# Installation
You can get the control by adding the following Nuget Package to your project:
[https://www.nuget.org/packages/SplitterItemsControl](https://www.nuget.org/packages/SplitterItemsControl)

# Usage
Below is the XAML snippet used to have the gif animation used above:

``` xml
<lib:SplitterItemsControl>
    <lib:SplitterItem Length="0.5*">
        <Border>
            <TextBlock Text="1" VerticalAlignment="Center" HorizontalAlignment="Center" FontSize="30" />
        </Border>
    </lib:SplitterItem>

    <lib:SplitterItemsControl Orientation="Horizontal" DraggingMode="Continuous">
        <lib:SplitterItem>
            <Border>
                <TextBlock Text="2" VerticalAlignment="Center" HorizontalAlignment="Center" FontSize="30" />
            </Border>
        </lib:SplitterItem>
        <lib:SplitterItem>
            <Border>
                <TextBlock Text="3" VerticalAlignment="Center" HorizontalAlignment="Center" FontSize="30" />
            </Border>
        </lib:SplitterItem>
    </lib:SplitterItemsControl>
</lib:SplitterItemsControl>
```
