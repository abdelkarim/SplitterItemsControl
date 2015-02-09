# Overview
The **SplitterItemsControl** is a control that allows you to host a set of items with a separator between them. The separator is used to adjust the space reserved for each Item.

The **SplitterItemsControl** uses the same mechanism as the **Grid** control to divide the available space among the items, except for the `Auto` mode.

You can add items with fixed length, eg: `<lib:SplitterItem Length="120" />`, or you can use the star `*` to give a weighted proportion of the available space, eg: `<lib:SplitterItem Length="0.5*" />`.

# Installation
You can get the control by adding the following Nuget Package to your project.

# Features
The control has many features that makes it ideal for your splitting needs:
* Orientation
* Dragging Mode
* Fixed Size
* Grid like star size.

### Orientation
By using the `Orientation` property, items can be arranged either vertically or horizontally.

![image](https://cloud.githubusercontent.com/assets/1153480/6107989/b8658bf0-b06f-11e4-9ab2-b9d06e06ecf6.png)

### Splitter Dragging Mode
In the **SplitterItemsControl** you can choose between a **Continuous** or **Deferred** dragging modes. In the Continuous mode, the items are re-sized as soon as the separator is moved.

The Deferred mode is ideal in case the Continuous mode incur a performance penalty. In this mode a preview popup will be displayed to indicate the final location of the separator.

### Fixed items
The control allows to include items whose size is **Fixed**, thus the separators adjascent to it on both sides can't be dragged.

The following example will fix the first item to `120` pixel.
``` xml
<lib:SplitterItemsControl>
    <lib:SplitterItem Length="120">
        <Border>
            <TextBlock Text="1" VerticalAlignment="Center" HorizontalAlignment="Center" FontSize="30" />
        </Border>
    </lib:SplitterItem>

    <lib:SplitterItem>
        <Border>
            <TextBlock Text="2" VerticalAlignment="Center" HorizontalAlignment="Center" FontSize="30" />
        </Border>
    </lib:SplitterItem>
</lib:SplitterItemsControl>
```

The result will be the following preview:

![image](https://cloud.githubusercontent.com/assets/1153480/6107777/fe26ac52-b06d-11e4-8e3b-212d755d5dc5.png)

# Usage
