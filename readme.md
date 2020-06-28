The foundations of a text-document editor, written from the ground up using WPF primitives .

## Features

A simple text-editor control that supports:

- Headers
- Paragraphs
- Undo/Redo

Not yet ready for production, but a cool tinkering project.

## Historical Notes

I liked markdown, but wanted to be able to add more rich-text; specifically I wanted a representation for:

- new blocks like notes/warnings/errors
- a way to add additional meta-data to blocks
- images to be stored with the document

In 2013, I started off with a web-based side-by-side markdown editor & preview called [EMD-Editor](https://github.com/zastrowm/emd-editor) - the thing I liked about it was that it allowed images to be uploaded and stored with the document as base64.

Along the lines of markup language, I toyed with the idea of a a markdown-language which I named *TextRight* (get it, it's not mark-*down*, but text-*right*).  It didn't get much past the idea phase, but I did have a [repository for the start of the language](https://github.com/zastrowm/TextRight). 

Eventually, I realized what I was really looking for what as a rich-text-document *editor*, not file format - I simply focused on the file format because that's what I was familiar with.  So, with that in mind, I started writing a rich-text-document editor, focusing on document semantics instead of text formatting - that is, the document wouldn't represent bolded text, it would represent it as emphasized text and allow the renderer concern itself with how emphasized text was presented.

This led to [TextRight.ContentEditor](https://github.com/zastrowm/TextRight.ContentEditor), which was an implementation in HTML/TypeScript. I was able to get to the point of a [cool web demo](http://github.programdotrun.com/TextRight.ContentEditor.Demo/) in which text selection and text cursor movement was written from the ground-up - it didn't use content-editor or any libraries to calculate positions etc.

Eventually I rewrote it via [TextRight.ContentEditor.Next](https://github.com/zastrowm/TextRight.ContentEditor.Next) with a focus of adding undo/redo from the beginning and making sure that tests were written from the beginning using TDD.

In 2015, I decided I wanted to go back to the desktop and rewrote it in C#/WPF via [TextRight.ContentEditor.Desktop](https://github.com/zastrowm/TextRight.ContentEditor.Desktop), again with a good focus on testing.  Over the years, I've continued tinkering and adding small features, but didn't add many features beyond basic text editing.  However, it did spawn [dotnet-guaper](https://github.com/zastrowm/dotnet-guapr) primarily to speed up development of the editor.



