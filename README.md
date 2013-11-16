# SpencerHakimNET

A small collection of C# 4.0 utility code and custom Windows Forms (and, eventually, WPF) components I've slowly written and/or rewritten over the years. Components are separated into their own projects. Requires [NuGet](http://www.nuget.org/).

I've not tested the code with Mono, but it should work for a lot of what's here. Anything that uses System.Management, which Mono explicitly does not support, will obviously not work, nor would anything using P/Invoke.

Licensed under the GNU LGPL 2.1

---

## SpencerHakimNET and SpencerHakimNET.Native
Random utility and helper code collected from various projects. The .Native project contains C++/CLR code, usually for wrapping advanced Win32 and COM APIs.

## SpencerHakim.Windows.Forms
An empty project which references every WinForms component in this repository. Post-build, it merges the components and all their dependencies into  a single assembly, `SpencerHakim.Windows.AIO.dll`, using [ILRepack](https://github.com/gluck/il-repack).

## ImageMapButton
Functions similarly to the HTML `<map>` tag, but using an image color map instead of explicitly defined `<area>` shapes. Demo app included.

A bit dependency heavy in terms of sheer number of files, as it relies on [Accord .NET's](http://accord-framework.net/) [k-d tree implementation](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_Structures_KDTree_1.htm) (the bulk of the dependencies), as well as the [ColorMine](https://github.com/THEjoezack/ColorMine) color-space library.

#### Features include...
- hot, pressed, and disabled states (enable/disable areas via `ImageMapButtonArea.Enabled` property)
- multiple, accelerating, repeated click events from holding down a button (via `ImageMapButton.MultiClick` property)
- toggle-able buttons (via `ImageMapButtonArea.ToggleMode` property)
- configurable scaling algorithm (via `ImageMapButton.InterpolationMode` and `.PixelOffsetMode` properties)
- optional tooltip text for areas (via `ImageMapButtonArea.Text` property)
- variable fuzzy matching of colors to handle gradients, banding, and compression artifacts in the chroma key (via the `.ChromaKeyFuzziness` property)

#### Usage and Tips
- All valid map area clicks trigger the `ImageMapButton.ButtonClick` event, you must filter events from there.
- If the base image (`ImageMapButton.Image`) contains static graphics that will not be part of an `ImageMapButtonArea`, those parts of the image should be completely transparent in the images provided to `ImageMapButton.MouseOverImage`, `.MouseDownImage`, and `.DisabledImage`.
- When choosing a size/location(/destination) for your area, your rectangle should completely fit the arbitrarily shaped area inside of it. (For the demo app, the images are 128x128 and the rectangles for each area are 64x64 despite the shape only taking up three-fourths of that area.)
- Overlapping destination rectangles are OK, just make sure to design your images so that source rectangles do not overlap.
- You own your images, so dispose of them when you're done; memory leaks are bad, mmkay?
- For ease of use, `ImageMapButton` implements `IEnumerable<ImageMapButtonArea>` and has an `ImageMapButtonArea this[Color]` indexer. This probably isn't "best practice." I do not care.
- Recommend using PNGs for the `ImageMapButton.ChromaKeyImage` property to avoid any compression artifacting which may create color distortion around the edges of colored areas, which could prevent matching.

## DWMThumbnail
Uses the Desktop Window Manager API to display a thumbnail of the specified window. Requires Windows Vista or newer. Demo app included.

#### Features include...
- dynamic thumbnail of a whole or partial window using Windows' own Desktop Window Manager, the desktop compositing engine introduced in Vista, for perfect performance and compatibility
- ease of use: just set `DWMThumbnail.SourceWindow` and you're good to go
- properties for configuring upscaling (`DWMThumbnail.ScaleAboveNativeSize`), opacity (`.Opacity`), whether to include the native window borders (`.SourceClientAreaOnly`), and the specific area to display a thumbnail for (`.SourceArea`)

#### Usage and Tips
- See [DWM_THUMBNAIL_PROPERTIES on MSDN](http://msdn.microsoft.com/en-us/library/windows/desktop/aa969502%28v=vs.85%29.aspx) for more info on the configuration properties.
- `DWMThumbnail.SourceArea` defaults to `Rectangle.Empty` (value of `0,0,0,0`), which is interpreted as setting the `SourceArea` to a rectangle the size returned by [DwmQueryThumbnailSourceSize](http://msdn.microsoft.com/en-us/library/windows/desktop/aa969520%28v=vs.85%29.aspx) (more or less the full size of the window, depending on `SourceClientAreaOnly`)

## UnitTests
As its name would imply, unit tests for my code. Nothing here yet, I'll get around to it (I know, I'm awful. Mostly I just need to figure out the best way to test the majority of this stuff.)