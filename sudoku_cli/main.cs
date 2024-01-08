namespace sudoku_cli;
using sudoku_lib;
using sudoku_optimization;

class Program
{
    static void Main(string[] args)
    {
        // TemperatureConverter.Exec( args );
        // Person.Exec( args );
        SudokuExecutor.Exec( args );
    }
}
