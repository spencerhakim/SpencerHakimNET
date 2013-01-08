ImageMapButton
==============
A .NET Windows Form component written in C# that functions similarly to the HTML <map> tag,
but using an image color map instead of explicitly defined <area> shapes. Demo app included.

###Features include...
- hot, pressed, and disabled states (enable/disable areas via `ImageMapButtonArea.Enabled` property)
- multiple, accelerating, repeated clicks from holding the mouse button down on an area (via `ImageMapButton.MultiClick` property)
- toggle-able areas (via `ImageMapButtonArea.ToggleMode` property)
- configurable scaling algorithm (via `ImageMapButton.InterpolationMode` and `.PixelOffsetMode` properties)
- optional tooltip text for areas (via `ImageMapButtonArea.Text` property)

###Usage and Tips
- If the base image (`ImageMapButton.Image`) contains static imagery that will not be part of an `ImageMapButtonArea`, those parts of the image should be completely transparent in the images provided to `ImageMapButton.MouseOverImage` and `.MouseDownImage`. These areas should be opaque in the image provided to `.DisabledImage`, however. See the demo app's images for a sample.
- When choosing a size/location(/destination) for your area, your rectangle should completely fit the arbitrarily shaped area inside of it. For the demo app, the images are 128x128 and the rectangles for each area are 64x64 despite the shape only taking up three-fourths of that area.
- Overlapping destination rectangles are OK, just make sure to design your images so that source rectangles do not overlap.
- You own your images, so dispose of them when you're done; memory leaks are bad, mmkay?
- For ease of use, `ImageMapButton` implements `IEnumerable<ImageMapButtonArea>` and has a `ImageMapButtonArea this[Color]` indexer. This probably isn't "best practice." I do not care.