using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests.ObjectModel.Blocks.Text
{
  internal class MarkupCollectionTests
  {
    private MarkupCollection _collection;
    private readonly MarkupType _fakeMarkupType = default(MarkupType);

    public TextRange[] Ranges
      => _collection.Select(s => s.GetRange()).ToArray();

    [SetUp]
    public void Setup()
    {
      _collectionOwner = Mock.Of<IMarkupCollectionOwner>(c => c.Length == 15);
      _collection = new MarkupCollection(_collectionOwner);

      _allValidTextRanges = new Lazy<List<TextRange>>(() => GetAllTextRanges().ToList());
    }

    /// <summary> All the valid text ranges for the current fragment. </summary>
    private IEnumerable<TextRange> GetAllTextRanges()
    {
      for (int startIndex = 0; startIndex < _collectionOwner.Length; startIndex++)
      {
        for (int endIndex = startIndex; endIndex < _collectionOwner.Length; endIndex++)
        {
          yield return new TextRange(startIndex, endIndex);
        }
      }
    }

    [Test]
    public void MarkRange_Works()
    {
      var markup = _collection.MarkRange(new TextRange(0, 3), _fakeMarkupType, null);

      markup.Should().NotBeNull();
    }

    [Test]
    public void MarkRange_ThrowsWhenOutOfRange()
    {
      CreateActionFor(new TextRange(-1, 3)).ShouldThrow<ArgumentOutOfRangeException>();
      CreateActionFor(new TextRange(3, _collectionOwner.Length + 1)).ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    public void MarkRange_DoesNotThrowForFullRange()
    {
      CreateActionFor(new TextRange(0, _collectionOwner.Length)).ShouldNotThrow();
    }

    private Action CreateActionFor(TextRange r)
    {
      return () => _collection.MarkRange(r, _fakeMarkupType, null);
    }

    [Test]
    public void MarkRange_ActuallyAdds()
    {
      _collection.MarkRange(new TextRange(0, 3), _fakeMarkupType, null);

      var first = _collection.First();
      Assert.That(first.GetRange(), Is.EqualTo(new TextRange(0, 3)));
    }

    [Test]
    public void MarkRange_MultipleItemsAddsThemAll()
    {
      _collection.MarkRange(new TextRange(0, 3), _fakeMarkupType, null);
      _collection.MarkRange(new TextRange(0, 5), _fakeMarkupType, null);
      _collection.MarkRange(new TextRange(0, 7), _fakeMarkupType, null);

      _collection.Should().HaveCount(3);

      _collection.Should().Contain(it => it.GetRange().Equals(new TextRange(0, 3)));
      _collection.Should().Contain(it => it.GetRange().Equals(new TextRange(0, 5)));
      _collection.Should().Contain(it => it.GetRange().Equals(new TextRange(0, 7)));
    }

    [Test]
    public void MarkRange_NonZeroItem_Works()
    {
      _collection.MarkRange(new TextRange(1, 7), _fakeMarkupType, null);
      _collection.Should().HaveCount(1);

      Ranges.Should().HaveElementAt(0, new TextRange(1, 7));
    }

    [Test]
    public void MarkRange_InMiddleWorks()
    {
      _collection.MarkRange(new TextRange(0, 8), _fakeMarkupType, null);
      _collection.MarkRange(new TextRange(1, 7), _fakeMarkupType, null);

      Ranges.Should().HaveElementAt(0, new TextRange(0, 8));
      Ranges.Should().HaveElementAt(1, new TextRange(1, 7));
    }

    [Test]
    public void MarkRange_OutOfOrderMaintainsSortedOrder()
    {
      _collection.MarkRange(new TextRange(3, 4), _fakeMarkupType, null);
      _collection.MarkRange(new TextRange(1, 7), _fakeMarkupType, null);
      _collection.MarkRange(new TextRange(0, 8), _fakeMarkupType, null);

      Ranges.Should().HaveElementAt(0, new TextRange(0, 8));
      Ranges.Should().HaveElementAt(1, new TextRange(1, 7));
      Ranges.Should().HaveElementAt(2, new TextRange(3, 4));
    }

    [Test]
    public void MarkRange_TryMany()
    {
      /*
       * 01234567890123456
       * #### 
       *   ########
       *       ######
       *       ########
       */
      var originalRanges = new[]
                           {
                             new TextRange(0, 4),
                             new TextRange(2, 10),
                             new TextRange(6, 12),
                             new TextRange(7, 14),
                           };

      AddAllRangesToCollection(originalRanges);

      for (var i = 0; i < originalRanges.Length; i++)
      {
        Ranges.Should().HaveElementAt(i, originalRanges[i]);
      }
    }

    [Test]
    public void MarkRange_ReturnsInstance_ThatsValidAfterOperation()
    {
      var instance = _collection.MarkRange(new TextRange(0, 10), _fakeMarkupType, null);
      _collection.UpdateFromEvent(new TextModification(4, 4, true));

      _collection.First().Should().BeSameAs(instance);
    }

    [Test]
    public void InsertText_BeforeRange_ShiftsRangesOver()
    {
      Verify(new TextRange(3, 8),
             new TextModification(1, 3, true),
             new TextRange(6, 11));
    }

    [Test]
    public void InsertText_AfterRange_DoesNothing()
    {
      Verify(new TextRange(3, 8),
             new TextModification(10, 3, true),
             new TextRange(3, 8));
    }

    [Test]
    public void InsertText_AtBeginningOfRange_DoesNotChangeLengthOfRange()
    {
      Verify(new TextRange(3, 8),
             new TextModification(3, 3, true),
             new TextRange(6, 11));
    }

    [Test]
    public void InsertText_AtEndOfRange_DoesNotChangeLengthOfRange()
    {
      Verify(new TextRange(3, 8),
             new TextModification(8, 3, true),
             new TextRange(3, 8));
    }

    [Test]
    public void InsertText_InMiddleOfRange_ExtendsRange()
    {
      Verify(new TextRange(3, 8),
             new TextModification(4, 1, true),
             new TextRange(3, 9));
    }

    [Test]
    public void InsertText_TryMany()
    {
      /*
       * 0123456789012345
       * #### 
       *   ########
       *       ######
       *       ########
       */
      var originalRanges = new[]
                           {
                             new TextRange(0, 4),
                             new TextRange(2, 10),
                             new TextRange(6, 12),
                             new TextRange(6, 14),
                           };

      /*
       * 01234567890123456
       * #### 
       *   ####__####
       *         ######
       *         ########
       */
      var newRanges = new[]
                      {
                        new TextRange(0, 4),
                        new TextRange(2, 12),
                        new TextRange(8, 14),
                        new TextRange(8, 16),
                      };

      AddAllRangesToCollection(originalRanges);

      _collection.UpdateFromEvent(new TextModification(6, 2, true));

      Ranges.Should().HaveElementAt(0, newRanges[0]);
      Ranges.Should().HaveElementAt(1, newRanges[1]);

      // note that technically the api doesn't guarantee that just be cause we added #3 after #2 that
      // the order was maintained so this could fail the test in the future if that changes. 
      Ranges.Should().HaveElementAt(2, newRanges[2]);
      Ranges.Should().HaveElementAt(3, newRanges[3]);
    }

    private Lazy<List<TextRange>> _allValidTextRanges;
    private IMarkupCollectionOwner _collectionOwner;

    [Test]
    public void InsertText_Iterations()
    {
      // it's actually very easy to take a given TextRange and TextModification and figure out what
      // the output region does (SubFragmentMarkupCollection is so complicated because it manages the
      // text-ranges in an efficient, easy to update manner).  Given that the algorithm is so easy
      // that it can be implemented in the function Transform, we can easily loop through every
      // combination of range and modification and verify that we get what we expect.  That's what
      // this function does. 

      for (int insertPoint = 0; insertPoint <= _collectionOwner.Length; insertPoint++)
      {
        for (int endInsertPoint = insertPoint; endInsertPoint <= _collectionOwner.Length; endInsertPoint++)
        {
          int length = endInsertPoint - insertPoint;
          var modification = new TextModification(insertPoint, length, true);

          VerifyCorrectness(modification);
        }
      }
    }

    [Test]
    public void DeleteText_AtBeginning_RemovesLength()
    {
      Verify(new TextRange(3, 8),
             new TextModification(3, 1, false),
             new TextRange(3, 7));
    }

    [Test]
    public void DeleteText_AtBeginningWithLongRange_CollapsesRange()
    {
      Verify(new TextRange(3, 8),
             new TextModification(3, 10, false),
             new TextRange(3, 3));
    }

    [Test]
    public void DeleteText_BeforeWithLongRange_ShiftsAndCollapsesRange()
    {
      Verify(new TextRange(3, 8),
             new TextModification(1, 10, false),
             new TextRange(1, 1));
    }

    [Test]
    public void DeleteText_UptoRange_RemovesNothing()
    {
      Verify(new TextRange(1, 2),
             new TextModification(0, 1, false),
             new TextRange(0, 1));
    }

    [Test]
    public void DeleteText_Before_ShiftsRange()
    {
      Verify(new TextRange(3, 8),
             new TextModification(1, 1, false),
             new TextRange(2, 7));
    }

    [Test]
    public void DeleteText_BeforeEmptyRange_ShiftsRange()
    {
      Verify(new TextRange(1, 1),
             new TextModification(0, 1, false),
             new TextRange(0, 0));
    }

    [Test]
    public void DeleteText_AfterEnd_DoesNothing()
    {
      Verify(new TextRange(3, 8),
             new TextModification(8, 3, false),
             new TextRange(3, 8));
    }

    [Test]
    public void DeleteText_AfterStart_RemovesLength()
    {
      Verify(new TextRange(3, 8),
             new TextModification(4, 8, false),
             new TextRange(3, 4));
    }

    [Test]
    public void DeleteText_InRange_RemovesLength()
    {
      Verify(new TextRange(3, 8),
             new TextModification(4, 2, false),
             new TextRange(3, 6));
    }

    [Test]
    public void DeleteText_TryMany()
    {
      /*
       * 012345678901234
       * #### 
       *   ####__####
       *        ###
       *         ######
       *         #######
       *          ####
       */

      var originalRanges = new[]
                           {
                             new TextRange(0, 4),
                             new TextRange(2, 12),
                             new TextRange(7, 10),
                             new TextRange(8, 14),
                             new TextRange(8, 15),
                             new TextRange(9, 13),
                           };

      /*
       * 012345678901234
       * #### 
       *   ########
       *       ##
       *       ######
       *       #######
       *        ####
       */
      var newRanges = new[]
                      {
                        new TextRange(0, 4),
                        new TextRange(2, 10),
                        new TextRange(6, 8),
                        new TextRange(6, 12),
                        new TextRange(6, 13),
                        new TextRange(7, 11),
                      };

      originalRanges.ToList().ForEach(
                      r => _collection.MarkRange(r, _fakeMarkupType, null)
                    );

      _collection.UpdateFromEvent(new TextModification(6, 2, false));

      Ranges.Should().HaveElementAt(0, newRanges[0]);
      Ranges.Should().HaveElementAt(1, newRanges[1]);

      // note that technically the api doesn't guarantee that just be cause we added #3 after #2 that
      // the order was maintained so this could fail the test in the future if that changes. 
      Ranges.Should().HaveElementAt(2, newRanges[2]);
      Ranges.Should().HaveElementAt(3, newRanges[3]);
    }

    [Test]
    public void DeleteText_Iterations()
    {
      // it's actually very easy to take a given TextRange and TextModification and figure out what
      // the output region does (SubFragmentMarkupCollection is so complicated because it manages the
      // text-ranges in an efficient, easy to update manner).  Given that the algorithm is so easy
      // that it can be implemented in the function Transform, we can easily loop through every
      // combination of range and modification and verify that we get what we expect.  That's what
      // this function does. 

      for (int insertPoint = 0; insertPoint <= _collectionOwner.Length; insertPoint++)
      {
        for (int endInsertPoint = insertPoint; endInsertPoint <= _collectionOwner.Length; endInsertPoint++)
        {
          int length = endInsertPoint - insertPoint;
          var modification = new TextModification(insertPoint, length, false);

          VerifyCorrectness(modification);
        }
      }
    }

    private void VerifyCorrectness(TextModification modification)
    {
      // clear it so that we're at a clean slate
      Setup();

      AddAllRangesToCollection(_allValidTextRanges.Value);
      _collection.UpdateFromEvent(modification);

      var rangesOriginal = _allValidTextRanges.Value;
      var actualRanges = Ranges;

      for (var i = 0; i < Ranges.Length; i++)
      {
        var type = modification.WasAdded ? "insert" : "deletion";
        var msg =
          $"'operation {type} @{modification.Index} of @{modification.NumberOfCharacters} characters'.  Original was {rangesOriginal[i]}";

        var expected = Transform(rangesOriginal[i], modification);
        actualRanges.Should().HaveElementAt(i, expected, msg);
      }
    }

    /// <summary>
    ///  Verifies that when the given modification is operated on the given range, that the expect
    ///  range is output.
    /// </summary>
    private void Verify(TextRange original, TextModification modification, TextRange expected)
    {
      // note, technically the single range is all that SHOULD be tested, but since everything
      // funnels through here, we might as well test that when the ranges are duplicated, that all of
      // the ranges end up the same (this wasn't always the case :: ) 

      {
        // single range, easy
        Setup();
        _collection.MarkRange(original, _fakeMarkupType, null);

        /* Extra Check */
        var originalMarkup = _collection.First();
        originalMarkup.GetRange().ShouldBeEquivalentTo(original);
        /* /Extra Check */

        _collection.UpdateFromEvent(modification);
        Ranges.Should().HaveElementAt(0, expected);

        /* Extra Check */
        var latestMarkup = _collection.First();
        latestMarkup.Should().BeSameAs(originalMarkup);
        /* /Extra Check */
      }

      {
        // duplicated range, let's make sure that works right
        Setup();
        _collection.MarkRange(original, _fakeMarkupType, null);
        _collection.MarkRange(original, _fakeMarkupType, null);
        _collection.UpdateFromEvent(modification);
        Ranges.Should().HaveElementAt(0, expected, "'duplicated twice'");
        Ranges.Should().HaveElementAt(1, expected, "'duplicated twice'");
      }

      {
        // duplicated duplicated range, just for good measure
        Setup();
        _collection.MarkRange(original, _fakeMarkupType, null);
        _collection.MarkRange(original, _fakeMarkupType, null);
        _collection.MarkRange(original, _fakeMarkupType, null);
        _collection.UpdateFromEvent(modification);
        Ranges.Should().HaveElementAt(0, expected, "'duplicated thrice'");
        Ranges.Should().HaveElementAt(1, expected, "'duplicated thrice'");
        Ranges.Should().HaveElementAt(2, expected, "'duplicated thrice'");
      }

      // this is really to make sure that our Transform method is working correctly
      var transformed = Transform(original, modification);
      transformed.Should().Be(expected, "the Transform is supposed to be correct");
    }

    private void AddAllRangesToCollection(IEnumerable<TextRange> ranges)
    {
      foreach (var range in ranges)
      {
        _collection.MarkRange(range, _fakeMarkupType, null);
      }
    }

    /// <summary> Transforms a single range using the given modification. </summary>
    private TextRange Transform(TextRange range, TextModification modification)
    {
      if (modification.WasAdded)
      {
        if (range.ContainsExclusive(modification.Index))
        {
          return new TextRange(range.StartIndex, range.EndIndex + modification.NumberOfCharacters);
        }
        else if (range.StartIndex < modification.Index)
        {
          return range;
        }
        else
        {
          return new TextRange(range.StartIndex + modification.NumberOfCharacters,
                               range.EndIndex + modification.NumberOfCharacters);
        }
      }
      else
      {
        var deletionRange = new TextRange(modification.Index, modification.Index + modification.NumberOfCharacters);

        if (deletionRange.ContainsInclusive(range.StartIndex) && deletionRange.ContainsInclusive(range.EndIndex))
        {
          // if the deletion range took out the range, remove it altogether
          return new TextRange(deletionRange.StartIndex, deletionRange.StartIndex);
        }
        else if (deletionRange.OverlapsInclusive(range))
        {
          if (deletionRange.StartIndex <= range.StartIndex)
          {
            int overlappingCharCount = deletionRange.EndIndex - range.StartIndex;
            int numberOfCharactersBefore = range.StartIndex - deletionRange.StartIndex;

            int numberOfAvailableCharsToDelete = Math.Min(overlappingCharCount,
                                                          range.Length);

            return new TextRange(range.StartIndex - numberOfCharactersBefore,
                                 range.EndIndex - numberOfCharactersBefore - numberOfAvailableCharsToDelete);
          }
          else /* range.StartIndex <= deletionRange.EndIndex */
          {
            var numberOfCharsToDelete = Math.Min(range.EndIndex - deletionRange.StartIndex, deletionRange.Length);
            return new TextRange(range.StartIndex, range.EndIndex - numberOfCharsToDelete);
          }
        }
        else if (deletionRange.StartIndex < range.StartIndex)
        {
          return new TextRange(range.StartIndex - modification.NumberOfCharacters,
                               range.EndIndex - modification.NumberOfCharacters);
        }
        else
        {
          return range;
        }
      }
    }
  }
}