namespace sudoku_lib;

using System;
using System.Diagnostics;


/// <summary>
/// Represents a flat index of a Sudoku cell.
/// </summary>
[DebuggerDisplay("{Value}")]
public struct CellFlatIndex : IComparable<CellFlatIndex>
{
    public int Value { get; }

    /// <summary>
    /// Initializes a new instance of the CellFlatIndex struct with the specified value.
    /// </summary>
    public CellFlatIndex(int value)
    {
        Debug.Assert(value < 81);
        Value = value;
    }

    /// <summary>
    /// Unwraps and returns the underlying value of the CellFlatIndex.
    /// </summary>
    public int Unwrap()
    {
        return Value;
    }

    /// <summary>
    /// Implicitly converts an integer to a CellFlatIndex.
    /// </summary>
    public static implicit operator CellFlatIndex(int src)
    {
        return new CellFlatIndex(src);
    }

    /// <summary>
    /// Implicitly converts a CellFlatIndex to an integer.
    /// </summary>
    public static implicit operator int(CellFlatIndex src)
    {
        return src.Value;
    }

    /// <summary>
    /// Implicitly converts a CellIndex to a CellFlatIndex.
    /// </summary>
    public static implicit operator CellFlatIndex(CellIndex src)
    {
        return new CellFlatIndex(src.Col + src.Row * 9);
    }

    /// <summary>
    /// Returns the hash code for this CellFlatIndex.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Compares this CellFlatIndex to another object for equality.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is CellFlatIndex other && CompareTo(other) == 0;
    }

    /// <summary>
    /// Compare two instances.
    /// </summary>
    public int CompareTo(CellFlatIndex other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Converts the cell value to its string representation.
    /// </summary>
    public override string ToString()
    {
        return Value.ToString();
    }

}

/// <summary>
/// Represents the index of a Sudoku cell.
/// </summary>
[DebuggerDisplay("({Col}, {Row})")]
public struct CellIndex : IComparable<CellIndex>
{
    public byte Col { get; }
    public byte Row { get; }

    /// <summary>
    /// Initializes a new instance of the CellIndex struct with the specified column and row values.
    /// </summary>
    public CellIndex(byte col, byte row)
    {
        Debug.Assert(col <= 8);
        Debug.Assert(row <= 8);
        Col = col;
        Row = row;
    }

    /// <summary>
    /// Generates a random CellIndex within a specified Sudoku block using the given Random generator.
    /// </summary>
    public static CellIndex RandomInBlock(BlockIndex block, Random rng)
    {
        var intervals = block.CellsIntervals();

        byte col = (byte)(intervals.Item1.Start.Value + rng.Next(0, intervals.Item1.End.Value - intervals.Item1.Start.Value));
        byte row = (byte)(intervals.Item2.Start.Value + rng.Next(0, intervals.Item2.End.Value - intervals.Item2.Start.Value));

        return new CellIndex(col, row);
    }

    /// <summary>
    /// Implicitly converts a tuple of two bytes to a CellIndex.
    /// </summary>
    public static implicit operator CellIndex((byte, byte) src)
    {
        return new CellIndex(src.Item1, src.Item2);
    }

    /// <summary>
    /// Implicitly converts a CellIndex to a tuple of two bytes.
    /// </summary>
    public static implicit operator (byte, byte)(CellIndex src)
    {
        return (src.Col, src.Row);
    }

    /// <summary>
    /// Implicitly converts a CellFlatIndex to a CellIndex.
    /// </summary>
    public static implicit operator CellIndex(CellFlatIndex src)
    {
        return new CellIndex((byte)(src.Value % 9), (byte)(src.Value / 9));
    }

    /// <summary>
    /// Implicitly converts a CellIndex to an integer by using a CellFlatIndex.
    /// </summary>
    public static implicit operator int(CellIndex src)
    {
        var index = (CellFlatIndex)src;
        return index;
    }

    /// <summary>
    /// Returns a string representation of the CellIndex.
    /// </summary>
    public override string ToString()
    {
        return $"({Col}, {Row})";
    }

    /// <summary>
    /// Compare two instances.
    /// </summary>
    public int CompareTo(CellIndex other)
    {
        // Compare Row first, then Col
        int rowComparison = Row.CompareTo(other.Row);
        if (rowComparison != 0)
            return rowComparison;

        return Col.CompareTo(other.Col);
    }

    /// <summary>
    /// Compares this CellIndex to another object for equality.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is CellIndex other)
        {
            return Row == other.Row && Col == other.Col;
        }
        return false;
    }

    /// <summary>
    /// Returns the hash code for this CellIndex.
    /// </summary>
    public override int GetHashCode()
    {
        return (Row << 8) | Col; // Combine Row and Col into a single int for hashing
    }

}
