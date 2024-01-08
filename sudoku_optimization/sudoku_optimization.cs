namespace sudoku_optimization;

using System;
using System.Diagnostics;
using MathNet.Numerics.Statistics;

using sudoku_lib;

public static class Sleeper 
{
    /// <summary>
    /// Sleep for a fixed duration.
    /// </summary>
    public static void Sleep()
    {
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}

/// <summary>
/// Represents initial configuration of SA optimization process for sudoku solving.
/// </summary>
public class SudokuInitial
{
    /// <summary>
    /// Initial state of sudoku board with fixed values.
    /// </summary>
    public Board Board { get; set; }
    /// <summary>
    /// Seed for random numbers generator.
    /// </summary>
    public int Seed { get; set; }
    /// <summary>
    /// Random numbers generator used for creating new state of SA.
    /// </summary>
    public Random Hrng { get; set; }
    /// <summary>
    /// Max amount of mutations in generation.
    /// </summary>
    public int NMutationsPerGenerationLimit { get; set; }
    /// <summary>
    /// Max allowed number of resets.
    /// </summary>
    public int NResetsLimit { get; set; }
    /// <summary>
    /// Max number of generations created during SA process.
    /// </summary>
    public int NGenerationsLimit { get; set; }
    /// <summary>
    /// Coefficient for lowering SA temperature.
    /// </summary>
    public double TemperatureDecreaseFactor { get; set; }
    /// <summary>
    /// Coefficient for increasing SA temperature during reset.
    /// </summary>
    public double TemperatureIncreaseFactor { get; set; }

    /// <summary>
    /// Create a SudokuInitial object with the given board and seed.
    /// </summary>
    public SudokuInitial(Board board, int seed)
    {
        Board = new Board(board);
        Seed = seed;
        Hrng = new Random(seed);
        TemperatureDecreaseFactor = 0.001;
        TemperatureIncreaseFactor = 1.0; 
        NMutationsPerGenerationLimit = 2000;
        NResetsLimit = 1000;
        NGenerationsLimit = 1000000;
    }

    /// <summary>
    /// Create the initial generation for the simulated annealing algorithm.
    /// </summary>
    public SudokuGeneration InitialGeneration()
    {
        var person = new SudokuPerson(this);
        var temperature = InitialTemperature();
        var hrng = Hrng;
        var nResets = 0;
        var nGeneration = 0;
        return new SudokuGeneration(this, hrng, person, temperature, nResets, nGeneration);
    }

    /// <summary>
    /// Calculate the initial temperature for the simulated annealing algorithm.
    /// </summary>
    public double InitialTemperature()
    {
        var state = new SudokuPerson(this);
        const int N = 16;
        var costs = new double[N];
        
        for (int i = 0; i < N; i++)
        {
            var state2 = state.MutateRandom(this, Hrng);
            costs[i] = state2.Cost;
        }

        return Statistics.StandardDeviation(costs);
    }

