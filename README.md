ImageMapButton
==============
A .NET Windows Form component written in C# that functions similarly to the HTML <map> tag,
but using an image color map instead of explicitly defined <area> shapes. Demo app included.

###Features
- hot, pressed, and disabled states (enable/disable areas via `ImageMapButtonArea.Enabled` property)
- multiple, accelerating, repeated clicks from holding the mouse button down on an area (via `ImageMapButton.MultiClick` property)
- toggle-able areas (via `ImageMapButtonArea.ToggleMode` property)
- configurable scaling algorithm (via `ImageMapButton.InterpolationMode` and `.PixelOffsetMode` properties)
- optional tooltip text for areas (via `ImageMapButtonArea.Text` property)