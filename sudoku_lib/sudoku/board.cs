namespace sudoku_lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a Sudoku board.
    /// </summary>
    public class Board : IComparable<Board>
    {
        private List<CellVal> storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class with the provided storage.
        /// </summary>
        public Board(List<CellVal> storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// Copy constructor for creating a new instance by copying from another Board.
        /// </summary>
        public Board(Board other)
        {
            // Assuming CellVal has a Clone method
            this.storage = other.storage.Select(cell => new CellVal(cell.Value)).ToList();
        }

        /// <summary>
        /// Gets the value of a cell at the specified flat index.
        /// </summary>
        /// <param name="index">The flat index of the cell.</param>
        public CellVal Cell(CellFlatIndex index)
        {
            int cellIndex = (int)index;
            return storage[cellIndex];
        }

        /// <summary>
        /// Returns an enumerable collection of cells along with their indices.
        /// </summary>
        public IEnumerable<(CellIndex, CellVal)> Cells()
        {
            return storage.Select((value, index) => (new CellIndex((byte)(index % 9), (byte)(index / 9)), value));
        }

        /// <summary>
        /// Returns an enumerable collection of cell values for the specified row index.
        /// </summary>
        /// <param name="index">The row index.</param>
        public IEnumerable<CellVal> Row(int index)
        {
            return storage.Skip(index * 9).Take(9);
        }

        /// <summary>
        /// Returns an enumerable collection of enumerable collections of cell values representing each row.
        /// </summary>
        public IEnumerable<IEnumerable<CellVal>> Rows()
        {
            return Enumerable.Range(0, 9).Select(i => Row(i));
        }

        /// <summary>
        /// Returns an enumerable collection of cell values for the specified column index.
        /// </summary>
        public IEnumerable<CellVal> Col(int index)
        {
            return storage.Where((_, i) => i % 9 == index).Select(e => e);
        }
        // public IEnumerable<CellVal> Col(int index)
        // {
        //     return storage.Skip(index).Where((_, i) => i % 9 == index);
        // }

        /// <summary>
        /// Returns an enumerable collection of enumerable collections of cell values representing each column.
        /// </summary>
        public IEnumerable<IEnumerable<CellVal>> Cols()
        {
            return Enumerable.Range(0, 9).Select(i => Col(i));
        }

        /// <summary>
        /// Returns an enumerable collection of cell values for the specified block.
        /// </summary>
        public IEnumerable<CellVal> Block(BlockIndex index)
        {
            int i = 0;
            int offset = (int)index.FirstCell();
            var result = storage.Skip(offset).Take(3);
            i++;
            result = result.Concat(storage.Skip(offset + i * 9).Take(3));
            i++;
            result = result.Concat(storage.Skip(offset + i * 9).Take(3));
            return result;
        }

        /// <summary>
        /// Returns an enumerable collection of block indices.
        /// </summary>
        public IEnumerable<BlockIndex> Blocks()
        {
            return Enumerable.Range(0, 9).Select(i => new BlockIndex((byte)(i % 3), (byte)(i / 3)));
        }

        /// <summary>
        /// Selects and returns an enumerable collection of cell values based on the provided flat indices.
        /// </summary>
        public IEnumerable<CellVal> Select(IEnumerable<CellFlatIndex> indices)
        {
            return indices.Select(index => storage[(int)index]);
        }

        /// <summary>
        /// Selects and returns an enumerable collection of cell values based on the provided flat indices while allowing mutations.
        /// </summary>
        public IEnumerable<CellVal> SelectMut(IEnumerable<CellFlatIndex> indices)
        {
            var storagePtr = storage.ToArray();
            //return indices.Select(index => storagePtr[(int)index]);


            foreach (var index in indices)
            {
                int arrayIndex = (int)index;
                if (arrayIndex >= 0 && arrayIndex < storage.Count)
                {
                    // Yield a mutable reference to the selected element
                    yield return storage[arrayIndex];
                }
                else
                {
                    // Handle index out of bounds or other conditions as needed
                    Console.WriteLine($"Index {arrayIndex} is out of bounds.");
                }
            }
        }

        public void SelectAndMut(IEnumerable<CellFlatIndex> indices, Func<CellVal, CellVal> mutationAction)
        {
            foreach (var index in indices)
            {
                int arrayIndex = (int)index;
                if (arrayIndex >= 0 && arrayIndex < storage.Count)
                {
                    var selectedCell = storage[arrayIndex];
                    storage[arrayIndex] = mutationAction(selectedCell); // Call the provided mutation action
                    //storage[arrayIndex] = selectedCell; // Update the original 'storage' list
                }
            }
        }

        /// <summary>
        /// Returns an enumerable collection of flat cell indices for the specified block.
        /// </summary>
        public IEnumerable<CellFlatIndex> BlockCells(BlockIndex index)
        {
            int[] indices = new int[9];
            int i1 = 0;
            int i2 = (int)index.FirstCell();
            for (int _a = 0; _a < 3; _a++)
            {
                for (int _b = 0; _b < 3; _b++)
                {
                    indices[i1] = i2;
                    i1++;
                    i2++;
                }
                i2 += 9 - 3;
            }

            return indices.Select(i => (CellFlatIndex)i);
        }

        /// <summary>
        /// Determines the missing cell values in the specified block.
        /// </summary>
        public HashSet<CellVal> BlockMissingVals(BlockIndex index)
        {
            HashSet<CellVal> digits = new HashSet<CellVal>(Enumerable.Range(1, 9).Select(e => (CellVal)e));

            var block = Block(index);
            HashSet<CellVal> has = new HashSet<CellVal>(block.Where(e => e != 0));
            return new HashSet<CellVal>(digits.Except(has));
        }

        /// <summary>
        /// Fills the missing cell values in the board randomly using the provided random number generator.
        /// </summary>
        public Board FillMissingRandomly(Random rng)
        {
            for (int i = 0; i < 9; i++)
            {
                BlockIndex block = Blocks().ElementAt(i);
                var missingVals = BlockMissingVals(block).ToList();
                missingVals = missingVals.OrderBy(_ => rng.Next()).ToList();
                var cells = BlockCells(block);
                int missingValIndex = 0;
                foreach (CellFlatIndex cellIndex in cells)
                {
                    CellVal cellVal = storage[(int)cellIndex];
                    if (cellVal == 0)
                    {
                        storage[(int)cellIndex] = missingVals[missingValIndex];
                        missingValIndex++;
                    }
                }
            }
            return this;
        }

        // Other methods and properties (commented for brevity)

        /// <summary>
        /// Calculates the cross error for a specified cell index.
        /// Its Number of not unique cells in row and col of the cell.
        /// </summary>
        public int CrossError(CellIndex index)
        {
            int error = 0;
            error += 9 - Col(index.Col).Where(e => e != 0).Distinct().Count();
            error += 9 - Row(index.Row).Where(e => e != 0).Distinct().Count();
            return error;
        }

        /// <summary>
        /// Calculates the total error for the entire board.
        /// Its Number of not unique cells in each row and col.
        /// </summary>
        public int TotalError()
        {
            int error = 0;
            for (int i = 0; i < 9; i++)
            {
                error += CrossError(new CellIndex((byte)i, (byte)i));
            }
            return error;
        }

        /// <summary>
        /// Swaps the values of two cells on the board.
        /// </summary>
        public void CellsSwap(CellIndex index1, CellIndex index2)
        {
            int temp = storage[(int)index1];
            storage[(int)index1] = storage[(int)index2];
            storage[(int)index2] = temp;
        }

        /// <summary>
        /// Creates a default Sudoku board.
        /// </summary>
        public static Board Default()
        {
            List<CellVal> storage = new List<CellVal>
            {
                3, 1, 0, 0, 0, 0, 0, 2, 0,
                0, 0, 6, 1, 0, 9, 0, 0, 5,
                0, 0, 0, 0, 8, 0, 0, 0, 0,
                0, 2, 0, 8, 0, 4, 0, 5, 0,
                0, 0, 4, 0, 7, 0, 0, 0, 0,
                0, 0, 0, 0, 6, 0, 0, 0, 8,
                0, 6, 0, 0, 0, 0, 9, 0, 0,
                0, 0, 9, 4, 0, 5, 0, 0, 1,
                0, 0, 0, 0, 0, 7, 0, 0, 0,
            }.Select(e => (CellVal)e).ToList();
            return new Board(storage);
        }

        /// <summary>
        /// Creates a Sudoku board from a string representation.
        /// </summary>
        public static Board From(string src)
        {
            string trimmedSrc = src.Trim();
            List<CellVal> storage = trimmedSrc
                .Split('\n')
                .SelectMany(line => line.Where(char.IsDigit))
                .Select(ch => (CellVal)(ch - '0'))
                .ToList();
            return new Board(storage);
        }

        /// <summary>
        /// Converts the board to its string representation.
        /// </summary>
        public override string ToString()
        {
            return string.Join('\n', Rows().Select(row => string.Join(" ", row.Select(cell => cell.ToString())))) + "\n";
        }

        /// <summary>
        /// Returns the hash code for this Board.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17; // Prime number
                foreach (var cell in storage)
                {
                    hash = hash * 23 + cell.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Compare two instances.
        /// </summary>
        public int CompareTo(Board? other)
        {
            if (other == null)
            {
                return 1; // Consider the current instance greater than null
            }

            // Compare the two storage lists element by element
            for (int i = 0; i < storage.Count; i++)
            {
                int result = storage[i].CompareTo(other.storage[i]);
                if (result != 0)
                {
                    return result;
                }
            }

            // If all elements are equal, the boards are equal
            return 0;
        }

        /// <summary>
        /// Compares this Board to another object for equality.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Board other && CompareTo(other) == 0;
        }

    }
}
