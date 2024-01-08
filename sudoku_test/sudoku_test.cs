namespace sudoku_test;

using sudoku_optimization;
using sudoku_lib;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {

    }
}

[TestClass]
public class BoardTests
{

    [TestMethod]
    public void FromString()
    {
        string src = @"
310 000 020
006 109 005
000 080 000

020 804 050
004 070 000
000 060 008

060 000 900
009 405 001
000 007 000
";

        Board got = Board.From(src);
        List<CellVal> storage = new List<CellVal>
        {
            3,1,0, 0,0,0, 0,2,0,
            0,0,6, 1,0,9, 0,0,5,
            0,0,0, 0,8,0, 0,0,0,
            0,2,0, 8,0,4, 0,5,0,
            0,0,4, 0,7,0, 0,0,0,
            0,0,0, 0,6,0, 0,0,8,
            0,6,0, 0,0,0, 9,0,0,
            0,0,9, 4,0,5, 0,0,1,
            0,0,0, 0,0,7, 0,0,0,
        };

        Board exp = new Board(storage);
        Assert.AreEqual(got, exp);
    }

    [TestMethod]
    public void Cell()
    {
        Board board = Board.Default();
        var cells = board.Cells().GetEnumerator();

        Assert.IsTrue(cells.MoveNext());
        Assert.AreEqual((new CellIndex(0, 0), (CellVal)3), cells.Current);

        Assert.IsTrue(cells.MoveNext());
        Assert.AreEqual((new CellIndex(1, 0), (CellVal)1), cells.Current);

        Assert.IsTrue(cells.MoveNext());
        Assert.AreEqual((new CellIndex(2, 0), (CellVal)0), cells.Current);

        for (int i = 0; i < 9; i++)
        {
            cells.MoveNext();
        }

        Assert.IsTrue(cells.MoveNext());
        Console.WriteLine(cells.Current);
        Assert.AreEqual((new CellIndex(3, 1), (CellVal)1), cells.Current);

        Assert.IsTrue(cells.MoveNext());
        Assert.AreEqual((new CellIndex(4, 1), (CellVal)0), cells.Current);

        Assert.IsTrue(cells.MoveNext());
        Assert.AreEqual((new CellIndex(5, 1), (CellVal)9), cells.Current);
    }

