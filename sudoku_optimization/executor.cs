namespace sudoku_optimization;
using sudoku_lib;
using OxyPlot;
using OxyPlot.Series;
using System.Diagnostics;
using System.Text;
using CommandLine;

public class Options
{
    [Option('n', "no-plots", Required = false, HelpText = "Execute without plotting")]
    public bool NoPlots { get; set; }

}

/// <summary>
/// Executes Simmulated annealing for sudoku solving.
/// </summary>
public class SudokuExecutor
{
    public static void Exec(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Board board = Board.Default();
        Console.WriteLine( board.ToString() );

        // Set the seed
        int seed = 10;

        // Create SudokuInitial instance
        SudokuInitial initial = new SudokuInitial(Board.Default(), seed);
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                if (options.NoPlots)
                {
                    (Reason reason, SudokuGeneration? generation) = initial.SolveWithSimulatedAnnealing();
                    PrintInfo(reason, generation);
                }
                else
                {
                    (Reason reason, SudokuGeneration? generation, Stats stats) = initial.SolveWithSAData();
                    Plotter.Draw(stats);
                    PrintInfo(reason, generation);
                }
            }
        );
    }

    static void PrintInfo(Reason reason, SudokuGeneration? generation)
    {

        Console.WriteLine(reason);

        if (generation != null)
        {
            Console.WriteLine(generation.Person.Board);
        }
        else
        {
            Console.WriteLine("Sudoku generation is null.");
        }
    }
}
