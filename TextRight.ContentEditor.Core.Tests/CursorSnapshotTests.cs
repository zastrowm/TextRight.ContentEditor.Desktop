using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Tests
{
  internal class CursorSnapshotTests
  {
    private TextBlock _textBlock;

    [SetUp]
    public void Setup()
    {
      _textBlock = new TextBlock();
      var cursor = _textBlock.GetTextCursor().ToBeginning();
      cursor.InsertText("This is the beginning of the paragraph");
    }

    [Test]
    public void StateCanBeRestored()
    {
      var end = _textBlock.GetTextCursor().ToEnd();

      var random = _textBlock.GetTextCursor().ToBeginning();
      random.Move(3);

      using (var snapshot = CursorSnapshot.From(random))
      {
        random.MoveTo(end);
        // proof that the cursor was changed
        Assert.That(random.CharacterAfter, Is.EqualTo('\0'));

        snapshot.Restore(random);
      }

      Assert.That(random.CharacterAfter, Is.EqualTo('s'));
      Assert.That(random.CharacterBefore, Is.EqualTo('i'));
    }
  }
}