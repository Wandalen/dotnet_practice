namespace sudoku_lib;

using System;
using System.Diagnostics;

/// <summary>
/// Represents the index of a Sudoku block.
/// </summary>
[DebuggerDisplay("({Row}, {Col})")]
public struct BlockIndex : IComparable<BlockIndex>
{
    public byte Row { get; }
    public byte Col { get; }

    /// <summary>
    /// Initializes a new instance of the BlockIndex struct with the specified row and column values.
    /// </summary>
    public BlockIndex(byte row, byte col)
    {
        Debug.Assert(row <= 2);
        Debug.Assert(col <= 2);
        Row = row;
        Col = col;
    }

    /// <summary>
    /// Gets the flat index of the first cell in the block.
    /// </summary>
    public CellFlatIndex FirstCell()
    {
        int cellIndex = Row * 3 + Col * 27;
        return new CellFlatIndex((byte)cellIndex);
    }

    /// <summary>
    /// Returns the row and column index intervals of cells within the block.
    /// </summary>
    public (Range, Range) CellsIntervals()
    {
        Range rowRange = new Range(Row * 3, Row * 3 + 3);
        Range colRange = new Range(Col * 3, Col * 3 + 3);
        return (rowRange, colRange);
    }

    /// <summary>
    /// Converts a tuple of two bytes to a BlockIndex.
    /// </summary>
    public static BlockIndex From((byte, byte) src)
    {
        return new BlockIndex(src.Item1, src.Item2);
    }

    /// <summary>
    /// Converts a CellIndex to a BlockIndex.
    /// </summary>
    public static BlockIndex From(CellIndex src)
    {
        return new BlockIndex((byte)(src.Col / 3), (byte)(src.Row / 3));
    }

    /// <summary>
    /// Converts a CellFlatIndex to a BlockIndex.
    /// </summary>
    public static BlockIndex From(CellFlatIndex src)
    {
        CellIndex cellIndex = (CellIndex)src;
        return From(cellIndex);
    }

    /// <summary>
    /// Returns a string representation of the BlockIndex.
    /// </summary>
    public override string ToString()
    {
        return $"({Col}, {Row})";
    }

    /// <summary>
    /// Compare two instances.
    /// </summary>
    public int CompareTo(BlockIndex other)
    {
        // Compare Row first, then Col
        int rowComparison = Row.CompareTo(other.Row);
        if (rowComparison != 0)
            return rowComparison;

        return Col.CompareTo(other.Col);
    }

    /// <summary>
    /// Compares this BlockIndex to another object for equality.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is BlockIndex other)
        {
            return Row == other.Row && Col == other.Col;
        }
        return false;
    }

    /// <summary>
    /// Returns the hash code for this BlockIndex.
    /// </summary>
    public override int GetHashCode()
    {
        return (Row << 8) | Col; // Combine Row and Col into a single int for hashing
    }

}
