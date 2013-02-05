SpencerHakim.Windows.Forms
==============
A small collection of custom .NET Windows Forms components written in C#. Components are separated into their own projects and merged using ILRepack into SpencerHakim.Windows.Forms.dll. Requires Nuget.

---

##ImageMapButton
Functions similarly to the HTML `<map>` tag, but using an image color map instead of explicitly defined `<area>` shapes. Demo app included.
####Features include...
- hot, pressed, and disabled states (enable/disable areas via `ImageMapButtonArea.Enabled` property)
- multiple, accelerating, repeated click events from holding down a button (via `ImageMapButton.MultiClick` property)
- toggle-able buttons (via `ImageMapButtonArea.ToggleMode` property)
- configurable scaling algorithm (via `ImageMapButton.InterpolationMode` and `.PixelOffsetMode` properties)
- optional tooltip text for areas (via `ImageMapButtonArea.Text` property)

####Usage and Tips
- All valid map area clicks trigger the `ImageMapButton.ButtonClick` event, you must filter events from there.
- If the base image (`ImageMapButton.Image`) contains static graphics that will not be part of an `ImageMapButtonArea`, those parts of the image should be completely transparent in the images provided to `ImageMapButton.MouseOverImage`, `.MouseDownImage`, and `.DisabledImage`.
- When choosing a size/location(/destination) for your area, your rectangle should completely fit the arbitrarily shaped area inside of it. (For the demo app, the images are 128x128 and the rectangles for each area are 64x64 despite the shape only taking up three-fourths of that area.)
- Overlapping destination rectangles are OK, just make sure to design your images so that source rectangles do not overlap.
- You own your images, so dispose of them when you're done; memory leaks are bad, mmkay?
- For ease of use, `ImageMapButton` implements `IEnumerable<ImageMapButtonArea>` and has an `ImageMapButtonArea this[Color]` indexer. This probably isn't "best practice." I do not care.

---

##DWMThumbnail
Uses the Desktop Window Manager API to display a thumbnail of the specified window. Requires Windows Vista or newer.