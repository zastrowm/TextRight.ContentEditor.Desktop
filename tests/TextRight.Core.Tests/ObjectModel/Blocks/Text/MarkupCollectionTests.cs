using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;
using Range = TextRight.Core.ObjectModel.Range;

namespace TextRight.Core.Tests.ObjectModel.Blocks.Text
{
  public class MarkupCollectionTests
  {
    private const int TextLength = 15;

    private MarkupCollection _collection;
    private Lazy<List<Range>> _allValidTextRanges;
    private IMarkupCollectionOwner _collectionOwner;
    private IMarkupDescriptor _markupDescriptor;

    public MarkupCollectionTests()
    {
      Reset();
    }
    private void Reset()
    {
      _markupDescriptor = Mock.Of<IMarkupDescriptor>();
      _collectionOwner = Mock.Of<IMarkupCollectionOwner>(c => c.Length == TextLength);
      _collection = new MarkupCollection(_collectionOwner);

      _allValidTextRanges = new Lazy<List<Range>>(() => GetAllTextRanges().ToList());
    }

    public static IEnumerable<object[]> GetAllInsertionPoints()
    {
      for (int insertPoint = 0; insertPoint <= TextLength; insertPoint++)
      {
        for (int endInsertPoint = insertPoint; endInsertPoint <= TextLength; endInsertPoint++)
        {
          int length = endInsertPoint - insertPoint;
          yield return new object[] { insertPoint, length };
        }
      }
    }

    public Range[] Ranges
      => _collection.Select(s => s.GetRange()).ToArray();

    /// <summary> All the valid text ranges for the current fragment. </summary>
    private IEnumerable<Range> GetAllTextRanges()
    {
      for (int startIndex = 0; startIndex < _collectionOwner.Length; startIndex++)
      {
        for (int endIndex = startIndex; endIndex < _collectionOwner.Length; endIndex++)
        {
          yield return new Range(startIndex, endIndex);
        }
      }
    }

    [Fact]
    public void MarkRange_Works()
    {
      var markup = _collection.MarkRange(new Range(0, 3), _markupDescriptor, null);

      DidYouKnow.That(markup).Should().NotBeNull();
    }

    [Fact]
    public void MarkRange_ThrowsWhenOutOfRange()
    {
      CreateActionFor(new Range(-1, 3)).Should().Throw<ArgumentOutOfRangeException>();
      CreateActionFor(new Range(3, _collectionOwner.Length + 1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void MarkRange_DoesNotThrowForFullRange()
    {
      CreateActionFor(new Range(0, _collectionOwner.Length)).Should().NotThrow();
    }

    private Action CreateActionFor(Range r)
    {
      return () => _collection.MarkRange(r, _markupDescriptor, null);
    }

    [Fact]
    public void MarkRange_ActuallyAdds()
    {
      _collection.MarkRange(new Range(0, 3), _markupDescriptor, null);

      var first = _collection.First();
      DidYouKnow.That(first.GetRange()).Should().Be(new Range(0, 3));
    }

    [Fact]
    public void MarkRange_MultipleItemsAddsThemAll()
    {
      _collection.MarkRange(new Range(0, 3), _markupDescriptor, null);
      _collection.MarkRange(new Range(0, 5), _markupDescriptor, null);
      _collection.MarkRange(new Range(0, 7), _markupDescriptor, null);

      DidYouKnow.That(_collection).Should()
                .HaveCount(3)
                .And.Contain(it => it.GetRange().Equals(new Range(0, 3)))
                .And.Contain(it => it.GetRange().Equals(new Range(0, 5)))
                .And.Contain(it => it.GetRange().Equals(new Range(0, 7)));
    }

    [Fact]
    public void MarkRange_NonZeroItem_Works()
    {
      _collection.MarkRange(new Range(1, 7), _markupDescriptor, null);
      _collection.Should().HaveCount(1);

      DidYouKnow.That(Ranges).Should().HaveElementAt(0, new Range(1, 7));
    }

    [Fact]
    public void MarkRange_InMiddleWorks()
    {
      _collection.MarkRange(new Range(0, 8), _markupDescriptor, null);
      _collection.MarkRange(new Range(1, 7), _markupDescriptor, null);

      DidYouKnow.That(Ranges).Should().HaveElementAt(0, new Range(0, 8));
      DidYouKnow.That(Ranges).Should().HaveElementAt(1, new Range(1, 7));
    }

    [Fact]
    public void MarkRange_OutOfOrderMaintainsSortedOrder()
    {
      _collection.MarkRange(new Range(3, 4), _markupDescriptor, null);
      _collection.MarkRange(new Range(1, 7), _markupDescriptor, null);
      _collection.MarkRange(new Range(0, 8), _markupDescriptor, null);

      DidYouKnow.That(Ranges).Should().HaveElementAt(0, new Range(0, 8));
      DidYouKnow.That(Ranges).Should().HaveElementAt(1, new Range(1, 7));
      DidYouKnow.That(Ranges).Should().HaveElementAt(2, new Range(3, 4));
    }

    [Fact]
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
                             new Range(0, 4), new Range(2, 10), new Range(6, 12), new Range(7, 14),
                           };

      AddAllRangesToCollection(originalRanges);

      for (var i = 0; i < originalRanges.Length; i++)
      {
        DidYouKnow.That(Ranges).Should().HaveElementAt(i, originalRanges[i]);
      }
    }

    [Fact]
    public void MarkRange_ReturnsInstance_ThatsValidAfterOperation()
    {
      var instance = _collection.MarkRange(new Range(0, 10), _markupDescriptor, null);
      _collection.UpdateFromEvent(new RangeModification(4, 4, true));

      DidYouKnow.That(_collection).Should().HaveElementAt(0, instance);
    }

    [Fact]
    public void InsertText_BeforeRange_ShiftsRangesOver()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(1, 3, true),
                         new Range(6, 11));
    }

