using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Tests.Framework;
using Xunit;
using Xunit.Abstractions;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class BlockTreeWalkerTests
  {
    private static Block a;
    private static Block a1;
    private static Block a11;
    private static Block a12;
    private static Block a13;
    private static Block b;
    private static Block b1;
    private static Block b11;
    private static Block b12;
    private static Block b111;
    private static Block c;
    private static readonly Block NullBlock = null;
    private static readonly Dictionary<Block, string> NameLookup = new Dictionary<Block, string>();

    private static readonly BlockCollection Collection
      = new AddableBlockCollection()
        {
          (a = new AddableBlockCollection()
               {
                 (a1 = new AddableBlockCollection()
                       {
                         (a11 = new ParagraphBlock()),
                         (a12 = new ParagraphBlock()),
                         (a13 = new ParagraphBlock()),
                       })
               }),
          (b = new AddableBlockCollection()
               {
                 (b1 = new AddableBlockCollection()
                       {
                         (b11 = new AddableBlockCollection()
                                {
                                  (b111 = new ParagraphBlock())
                                }),
                         (b12 = new ParagraphBlock()),
                       })
               }),
          (c = new ParagraphBlock()),
        };

    static BlockTreeWalkerTests()
    {
      try
      {
        Collection.RemoveFirstChilds();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    /// <summary> Gets the name of the given block. </summary>
    public static string GetNameOf(Block block)
    {
      return typeof(BlockTreeWalkerTests)
        .GetTypeInfo()
        .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(Block))
        .FirstOrDefault(f => f.GetValue(null) == block)
        ?.Name;
    }

    public static TheoryData<TestData> GetNextData()
    {
      return new TheoryData<TestData>()
             {
               CreateTestCase(() => a11, () => a12, "simple move (beginning)"),
               CreateTestCase(() => a12, () => a13, "simple move (end)"),
               CreateTestCase(() => a13, () => b111, "move to sub-level"),
               CreateTestCase(() => b111, () => b12, "move to parent level"),
               CreateTestCase(() => c, () => NullBlock, "end of document"),
             };
    }

    [Theory]
    [MemberData(nameof(GetNextData))]
    public void GetNextContainerBlock_Works(TestData data)
    {
      var nextBlock = BlockTreeWalker.GetNextNonContainerBlock(data.GetCurrent(this));

      DidYouKnow.That(GetNameOf(nextBlock)).Should().Be(GetNameOf(data.GetExpected(this)));
      DidYouKnow.That(nextBlock).Should().Be(data.GetExpected(this));
    }

    public static TheoryData<TestData> GetPreviousData()
    {
      return new TheoryData<TestData>()
             {
               CreateTestCase(() => a12, () => a11, "simple move (beginning)"),
               CreateTestCase(() => a13, () => a12, "simple move (end)"),
               CreateTestCase(() => b111, () => a13, "move to parent level"),
               CreateTestCase(() => b12, () => b111, "move to sub-level"),
               CreateTestCase(() => a11, () => NullBlock, "beginning of document"),
             };
    }

    [Theory]
    [MemberData(nameof(GetPreviousData))]
    public void GetPreviousContainerBlock_Works(TestData data)
    {
      var nextBlock = BlockTreeWalker.GetPreviousNonContainerBlock(data.GetCurrent(this));

      DidYouKnow.That(GetNameOf(nextBlock)).Should().Be(GetNameOf(data.GetExpected(this)));
      DidYouKnow.That(nextBlock).Should().Be(data.GetExpected(this));
    }

    /// <summary> Create a test case from the given information. </summary>
    private static TestData CreateTestCase(
      Expression<Func<Block>> currentBlock,
      Expression<Func<Block>> expectedBlock,
      string description
    )
    {
      var data = new TestData()
                 {
                   CurrentName = Utils.GetFieldInfo(currentBlock).Name,
                   ExpectedName = Utils.GetFieldInfo(expectedBlock).Name,
                   Description = description,
                 };

      return data;
    }

    /// <summary> Test data for a single test case </summary>
    public class TestData : SerializableTestData<TestData>
    {
      public Block GetCurrent(BlockTreeWalkerTests instance)
        => instance.GetFieldValue<Block>(CurrentName);

      public Block GetExpected(BlockTreeWalkerTests instance)
        => instance.GetFieldValue<Block>(ExpectedName);

      public string CurrentName;
      public string ExpectedName;
      public string Description;

      public override string ToString()
        => Description;
    }
  }
}