    /// <summary>
    /// Solve the Sudoku puzzle using the Simulated Annealing algorithm.
    /// </summary>
    public (Reason, SudokuGeneration?) SolveWithSimulatedAnnealing()
    {
        var generation = InitialGeneration();

        while (true)
        {
            if (generation.NGeneration > NGenerationsLimit)
            {
                return (Reason.GenerationLimit, null);
            }

            Console.WriteLine($"\n= n_generation : {generation.NGeneration}\n");

            var (reason, generation2) = generation.Mutate(generation.Hrng);

            if (generation2 == null)
            {
                return (reason, null);
            }

            if (generation2.IsGoodEnough())
            {
                return (Reason.GoodEnough, generation2);
            }

            generation = generation2;
        }
    }
}

public static class BoardExtension
{
    /// <summary>
    /// Validate that each block has at least one non-fixed cell. Returns true if each block has at least one non-fixed cell, otherwise false.
    /// </summary>
    public static bool ValidateEachBlockHasNonFixedCell(this Board board)
    {
        
        foreach (var block in board.Blocks())
        {
            
            var fixedCount = board.BlockCells(block)
                .Select(cell => board.Cell(cell))
                .Aggregate(0, (acc, e) => e == 0 ? acc + 1 : acc);

            if (fixedCount == 0 || fixedCount >= 8)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Get a pair of random cells in a block that are not fixed.
    /// </summary>
    /// <returns>A tuple containing two random cells in the specified block.</returns>
    public static (CellIndex, CellIndex) CellsPairRandomInBlock( Board initial, BlockIndex block, Random hrng)
    {
        
        Debug.Assert(initial.ValidateEachBlockHasNonFixedCell());

        CellIndex cell1;
        do
        {
            cell1 = CellIndex.RandomInBlock(block, hrng);
            //Debug.WriteLine($"cell1 : {cell1}");
        } while (initial.Cell(cell1) != 0);

        CellIndex cell2;
        do
        {
            cell2 = CellIndex.RandomInBlock(block, hrng);
            //Debug.WriteLine($"cell2 : {cell2}");
        } while (cell1 == cell2 || initial.Cell(cell2) != 0);

        //Debug.WriteLine($"cell1 : {cell1}, cell2 : {cell2}");
        return (cell1, cell2);
    }
}

/// <summary>
/// Represents the reasons for the termination or proceeding with the Sudoku solving.
/// </summary>
public enum Reason
{
    GoodEnough,
    NotFinished,
    ResetLimit,
    GenerationLimit
}

/// <summary>
/// Represents single change(mutation) which contains indeces of two swapped cells. It is used to generate new state for sudoku solving process.
/// </summary>
public class SudokuMutagen
{
    public CellIndex Cell1 { get; private set; }
    public CellIndex Cell2 { get; private set; }

    public SudokuMutagen((CellIndex, CellIndex) cells)
    {
        Cell1 = cells.Item1;
        Cell2 = cells.Item2;
    }
}

/// <summary>
/// Represents intermediate state of sudoku board filled with random digits and the number of the errors of the board as the cost.
/// </summary>
public class SudokuPerson
{
    public Board Board { get; private set; }
    public int Cost { get; private set; }

    /// <summary>
    /// Create new state using initial sudoku board, fill it randomly with digits and calculate cost.
    /// </summary>
    public SudokuPerson(SudokuInitial initial)
    {
        Board = new Board(initial.Board);
        Board.FillMissingRandomly(initial.Hrng);
        Cost = Board.TotalError();
    }

    /// <summary>
    /// Construct new item of SudokuPerson from given board value and cost.
    /// </summary>
    public SudokuPerson(Board board, int cost)
    {
        Board = new Board(board);
        Cost = cost;
    }

    /// <summary>
    /// Copy constructor for SudokuPerson.
    /// </summary>
    public SudokuPerson(SudokuPerson other)
    {
        Board = new Board(other.Board);
        Cost = other.Cost;
    }

    /// <summary>
    /// Create new SudokuPerson by applying provided mutagen to current SudokuPerson.
    /// </summary>
    public SudokuPerson Mutate(SudokuInitial initial, SudokuMutagen mutagen)
    {
        SudokuPerson mutatedPerson = new SudokuPerson(this);
        Console.WriteLine($"cells_swap({mutagen.Cell1}, {mutagen.Cell2})");
        mutatedPerson.Board.CellsSwap(mutagen.Cell1, mutagen.Cell2);

        mutatedPerson.Cost -= this.Board.CrossError(mutagen.Cell1);
        mutatedPerson.Cost -= this.Board.CrossError(mutagen.Cell2);
        mutatedPerson.Cost += mutatedPerson.Board.CrossError(mutagen.Cell1);
        mutatedPerson.Cost += mutatedPerson.Board.CrossError(mutagen.Cell2);

        return mutatedPerson;
    }

    /// <summary>
    /// Create new SudokuPerson by applying random mutation to current SudokuPerson.
    /// </summary>
    public SudokuPerson MutateRandom(SudokuInitial initial, Random hrng)
    {
        SudokuMutagen mutagen = Mutagen(initial, hrng);
        return Mutate(initial, mutagen);
    }

    /// <summary>
    /// Create new SudokuMutagen as random cells pair in random sudoku block.
    /// </summary>
    public SudokuMutagen Mutagen(SudokuInitial initial, Random hrng)
    {
        BlockIndex block = BlockIndex.From(((byte)hrng.Next(3), (byte)hrng.Next(3)));
        var mutagen = new SudokuMutagen(BoardExtension.CellsPairRandomInBlock(initial.Board, block, hrng));
        return mutagen;
    }
}

/// <summary>
/// Represents a state in the Simulated Annealing optimization process for solving Sudoku.
/// </summary>
public class SudokuGeneration
{
    /// <summary>
    /// Initial configuration for the Sudoku puzzle.
    /// </summary>
    public SudokuInitial Initial { get; private set; }
    /// <summary>
    /// Random number generator for generating new state.
    /// </summary>
    public Random Hrng { get; private set; }
    /// <summary>
    /// Current state of sudoku board.
    /// </summary>
    public SudokuPerson Person { get; private set; }
    /// <summary>
    /// Current temperature in the optimization process.
    /// </summary>
    public double Temperature { get; private set; }
    /// <summary>
    /// Number of resets performed.
    /// </summary>
    public int NResets { get; private set; }
    /// <summary>
    /// Amount of generations before current genetration.
    /// </summary>
    public int NGeneration { get; private set; }

    /// <summary>
    /// Constructor for SudokuGeneration.
    /// </summary>
    public SudokuGeneration(
        SudokuInitial initial,
        Random hrng,
        SudokuPerson person,
        double temperature,
        int nResets,
        int nGeneration)
    {
        Initial = initial;
        Hrng = hrng;
        Person = person;
        Temperature = temperature;
        NResets = nResets;
        NGeneration = nGeneration;
    }

    /// <summary>
    /// Performs single iteration of optimization process, creates new vital SudokuGeneration with updated values.
    /// </summary>
    /// <returns>A tuple containing the reason to stop or continue optimization process and the new Sudoku generation if successful.</returns>
    public (Reason, SudokuGeneration?) Mutate(Random hrng)
    {
        var initial = this.Initial;
        var temperature = this.Temperature;
        var nMutations = 0;
        var nResets = this.NResets;

        SudokuPerson person;

        do
        {
            if (nMutations > initial.NMutationsPerGenerationLimit)
            {
                nResets++;
                if (nResets >= initial.NResetsLimit)
                {
                    return (Reason.ResetLimit, null);
                }

                var temperature2 = temperature + initial.TemperatureIncreaseFactor;
                Console.WriteLine($" 🔄 reset temperature {temperature} -> {temperature2}");
                Sleeper.Sleep();
                temperature = temperature2;
                nMutations = 0;
            }

            var mutagen = this.Person.Mutagen(initial, hrng);
            person = this.Person.Mutate(initial, mutagen);

            var costDifference = 0.5 + person.Cost - this.Person.Cost;
            var threshold = Math.Exp(-costDifference / temperature);

            Console.WriteLine($"cost : {this.Person.Cost} -> {person.Cost} | costDifference : {costDifference} | temperature : {temperature}");
            var rand = hrng.NextDouble();
            var vital = rand < threshold;

            if (vital)
            {
                var emoji = "🔘";
                if (costDifference > 0.0)
                {
                emoji = "🔼";
                }
                else if (costDifference < 0.0)
                {
                emoji = "✔️";
                }

                Console.WriteLine( $" {emoji} vital | rand( {rand} ) < threshold( {threshold} )" );
            }
            else
            {
                Console.WriteLine( $" ❌ non-vital | rand( {rand} ) > threshold( {threshold} )" );
            }

            if (vital)
            {
                break;
            }

            nMutations++;

        } while (true);

        temperature = temperature * (1.0 - initial.TemperatureDecreaseFactor);
        var nGeneration = this.NGeneration + 1;

        var generation = new SudokuGeneration(initial, hrng, person, temperature, nResets, nGeneration);

        return (Reason.NotFinished, generation);
    }
    /// <summary>
    /// Checks if the current state is considered good enough as a solution.
    /// </summary>
    /// <returns>True if the state is good enough, otherwise false.</returns>
    public bool IsGoodEnough()
    {
        return Person.Cost == 0;
    }
}