    [Fact]
    public void InsertText_AfterRange_DoesNothing()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(10, 3, true),
                         new Range(3, 8));
    }

    [Fact]
    public void InsertText_AtBeginningOfRange_DoesNotChangeLengthOfRange()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(3, 3, true),
                         new Range(6, 11));
    }

    [Fact]
    public void InsertText_AtEndOfRange_DoesNotChangeLengthOfRange()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(8, 3, true),
                         new Range(3, 8));
    }

    [Fact]
    public void InsertText_InMiddleOfRange_ExtendsRange()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(4, 1, true),
                         new Range(3, 9));
    }

    [Fact]
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
                             new Range(0, 4), new Range(2, 10), new Range(6, 12), new Range(6, 14),
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
                        new Range(0, 4), new Range(2, 12), new Range(8, 14), new Range(8, 16),
                      };

      AddAllRangesToCollection(originalRanges);

      _collection.UpdateFromEvent(new RangeModification(6, 2, true));

      DidYouKnow.That(Ranges).Should().HaveElementAt(0, newRanges[0]);
      DidYouKnow.That(Ranges).Should().HaveElementAt(0, newRanges[0]);
      DidYouKnow.That(Ranges).Should().HaveElementAt(1, newRanges[1]);

      // note that technically the api doesn't guarantee that just be cause we added #3 after #2 that
      // the order was maintained so this could fail the test in the future if that changes. 
      DidYouKnow.That(Ranges).Should().HaveElementAt(2, newRanges[2]);
      DidYouKnow.That(Ranges).Should().HaveElementAt(3, newRanges[3]);
    }

    [Theory]
    [MemberData(nameof(GetAllInsertionPoints))]
    public void InsertText_Iterations(int insertionPoint, int length)
    {
      // it's actually very easy to take a given TextRange and TextModification and figure out what
      // the output region does (SubFragmentMarkupCollection is so complicated because it manages the
      // text-ranges in an efficient, easy to update manner).  Given that the algorithm is so easy
      // that it can be implemented in the function Transform, we can easily loop through every
      // combination of range and modification and verify that we get what we expect.  That's what
      // this function does. 
      var actualModification = new RangeModification(insertionPoint, length, wasAdded: true);
      VerifyCorrectness(actualModification);
    }

    [Fact]
    public void DeleteText_AtBeginning_RemovesLength()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(3, 1, false),
                         new Range(3, 7));
    }

    [Fact]
    public void DeleteText_AtBeginningWithLongRange_CollapsesRange()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(3, 10, false),
                         new Range(3, 3));
    }

    [Fact]
    public void DeleteText_BeforeWithLongRange_ShiftsAndCollapsesRange()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(1, 10, false),
                         new Range(1, 1));
    }

    [Fact]
    public void DeleteText_UptoRange_RemovesNothing()
    {
      VerifyModification(new Range(1, 2),
                         new RangeModification(0, 1, false),
                         new Range(0, 1));
    }

    [Fact]
    public void DeleteText_Before_ShiftsRange()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(1, 1, false),
                         new Range(2, 7));
    }

    [Fact]
    public void DeleteText_BeforeEmptyRange_ShiftsRange()
    {
      VerifyModification(new Range(1, 1),
                         new RangeModification(0, 1, false),
                         new Range(0, 0));
    }

    [Fact]
    public void DeleteText_AfterEnd_DoesNothing()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(8, 3, false),
                         new Range(3, 8));
    }

    [Fact]
    public void DeleteText_AfterStart_RemovesLength()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(4, 8, false),
                         new Range(3, 4));
    }

    [Fact]
    public void DeleteText_InRange_RemovesLength()
    {
      VerifyModification(new Range(3, 8),
                         new RangeModification(4, 2, false),
                         new Range(3, 6));
    }

    [Fact]
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
                             new Range(0, 4), new Range(2, 12), new Range(7, 10), new Range(8, 14), new Range(8, TextLength), new Range(9, 13),
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
                        new Range(0, 4), new Range(2, 10), new Range(6, 8), new Range(6, 12), new Range(6, 13), new Range(7, 11),
                      };

      originalRanges.ToList()
                    .ForEach(r => _collection.MarkRange(r, _markupDescriptor, null));

      _collection.UpdateFromEvent(new RangeModification(6, 2, false));

      DidYouKnow.That(Ranges).Should().HaveElementAt(0, newRanges[0]);
      DidYouKnow.That(Ranges).Should().HaveElementAt(1, newRanges[1]);

      // note that technically the api doesn't guarantee that just be cause we added #3 after #2 that
      // the order was maintained so this could fail the test in the future if that changes. 
      DidYouKnow.That(Ranges).Should().HaveElementAt(2, newRanges[2]);
      DidYouKnow.That(Ranges).Should().HaveElementAt(3, newRanges[3]);
    }

    [Theory]
    [MemberData(nameof(GetAllInsertionPoints))]
    public void DeleteText_Iterations(int insertionPoint, int length)
    {
      // it's actually very easy to take a given TextRange and TextModification and figure out what
      // the output region does (SubFragmentMarkupCollection is so complicated because it manages the
      // text-ranges in an efficient, easy to update manner).  Given that the algorithm is so easy
      // that it can be implemented in the function Transform, we can easily loop through every
      // combination of range and modification and verify that we get what we expect.  That's what
      // this function does. 

      var actualModification = new RangeModification(insertionPoint, length, wasAdded: false);
      VerifyCorrectness(actualModification);
    }

    private void VerifyCorrectness(RangeModification modification)
    {
      AddAllRangesToCollection(_allValidTextRanges.Value);
      _collection.UpdateFromEvent(modification);

      var rangesOriginal = _allValidTextRanges.Value;
      var actualRanges = Ranges;

      for (var i = 0; i < Ranges.Length; i++)
      {
        var type = modification.WasAdded ? "insert" : "deletion";
        var msg = $"'operation {type} @{modification.Index} of @{modification.NumberOfItems} characters'.  Original was {rangesOriginal[i]}";

        var expected = Transform(rangesOriginal[i], modification);
        DidYouKnow.That(Ranges).Should().HaveElementAt(i, expected, because: msg); // msg
      }
    }

    /// <summary>
    ///  Verifies that when the given modification is operated on the given range, that the expect
    ///  range is output.
    /// </summary>
    private void VerifyModification(Range original, RangeModification modification, Range expected)
    {
      // note, technically the single range is all that SHOULD be tested, but since everything
      // funnels through here, we might as well test that when the ranges are duplicated, that all of
      // the ranges end up the same (this wasn't always the case :: ) 

      {
        // single range, easy
        Reset();
        _collection.MarkRange(original, _markupDescriptor, null);

        /* Extra Check */
        var originalMarkup = _collection.First();
        originalMarkup.GetRange().Should().BeEquivalentTo(original);
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
        Reset();
        _collection.MarkRange(original, _markupDescriptor, null);
        _collection.MarkRange(original, _markupDescriptor, null);
        _collection.UpdateFromEvent(modification);
        Ranges.Should().HaveElementAt(0, expected, "'duplicated twice'");
        Ranges.Should().HaveElementAt(1, expected, "'duplicated twice'");
      }

      {
        // duplicated duplicated range, just for good measure
        Reset();
        _collection.MarkRange(original, _markupDescriptor, null);
        _collection.MarkRange(original, _markupDescriptor, null);
        _collection.MarkRange(original, _markupDescriptor, null);
        _collection.UpdateFromEvent(modification);

        Ranges.Should().HaveElementAt(0, expected, "'duplicated thrice'");
        Ranges.Should().HaveElementAt(1, expected, "'duplicated thrice'");
        Ranges.Should().HaveElementAt(2, expected, "'duplicated thrice'");
      }

      // this is really to make sure that our Transform method is working correctly
      var transformed = Transform(original, modification);
      transformed.Should().Be(expected, "the Transform is supposed to be correct");
    }

    private void AddAllRangesToCollection(IEnumerable<Range> ranges)
    {
      foreach (var range in ranges)
      {
        _collection.MarkRange(range, _markupDescriptor, null);
      }
    }

    /// <summary> Transforms a single range using the given modification. </summary>
    private Range Transform(Range range, RangeModification modification)
    {
      if (modification.WasAdded)
      {
        if (range.ContainsExclusive(modification.Index))
        {
          return new Range(range.StartIndex, range.EndIndex + modification.NumberOfItems);
        }
        else if (range.StartIndex < modification.Index)
        {
          return range;
        }
        else
        {
          return new Range(range.StartIndex + modification.NumberOfItems,
                               range.EndIndex + modification.NumberOfItems);
        }
      }
      else
      {
        var deletionRange = new Range(modification.Index, modification.Index + modification.NumberOfItems);

        if (deletionRange.ContainsInclusive(range.StartIndex) && deletionRange.ContainsInclusive(range.EndIndex))
        {
          // if the deletion range took out the range, remove it altogether
          return new Range(deletionRange.StartIndex, deletionRange.StartIndex);
        }
        else if (deletionRange.OverlapsInclusive(range))
        {
          if (deletionRange.StartIndex <= range.StartIndex)
          {
            int overlappingCharCount = deletionRange.EndIndex - range.StartIndex;
            int numberOfCharactersBefore = range.StartIndex - deletionRange.StartIndex;

            int numberOfAvailableCharsToDelete = Math.Min(overlappingCharCount,
                                                          range.Length);

            return new Range(range.StartIndex - numberOfCharactersBefore,
                                 range.EndIndex - numberOfCharactersBefore - numberOfAvailableCharsToDelete);
          }
          else /* range.StartIndex <= deletionRange.EndIndex */
          {
            var numberOfCharsToDelete = Math.Min(range.EndIndex - deletionRange.StartIndex, deletionRange.Length);
            return new Range(range.StartIndex, range.EndIndex - numberOfCharsToDelete);
          }
        }
        else if (deletionRange.StartIndex < range.StartIndex)
        {
          return new Range(range.StartIndex - modification.NumberOfItems,
                               range.EndIndex - modification.NumberOfItems);
        }
        else
        {
          return range;
        }
      }
    }

    public class TestRangeModification
    {
      public TestRangeModification(RangeModification modification)
      {
        Modification = modification;
      }

      public RangeModification Modification { get; }

      public override string ToString()
        => $"({Modification.Index:00}, {(Modification.Index + Modification.NumberOfItems):00}, WasAdded={Modification.WasAdded}";
    }
  }
}