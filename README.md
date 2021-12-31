# Font Alignment

Font Alignment is a set of tools for adjusting the vertical center of text taking font metric into account.

- __Font Alignment:__ Core library.

- __Font Alignment Finder:__ A Windows tool to visualize font metric so as to find the vertical center of Uppercase characters or that of Extent as well as its offset from the vertical center of font. That information will be helpful for software developpers and designers to adjust the vertical alignment of text.<br /><br />
![Font Alignment Finder](Images/FontAlignmentFinder.png)

- __Font Alignment Demo:__ A WPF app to demonstrate attached property included in core library for `TextBlock`, `TextBox` or other `Control`. It will automatically adjust the vertical alignment of text by changing padding.<br /><br />
![Font Alignment Demo](Images/FontAlignmentDemo.png)

## Requirements

The core library works on Windows and compatible with:

.NET 5.0|.NET Framework 4.7.2
-|-

The calling app must include app.manifest which declares DPI awareness.

## Background

The basic way to obtain font metric is explained in the following article.

 - [Get font metrics in a WPF program using C#](http://csharphelper.com/blog/2015/05/get-font-metrics-in-a-wpf-program-using-c/)

 You can align a text inside the element which hosts the text by setting vertical alignment. It is generally easy but it might not be enough to place the characters of text TRUELY at the vertical center of element because  font glyphs are often more or less aligned to the top or bottom inside font.
 
 The degree of offset between the center of font glyphs and the center of font depends on each font and character. This offset is a part of font design but sometimes it could be troublesome. To fine-tune the vertical alignment of text, you need to move the text by arbitrary length but it is not always easy to find the appropriate length.

Font Alignment Finder will greatly help to find the appropriate length for the specified font family, style, weight and size.
