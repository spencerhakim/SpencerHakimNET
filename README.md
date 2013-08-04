SpencerHakimNET
===============
A small collection of C# utility code and custom Windows Forms and WPF components I've slowly written and/or rewritten over the years. Components are separated into their own projects and merged using ILRepack into SpencerHakim.Windows.AIO.dll. Requires NuGet.

Licensed under the GNU LGPL 2.1

---

## SpencerHakimNET and SpencerHakimNET.Native
Random utility and helper code collected from various projects. The .Native project contains C++/CLR code, usually for wrapping advanced Win32 and COM APIs.

## ImageMapButton
Functions similarly to the HTML `<map>` tag, but using an image color map instead of explicitly defined `<area>` shapes. Demo app included.

#### Features include...
- hot, pressed, and disabled states (enable/disable areas via `ImageMapButtonArea.Enabled` property)
- multiple, accelerating, repeated click events from holding down a button (via `ImageMapButton.MultiClick` property)
- toggle-able buttons (via `ImageMapButtonArea.ToggleMode` property)
- configurable scaling algorithm (via `ImageMapButton.InterpolationMode` and `.PixelOffsetMode` properties)
- optional tooltip text for areas (via `ImageMapButtonArea.Text` property)

#### Usage and Tips
- All valid map area clicks trigger the `ImageMapButton.ButtonClick` event, you must filter events from there.
- If the base image (`ImageMapButton.Image`) contains static graphics that will not be part of an `ImageMapButtonArea`, those parts of the image should be completely transparent in the images provided to `ImageMapButton.MouseOverImage`, `.MouseDownImage`, and `.DisabledImage`.
- When choosing a size/location(/destination) for your area, your rectangle should completely fit the arbitrarily shaped area inside of it. (For the demo app, the images are 128x128 and the rectangles for each area are 64x64 despite the shape only taking up three-fourths of that area.)
- Overlapping destination rectangles are OK, just make sure to design your images so that source rectangles do not overlap.
- You own your images, so dispose of them when you're done; memory leaks are bad, mmkay?
- For ease of use, `ImageMapButton` implements `IEnumerable<ImageMapButtonArea>` and has an `ImageMapButtonArea this[Color]` indexer. This probably isn't "best practice." I do not care.

## DWMThumbnail
Uses the Desktop Window Manager API to display a thumbnail of the specified window. Requires Windows Vista or newer. Demo app included.

#### Features include...
- dynamic thumbnail of a whole or partial window using Windows' own Desktop Window Manager, the desktop compositing engine introduced in Vista, for perfect performance and compatibility
- ease of use: just set `DWMThumbnail.SourceWindow` and you're good to go
- properties for configuring upscaling (`DWMThumbnail.ScaleAboveNativeSize`), opacity (`.Opacity`), whether to include the native window borders (`.SourceClientAreaOnly`), and the specific area to display a thumbnail for (`.SourceArea`)

#### Usage and Tips
- See [DWM_THUMBNAIL_PROPERTIES on MSDN](http://msdn.microsoft.com/en-us/library/windows/desktop/aa969502%28v=vs.85%29.aspx) for more info on the configuration properties.
- `DWMThumbnail.SourceArea` defaults to `Rectangle.Empty` (value of `0,0,0,0`), which is interpreted as setting the `SourceArea` to a rectangle the size returned by [DwmQueryThumbnailSourceSize](http://msdn.microsoft.com/en-us/library/windows/desktop/aa969520%28v=vs.85%29.aspx) (more or less the full size of the window, depending on `SourceClientAreaOnly`)