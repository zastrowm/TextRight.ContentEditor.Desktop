using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.ObjectModel.Blocks
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

    public static IEnumerable<TestCaseData> GetNextData()
    {
      yield return CreateTestCase(() => a11, () => a12, "simple move (beginning)");
      yield return CreateTestCase(() => a12, () => a13, "simple move (end)");
      yield return CreateTestCase(() => a13, () => b111, "move to sub-level");
      yield return CreateTestCase(() => b111, () => b12, "move to parent level");
      yield return CreateTestCase(() => c, () => NullBlock, "end of document");
    }

    [Test]
    [TestCaseSource(nameof(GetNextData))]
    public void GetNextContainerBlock_Works(TestData data)
    {
      var nextBlock = BlockTreeWalker.GetNextNonContainerBlock(data.Current);

      Assert.That(GetNameOf(nextBlock), Is.EqualTo(GetNameOf(data.Expected)));
      Assert.That(nextBlock, Is.EqualTo(data.Expected));
    }

    public static IEnumerable<TestCaseData> GetPreviousData()
    {
      yield return CreateTestCase(() => a12, () => a11, "simple move (beginning)");
      yield return CreateTestCase(() => a13, () => a12, "simple move (end)");
      yield return CreateTestCase(() => b111, () => a13, "move to parent level");
      yield return CreateTestCase(() => b12, () => b111, "move to sub-level");
      yield return CreateTestCase(() => a11, () => NullBlock, "beginning of document");
    }

    [Test]
    [TestCaseSource(nameof(GetPreviousData))]
    public void GetPreviousContainerBlock_Works(TestData data)
    {
      var nextBlock = BlockTreeWalker.GetPreviousNonContainerBlock(data.Current);

      Assert.That(GetNameOf(nextBlock), Is.EqualTo(GetNameOf(data.Expected)));
      Assert.That(nextBlock, Is.EqualTo(data.Expected));
    }

    /// <summary> Create a test case from the given information. </summary>
    private static TestCaseData CreateTestCase(
      Expression<Func<Block>> currentBlock,
      Expression<Func<Block>> expectedBlock,
      string description
      )
    {
      var data = new TestData()
                 {
                   Current = currentBlock.Compile().Invoke(),
                   Expected = expectedBlock.Compile().Invoke(),
                   CurrentName = Utils.GetFieldInfo(currentBlock).Name,
                   ExpectedName = Utils.GetFieldInfo(expectedBlock).Name,
                   Description = description,
                 };

      return data.ToTestCase();
    }

    /// <summary> Test data for a single test case </summary>
    public struct TestData
    {
      public Block Current;
      public Block Expected;

      public string CurrentName;
      public string ExpectedName;

      public string Description;

      public TestCaseData ToTestCase()
        => new TestCaseData(this)
          .SetName($"{Description}: {CurrentName}->{ExpectedName}");
    }
  }
}