<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🎨 Hydrogen.Drawing

**Unified 2D graphics abstraction layer** providing drawing utilities, color manipulation, image processing, and shape rendering for Windows desktop and cross-platform applications.

Hydrogen.Drawing enables **platform-agnostic graphics operations** through a high-level abstraction over GDI+, with utilities for color gradients, image manipulation, geometric transformations, and visual effects.

## ⚡ 10-Second Example

```csharp
using Hydrogen;
using Tools;
using System.Drawing;

// Create color gradient
Color[] gradient = Drawing.CalculateColorGradient(
    Color.Red, 
    Color.Blue, 
    10);  // 10 colors from red to blue

foreach (var color in gradient) {
    Console.WriteLine($"RGB({color.R}, {color.G}, {color.B})");
}

// Convert string to color
Color myColor = Drawing.ConvertStringToColor("255:0:128");

// Manipulate images
Bitmap image = new Bitmap("photo.jpg");
Bitmap rounded = Drawing.CreateRoundedImage(image, 20);  // 20px radius
rounded.Save("rounded.jpg");
```

## 🏗️ Core Concepts

**Color Gradients**: Smooth color transitions between start and end colors with interpolation.

**Color Conversions**: Parse and convert between string, ARGB, and Color representations.

**Image Processing**: Bitmap manipulation including resizing, rounding corners, and visual effects.

**Geometric Utilities**: Point, size, and rectangle extensions for calculations.

**Rounded Corners**: Create images with smooth rounded corners and borders.

## 🔧 Core Examples

### Color Gradient Generation

```csharp
using Tools;
using System.Drawing;

// Generate gradient with interpolation between two colors
Color startColor = Color.FromArgb(255, 0, 0);      // Red
Color endColor = Color.FromArgb(0, 0, 255);        // Blue
int gradientSteps = 20;

Color[] gradient = Drawing.CalculateColorGradient(
    startColor, 
    endColor, 
    gradientSteps);

Console.WriteLine($"Generated {gradient.Length} colors:");
for (int i = 0; i < gradient.Length; i++) {
    Console.WriteLine($"  [{i}] ARGB({gradient[i].A}, {gradient[i].R}, {gradient[i].G}, {gradient[i].B})");
}

// Use gradient for visual effects
// For example, painting a gradient fill:
using (var brush = new LinearGradientBrush(
    new Point(0, 0), 
    new Point(256, 0), 
    startColor, 
    endColor)) {
    
    graphics.FillRectangle(brush, new Rectangle(0, 0, 256, 50));
}
```

### String to Color Conversion

```csharp
using Tools;
using System.Drawing;

// Convert string formats to Color
string[] colorStrings = {
    "255:0:0",              // Red (RGB)
    "0:255:0",              // Green (RGB)
    "0:0:255",              // Blue (RGB)
    "255:128:64:200"        // With Alpha (ARGB)
};

foreach (var colorStr in colorStrings) {
    Color color = Drawing.ConvertStringToColor(colorStr);
    Console.WriteLine($"{colorStr} -> ARGB({color.A}, {color.R}, {color.G}, {color.B})");
}

// Round-trip conversion
Color original = Color.FromArgb(200, 100, 150, 50);
string colorString = $"{original.A}:{original.R}:{original.G}:{original.B}";
Color converted = Drawing.ConvertStringToColor(colorString);
Console.WriteLine($"Match: {original == converted}");  // true
```

### Image Rounding & Corner Effects

```csharp
using Tools;
using System.Drawing;

// Load and process image
Bitmap originalImage = new Bitmap("photo.jpg");

// Create rounded corners
int cornerRadius = 30;
Bitmap roundedImage = Drawing.CreateRoundedImage(originalImage, cornerRadius);
roundedImage.Save("rounded_photo.jpg");

// Create rounded image with border
Bitmap borderedImage = Drawing.CreateRoundedImage(
    originalImage, 
    cornerRadius, 
    5,              // Border width
    Color.DarkGray);// Border color

borderedImage.Save("rounded_with_border.jpg");

// Create circular image (max corner radius)
int maxRadius = Math.Min(originalImage.Width, originalImage.Height) / 2;
Bitmap circularImage = Drawing.CreateRoundedImage(originalImage, maxRadius);
circularImage.Save("circular_photo.jpg");
```

### Image Resizing & Scaling

