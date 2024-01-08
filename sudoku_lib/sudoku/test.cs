// // namespace project1;
// namespace ProgramTest;
//
// [TestClass]
// public class BoardTests
// {
//     [TestMethod]
//     public void FromString()
//     {
//         string src = @"
// 310 000 020
// 006 109 005
// 000 080 000
//
// 020 804 050
// 004 070 000
// 000 060 008
//
// 060 000 900
// 009 405 001
// 000 007 000
// ";
//
//         Board got = new Board(src);
//         List<CellVal> storage = new List<CellVal>
//         {
//             3,1,0, 0,0,0, 0,2,0,
//             0,0,6, 1,0,9, 0,0,5,
//             0,0,0, 0,8,0, 0,0,0,
//             0,2,0, 8,0,4, 0,5,0,
//             0,0,4, 0,7,0, 0,0,0,
//             0,0,0, 0,6,0, 0,0,8,
//             0,6,0, 0,0,0, 9,0,0,
//             0,0,9, 4,0,5, 0,0,1,
//             0,0,0, 0,0,7, 0,0,0,
//         };
//
//         Board exp = new Board(storage);
//
//         CollectionAssert.AreEqual(got.Storage, exp.Storage);
//     }
//
//     [TestMethod]
//     public void Cell()
//     {
//         Board board = Board.Default();
//         var cells = board.Cells().GetEnumerator();
//
//         Assert.IsTrue(cells.MoveNext());
//         Assert.AreEqual((new CellIndex(0, 0), new CellVal(3)), cells.Current);
//
//         Assert.IsTrue(cells.MoveNext());
//         Assert.AreEqual((new CellIndex(1, 0), new CellVal(1)), cells.Current);
//
//         Assert.IsTrue(cells.MoveNext());
//         Assert.AreEqual((new CellIndex(2, 0), new CellVal(0)), cells.Current);
//
//         // ...
//     }
//
//     [TestMethod]
//     public void Col()
//     {
//         Board board = Board.Default();
//
//         List<CellVal> exp = new List<CellVal> { 3, 0, 0, 0, 0, 0, 0, 0, 0 };
//         List<CellVal> got = board.Col(0).ToList();
//         CollectionAssert.AreEqual(exp, got);
//
//         exp = new List<CellVal> { 0, 5, 0, 0, 0, 8, 0, 1, 0 };
//         got = board.Col(8).ToList();
//         CollectionAssert.AreEqual(exp, got);
//
//         Assert.AreEqual(9, board.Cols().Count());
//     }
//
//     [TestMethod]
//     public void Row()
//     {
//         Board board = Board.Default();
//
//         List<CellVal> exp = new List<CellVal> { 3, 1, 0, 0, 0, 0, 0, 2, 0 };
//         List<CellVal> got = board.Row(0).ToList();
//         CollectionAssert.AreEqual(exp, got);
//
//         exp = new List<CellVal> { 0, 0, 0, 0, 0, 7, 0, 0, 0 };
//         got = board.Row(8).ToList();
//         CollectionAssert.AreEqual(exp, got);
//
//         Assert.AreEqual(9, board.Rows().Count());
//     }
//
//     [TestMethod]
//     public void Block()
//     {
//         Board board = Board.Default();
//
//         List<CellVal> got = board.Block(new BlockIndex(0, 0)).ToList();
//         List<CellVal> exp = new List<CellVal> { 3, 1, 0, 0, 0, 6, 0, 0, 0 };
//         CollectionAssert.AreEqual(exp, got);
//
//         got = board.Block(new BlockIndex(1, 0)).ToList();
//         exp = new List<CellVal> { 0, 0, 0, 1, 0, 9, 0, 8, 0 };
//         CollectionAssert.AreEqual(exp, got);
//
//         got = board.Block(new BlockIndex(2, 2)).ToList();
//         exp = new List<CellVal> { 9, 0, 0, 0, 0, 1, 0, 0, 0 };
//         CollectionAssert.AreEqual(exp, got);
//
//         Assert.AreEqual(9, board.Blocks().Count());
//     }
//
// }