    [TestMethod]
    public void Col()
    {
        Board board = Board.Default();

        List<CellVal> exp = new List<CellVal> { 3, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<CellVal> got = board.Col(0).ToList();
        CollectionAssert.AreEqual(exp, got);

        got = board.Col(1).ToList();
        exp = new List<CellVal> { 1, 0, 0, 2, 0, 0, 6, 0, 0 };
        Console.WriteLine("got : " + string.Join(", ", got));
        Console.WriteLine("exp : " + string.Join(", ", exp));
        CollectionAssert.AreEqual(got, exp);

        got = board.Col(8).ToList();
        exp = new List<CellVal> { 0, 5, 0, 0, 0, 8, 0, 1, 0 };
        Console.WriteLine("got : " + string.Join(", ", got));
        Console.WriteLine("exp : " + string.Join(", ", exp));
        CollectionAssert.AreEqual(got, exp);

        Assert.AreEqual(9, board.Cols().Count());
    }

    [TestMethod]
    public void Row()
    {
        Board board = Board.Default();

        List<CellVal> exp = new List<CellVal> { 3, 1, 0, 0, 0, 0, 0, 2, 0 };
        List<CellVal> got = board.Row(0).ToList();
        CollectionAssert.AreEqual(exp, got);

        exp = new List<CellVal> { 0, 0, 0, 0, 0, 7, 0, 0, 0 };
        got = board.Row(8).ToList();
        CollectionAssert.AreEqual(exp, got);

        Assert.AreEqual(9, board.Rows().Count());
    }

    [TestMethod]
    public void Block()
    {
        Board board = Board.Default();

        List<CellVal> got = board.Block(new BlockIndex(0, 0)).ToList();
        List<CellVal> exp = new List<CellVal> { 3, 1, 0, 0, 0, 6, 0, 0, 0 };
        CollectionAssert.AreEqual(exp, got);

        got = board.Block(new BlockIndex(1, 0)).ToList();
        exp = new List<CellVal> { 0, 0, 0, 1, 0, 9, 0, 8, 0 };
        CollectionAssert.AreEqual(exp, got);

        got = board.Block(new BlockIndex(2, 2)).ToList();
        exp = new List<CellVal> { 9, 0, 0, 0, 0, 1, 0, 0, 0 };
        CollectionAssert.AreEqual(exp, got);

        Assert.AreEqual(9, board.Blocks().Count());
    }

    [TestMethod]
    public void Select()
    {
        Board board = Board.Default();

        var indices = board.BlockCells(new BlockIndex(0,0));
        var got = board.Select(indices).ToList();
        List<CellVal> exp = new List<CellVal>  { 3, 1, 0, 0, 0, 6, 0, 0, 0 };

        CollectionAssert.AreEqual(got, exp);

        indices = board.BlockCells(new BlockIndex(1, 0));
        got = board.Select(indices).ToList();
        exp = new List<CellVal> { 0, 0, 0, 1, 0, 9, 0, 8, 0 };

        CollectionAssert.AreEqual(got, exp);

        indices = board.BlockCells(new BlockIndex(2, 2));
        got = board.Select(indices).ToList();
        exp = new List<CellVal>{ 9, 0, 0, 0, 0, 1, 0, 0, 0 };

        CollectionAssert.AreEqual(got, exp);

    }


    [TestMethod]
    public void SelectAndMut()
    {
        Board board = Board.Default();

        var indices = board.BlockCells(new BlockIndex(0,0));

        board.SelectAndMut(indices, cell =>
        {
            return cell.Value += 1;
        });
        var got = board.Select(indices).ToList();
        
        List<CellVal> exp = new List<CellVal>  { 4, 2, 1, 1, 1, 7, 1, 1, 1 };
        
        CollectionAssert.AreEqual(got, exp);

    }

    [TestMethod]
    public void CrossError()
    {
        Board board = Board.Default();

        var exp = 14;
        var got = board.CrossError( new CellIndex( 0, 0 ) );

        Assert.AreEqual(got, exp);

    }

    [TestMethod]
    public void TotalError()
    {
        Board board = Board.Default();

        var exp = 116;
        var got = board.TotalError();

        Assert.AreEqual(got, exp);

    }

    [TestMethod]
    public void CellSwap()
    {
        var storage = new List<CellVal>
        ([
            0,1,0, 0,0,0, 0,2,0,
            0,0,6, 1,0,9, 0,0,5,
            0,0,0, 0,8,0, 0,0,0,
            0,2,0, 8,0,4, 0,5,0,
            0,0,4, 0,7,0, 0,0,0,
            0,0,0, 0,6,0, 0,0,8,
            0,6,0, 0,0,0, 9,0,0,
            0,0,9, 4,0,5, 0,0,1,
            0,0,0, 0,0,7, 0,0,3,
        ]);

        var exp = new Board(storage);
        var got = Board.Default();
        got.CellsSwap(new CellIndex(0, 0), new CellIndex(8, 8));

        Assert.AreEqual(got.CompareTo(exp), 0);

        var storage1 = new List<CellVal>
        ([
            3,1,0, 0,0,0, 0,2,0,
            0,0,6, 1,0,9, 0,0,2,
            0,0,0, 0,8,0, 0,0,0,
            0,5,0, 8,0,4, 0,5,0,
            0,0,4, 0,7,0, 0,0,0,
            0,0,0, 0,6,0, 0,0,8,
            0,6,0, 0,0,0, 9,0,0,
            0,0,9, 4,0,5, 0,0,1,
            0,0,0, 0,0,7, 0,0,0,
        ]);

        var exp1 = new Board(storage1);
        var got1 = Board.Default();
        got1.CellsSwap(new CellIndex( 1, 3 ), new CellIndex( 8, 1 ));

        Assert.AreEqual(got1.CompareTo(exp1), 0);

    }

    [TestMethod]
    public void BlockMissingVals()
    {
        Board board = Board.Default();

        var got = board.BlockMissingVals( new BlockIndex(0,0));
        var exp = new HashSet<CellVal> {
            new(2),
            new(4),
            new(5),
            new(7),
            new(8),
            new(9)
        };

        Assert.AreEqual(exp.SetEquals(got), true);

    }

    [TestMethod]
    public void FillMissingRandomly()
    {
        Board board = Board.Default();
        var hrng = new Random(10);

        board.FillMissingRandomly(hrng);

        foreach (var cell in board.Cells())
        {
            // Assert that the cell value is not 0
            // Assuming CellVal has an implicit conversion from int
            Assert.IsTrue(cell.Item2 != 0);
        }

        foreach (var block in board.Blocks())
        {
            var missing = board.BlockMissingVals(block);
            Assert.IsTrue(missing.Count == 0);
        }

        Console.WriteLine($"{board} with hash {board.GetHashCode()}");
        Console.WriteLine($"total_error : {board.TotalError()}");

        // Create another board with the same seed
        var hrng2 = new Random(10);
        var board2 = Board.Default();
        Console.WriteLine($"{board2}");
        board2.FillMissingRandomly(hrng2);
        Console.WriteLine($"{board2} with hash {board2.GetHashCode()}");
        Assert.AreEqual(board.CompareTo(board2), 0);

        // Assert that the hash codes are equal
        Assert.AreEqual(board.GetHashCode(), board2.GetHashCode());

    }
}

