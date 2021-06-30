using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Andy.Functional;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class BoxBingoTest
    {
        [TestMethod]
        public void TestBingo_true()
        {
            // 4 x 4
            Assert.IsTrue(Bingo(4, 3, 2, 1));
            Assert.IsTrue(Bingo(1, 5, 9, 13));
            Assert.IsTrue(Bingo(1, 6, 11, 16));
            Assert.IsTrue(Bingo(4, 7, 10, 13));

            // 5 x 5
            Assert.IsTrue(Bingo(6, 7, 8, 9, 10));
            Assert.IsTrue(Bingo(25, 10, 15, 20, 5));
            Assert.IsTrue(Bingo(1, 7, 13, 19, 25));
            Assert.IsTrue(Bingo(3, 8, 13, 18, 23));
        }

        [TestMethod]
        public void TestBingo_false()
        {
            Assert.IsFalse(Bingo(1, 2, 3, 8));
            Assert.IsFalse(Bingo(1, 3, 5, 7));
            Assert.IsFalse(Bingo(6, 7, 8, 9));
            Assert.IsFalse(Bingo(2, 5, 10, 15));
        }

        internal bool Bingo(params int[] positions)
        {
            Func<int, bool> IsTop(int size) => (loc) => loc <= size;
            Func<int, bool> IsLeft(int size) => (loc) => loc % size == 1;
            Func<int, bool> IsRight(int size) => (loc) => loc % size == 0;
            Func<int, bool> IsBottom(int size) => (loc) => loc > size * (size - 1);
            Func<IEnumerable<int>, IEnumerable<int>> GetGaps = ary => ary.Zip(ary.Skip(1), (x, y) => y - x);
            Func<IEnumerable<int>, bool> GapsIsSame = ary => !ary.Any(x => x != ary.First());

            return F.Boxing(positions)
                        .Map(ary => ary.OrderBy(x => x).ToArray())
                        .Where(ary => IsTop(ary.Length)(ary[0]) || IsLeft(ary.Length)(ary[0]))
                        .Where(ary => IsBottom(ary.Length)(ary.Last()) || IsRight(ary.Length)(ary.Last()))                      
                        .Map(GetGaps)
                        .Map(GapsIsSame)
                        .GetValue();
        }
    }
}
