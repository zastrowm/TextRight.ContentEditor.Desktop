using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NFluent;

using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

using Xunit;

namespace TextRight.Core.Tests
{
  public class CursorSnapshotTests
  {
    private TextBlock _textBlock;

    public CursorSnapshotTests()
    {
      _textBlock = new ParagraphBlock();
      var cursor = _textBlock.GetTextCursor().ToBeginning();
      cursor.InsertText("This is the beginning of the paragraph");
    }

    [Fact]
    public void StateCanBeRestored()
    {
      var end = _textBlock.GetTextCursor().ToEnd();

      var random = _textBlock.GetTextCursor().ToBeginning();
      random.Move(3);

      using (var snapshot = CursorSnapshot.From(random))
      {
        random.MoveTo(end);
        // proof that the cursor was changed
        Check.That(random.CharacterAfter).IsEqualTo('\0');

        snapshot.Restore(random);
      }

      Check.That(random.CharacterAfter).IsEqualTo('s');
      Check.That(random.CharacterBefore).IsEqualTo('i');
    }
  }
}