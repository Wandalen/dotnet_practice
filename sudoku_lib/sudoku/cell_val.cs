namespace sudoku_lib;
using System;

/// <summary>
/// Represents the value of a cell in Sudoku. It can have a value from 1 to 9 or 0 if the cell is not assigned.
/// </summary>
public struct CellVal : IComparable<CellVal>
{
    /// <summary>
    /// Gets the underlying value of the CellVal.
    /// </summary>
    public byte Value { get;set; }

    /// <summary>
    /// Initializes a new instance of the CellVal struct with the specified value.
    /// </summary>
    public CellVal(byte value)
    {
        if (value > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 9.");
        }

        Value = value;
    }

    /// <summary>
    /// Converts a CellVal to an integer.
    /// </summary>
    /// <returns>An integer representing the CellVal.</returns>
    public static implicit operator int(CellVal cellVal) => cellVal.Value;

    /// <summary>
    /// Converts an integer to a CellVal.
    /// </summary>
    public static implicit operator CellVal(int value) => new CellVal((byte)value);

    // Other conversion operators and methods remain the same...

    /// <summary>
    /// Compares this CellVal to another CellVal for equality.
    /// </summary>
    public bool Equals(CellVal other) => Value == other.Value;

    /// <summary>
    /// Returns the hash code for this CellVal.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Compares this CellVal to another object for equality.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is CellVal other && CompareTo(other) == 0;
    }

    /// <summary>
    /// Compare two instances.
    /// </summary>
    public int CompareTo(CellVal other)
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
