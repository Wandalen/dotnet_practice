namespace sudoku_test;

using sudoku_optimization;
using sudoku_lib;
using System.Diagnostics;

[TestClass]
public class OptimizationTest
{

    [TestMethod]
    public void InitialTemperature()
    {
        var initial = new SudokuInitial( Board.Default(), default );
        var temperature = initial.InitialTemperature();

        Assert.IsTrue(temperature >= 0);
        Assert.AreEqual(temperature, 1,4375905768565218);
    }

    [TestMethod]
    public void PersonMutate()
    {
        var initial = new SudokuInitial( Board.Default(), default );
        var person = new SudokuPerson(initial);

        Assert.AreEqual( person.Cost, 46 );
        Assert.AreEqual(person.Cost, person.Board.TotalError());

        var mutagen = person.Mutagen( initial, initial.Hrng );
        Assert.AreEqual( BlockIndex.From( mutagen.Cell1 ), BlockIndex.From( mutagen.Cell2 ) );

        var person2 = person.Mutate( initial, mutagen );

        Assert.AreEqual( person2.Cost, 46 );
        Assert.AreEqual(person2.Cost, person2.Board.TotalError());

        var mutagen1 = person2.Mutagen( initial, initial.Hrng );
        Assert.AreEqual( BlockIndex.From( mutagen1.Cell1 ), BlockIndex.From( mutagen1.Cell2 ) );

        var person3 = person2.Mutate( initial, mutagen1 );

        Assert.AreEqual( person3.Cost, 47 );
        Assert.AreEqual( person3.Cost, person3.Board.TotalError() );
    }

    //[TestMethod]
    public void SolveWithSA()
    {

        // Set the seed
        int seed = 10;

        // Create SudokuInitial instance
        SudokuInitial initial = new SudokuInitial(Board.Default(), seed);
        
        (Reason reason, SudokuGeneration? generation) = initial.SolveWithSimulatedAnnealing();

        Console.WriteLine(reason);
        // Assert that generation is not null
        Assert.IsNotNull(generation);

        Assert.AreEqual(generation.Person.Cost, 0);

    }

    // [TestMethod]
    public void TimeMeasure()
    {
        //time dotnet test --filter FullyQualifiedName~sudoku_test.OptimizationTest.TimeMeasure

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i <=9; i++ ) {
            SudokuInitial initial = new SudokuInitial(Board.Default(), 1);
            (Reason reason, SudokuGeneration? generation) = initial.SolveWithSimulatedAnnealing();
        }
        stopwatch.Stop();
        TimeSpan averageTime = TimeSpan.FromTicks(stopwatch.Elapsed.Ticks / 10);

        Trace.WriteLine(averageTime);
    }

}