```csharp
using Tools;
using System.Drawing;

Bitmap original = new Bitmap("large_image.jpg");

// Resize to specific dimensions
Size newSize = new Size(400, 300);
Bitmap resized = Drawing.ResizeImage(original, newSize, ResizeMethod.HighQuality);
resized.Save("resized.jpg");

// Scale by percentage
Bitmap scaled = Drawing.ScaleImage(original, 0.5);  // 50% of original
scaled.Save("scaled_50percent.jpg");

// Maintain aspect ratio while fitting in bounds
Size maxBounds = new Size(800, 600);
Bitmap fitted = Drawing.FitImageInBounds(original, maxBounds);
fitted.Save("fitted.jpg");
```

### Point & Rectangle Extensions

```csharp
using Tools;
using System.Drawing;

// Point calculations
Point p1 = new Point(10, 20);
Point p2 = new Point(30, 40);

// Distance between points
double distance = p1.DistanceTo(p2);
Console.WriteLine($"Distance: {distance}");

// Offset point
Point offset = p1.Offset(5, 10);
Console.WriteLine($"Offset: {offset}");  // (15, 30)

// Rectangle operations
Rectangle rect = new Rectangle(10, 10, 100, 100);
Point center = rect.GetCenter();
Console.WriteLine($"Center: {center}");  // (60, 60)

// Check containment with margin
bool contains = rect.ContainsWithMargin(new Point(50, 50), 5);

// Get area
int area = rect.GetArea();
Console.WriteLine($"Area: {area}");  // 10000

// Expand rectangle
Rectangle expanded = rect.Expand(20);  // Expand by 20px on all sides
Console.WriteLine($"Expanded: {expanded}");
```

### Disabled/Grayed-Out Effects

```csharp
using Tools;
using System.Drawing;

Bitmap originalImage = new Bitmap("icon.png");

// Create disabled (grayed-out) version
Bitmap disabledImage = Drawing.CreateDisabledImage(originalImage);
disabledImage.Save("icon_disabled.png");

// This is useful for UI states like:
// - Inactive buttons
// - Disabled menu items
// - Inactive toolbar icons
```

### Size & Scaling Extensions

```csharp
using Tools;
using System.Drawing;

Size original = new Size(800, 600);

// Scale size
Size scaled = original.Scale(1.5);  // 1200 x 900
Console.WriteLine($"Scaled: {scaled}");

// Get aspect ratio
float ratio = original.GetAspectRatio();  // 1.333...
Console.WriteLine($"Aspect Ratio: {ratio}");

// Fit within bounds maintaining aspect
Size maxBounds = new Size(400, 400);
Size fitted = original.FitWithinBounds(maxBounds);
Console.WriteLine($"Fitted: {fitted}");

// Create thumbnail
Size thumb = original.CreateThumbnailSize(150);
Console.WriteLine($"Thumbnail: {thumb}");
```

## 🏗️ Architecture & Modules

**Color Module**: Color conversion, gradients, and color space operations
- String parsing (ARGB format)
- RGB ↔ HSL conversions
- Color space transformations
- Gradient interpolation

**Image Processing Module**: Bitmap operations and effects
- Resizing with quality control
- Rounded corners and borders
- Disabled/grayed-out effects
- Aspect ratio preservation

**Geometric Utilities**: Point, Size, Rectangle extensions
- Distance calculations
- Containment testing
- Area and perimeter operations
- Center point calculation
- Alignment and positioning

**Drawing Extensions**: System.Drawing extensions
- Graphics primitives
- Brush and pen utilities
- Text rendering helpers
- Bitmap manipulation

## 📦 Dependencies

- **Hydrogen**: Core framework
- **System.Drawing.Common**: .NET graphics abstraction (.NET built-in)
- **System.Drawing.Primitives**: Primitive types (Point, Size, Rectangle)

## ⚠️ Best Practices

- **Dispose resources**: Always dispose Bitmap, Graphics, and Brush objects
- **Quality vs performance**: Use HighQuality resize for final output; LowQuality for previews
- **Color validation**: Validate color strings before conversion
- **Image size limits**: Check image dimensions before processing to avoid memory issues
- **Use using statements**: Wrap Graphics operations in using blocks
- **Cache gradients**: Pre-calculate gradients if used repeatedly

## ✅ Status & Compatibility

- **Maturity**: Production-tested, stable for desktop applications
- **.NET Target**: .NET 8.0+ (primary), .NET Framework 4.7+ (legacy)
- **Platform Support**: Windows primary; limited cross-platform via System.Drawing.Common
- **Performance**: Resizing is CPU-intensive; cache results when possible

## 📖 Related Projects

- [Hydrogen](../Hydrogen) - Core framework
- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - WinForms integration with drawing support
- [Hydrogen.Windows](../Hydrogen.Windows) - Windows-specific graphics APIs

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
