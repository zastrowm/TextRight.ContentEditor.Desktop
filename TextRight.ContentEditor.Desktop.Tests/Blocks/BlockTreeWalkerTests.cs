using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Desktop.Blocks;

namespace TextRight.ContentEditor.Desktop.Tests
{
  public class BlockTreeWalkerTests
  {
    static Block a, a1, a11, a12, a13, b, b1, b11, b12, b111, c;
    private static Block nullBlock = null;

    static BlockCollection collection = new BlockCollection()
    {
      (a = new BlockCollection()
      {
        (a1 = new BlockCollection()
        {
          (a11 = new TextBlock()),
          (a12 = new TextBlock()),
          (a13 = new TextBlock()),
        })
      }),
      (b = new BlockCollection()
      {
        (b1 = new BlockCollection()
        {
          (b11 = new BlockCollection()
          {
            (b111 = new TextBlock())
          }),
          (b12 = new TextBlock()),
        })
      }),
      (c = new TextBlock()),
    };

    public static IEnumerable<TestCaseData> GetNextData()
    {
      yield return CreateTestCase(() => a11, () => a12, "simple move (beginning)");
      yield return CreateTestCase(() => a12, () => a13, "simple move (end)");
      yield return CreateTestCase(() => a13, () => b111, "move to sub-level");
      yield return CreateTestCase(() => b111, () => b12, "move to parent level");
      yield return CreateTestCase(() => c, () => nullBlock, "end of document");
    }

    [Test]
    [TestCaseSource(nameof(GetNextData))]
    public void GetNextContainerBlock_Works(TestData data)
    {
      var nextBlock = BlockTreeWalker.GetNextNonContainerBlock(data.Current);
      Assert.That(nextBlock, Is.EqualTo(data.Expected));
    }

    public static IEnumerable<TestCaseData> GetPreviousData()
    {
      yield return CreateTestCase(() => a12, () => a11, "simple move (beginning)");
      yield return CreateTestCase(() => a13, () => a12, "simple move (end)");
      yield return CreateTestCase(() => b111, () => a13, "move to parent level");
      yield return CreateTestCase(() => b12, () => b111, "move to sub-level");
      yield return CreateTestCase(() => a11, () => nullBlock, "beginning of document");
    }

    [Test]
    [TestCaseSource(nameof(GetPreviousData))]
    public void GetPreviousContainerBlock_Works(TestData data)
    {
      var nextBlock = BlockTreeWalker.GetPreviousNonContainerBlock(data.Current);
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