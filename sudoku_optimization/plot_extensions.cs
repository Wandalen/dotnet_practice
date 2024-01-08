namespace sudoku_optimization;
using sudoku_lib;
using OxyPlot;
using OxyPlot.Series;
using System.Diagnostics;

public class Stats
{
    public List<double> Cost { get; set; }
    public List<double> Temperature { get; set; }
    public List<double> Threshold { get; set; }

    public Stats()
    {
        Cost = [];
        Temperature = [];
        Threshold = [];
    }
}

public static class SudokuInitialExtension
{
    /// <summary>
    /// Solve the Sudoku puzzle using the Simulated Annealing algorithm and collects statistics about the process.
    /// </summary>
    public static (Reason, SudokuGeneration?, Stats) SolveWithSAData(this SudokuInitial sudokuInitial)
    {
        var stats = new Stats();
        var generation = sudokuInitial.InitialGeneration();
        stats.Temperature.Add(generation.Temperature);
        stats.Cost.Add(generation.Person.Cost);

        while (true)
        {
            if (generation.NGeneration > sudokuInitial.NGenerationsLimit)
            {
                return (Reason.GenerationLimit, null, stats);
            }

            Console.WriteLine($"\n= n_generation : {generation.NGeneration}\n");

            var (reason, generation2, thresholds) = generation.MutateWithStats(generation.Hrng);
            for (int i = 0; i < thresholds.Count; i++) 
            {
                stats.Threshold.Add(thresholds[i]);
            }
            

            if (generation2 == null)
            {
                return (reason, null, stats);
            }

            if (generation2.IsGoodEnough())
            {
                return (Reason.GoodEnough, generation2, stats);
            }

            generation = generation2;
            stats.Temperature.Add(generation.Temperature);
            stats.Cost.Add(generation.Person.Cost);
        }
    }
}

public static class SudokuGenerationExtension
{
    /// <summary>
    /// Extension method for Generation mutaton with statistical data.
    /// </summary>
    public static (Reason, SudokuGeneration?, List<double>) MutateWithStats(this SudokuGeneration sudokuGeneration, Random hrng)
    {
        var initial = sudokuGeneration.Initial;
        var temperature = sudokuGeneration.Temperature;
        var nMutations = 0;
        var nResets = sudokuGeneration.NResets;
        var Thresholds = new List<double>();

        SudokuPerson person;

        do
        {
            if (nMutations > initial.NMutationsPerGenerationLimit)
            {
                nResets++;
                if (nResets >= initial.NResetsLimit)
                {
                    return (Reason.ResetLimit, null, Thresholds);
                }

                var temperature2 = temperature + initial.TemperatureIncreaseFactor;
                Console.WriteLine($" 🔄 reset temperature {temperature} -> {temperature2}");
                Sleeper.Sleep();
                temperature = temperature2;
                nMutations = 0;
            }

            var mutagen = sudokuGeneration.Person.Mutagen(initial, hrng);
            person = sudokuGeneration.Person.Mutate(initial, mutagen);

            var costDifference = 0.5 + person.Cost - sudokuGeneration.Person.Cost;
            var threshold = Math.Exp(-costDifference / temperature);

            if (threshold > 1) {
                Thresholds.Add(1);
            } else {
                Thresholds.Add(threshold);
            }

            Console.WriteLine($"cost : {sudokuGeneration.Person.Cost} -> {person.Cost} | costDifference : {costDifference} | temperature : {temperature}");
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
        var nGeneration = sudokuGeneration.NGeneration + 1;

        var generation = new SudokuGeneration(initial, hrng, person, temperature, nResets, nGeneration);

        return (Reason.NotFinished, generation, Thresholds);
    }
}

/// <summary>
/// Creates plots for data gathered during SA process - change of cost, temperature and threshold at every step.
/// </summary>
public class Plotter
{
    /// <summary>
    /// Find parent directory of project.
    /// </summary>
    static string FindProjectDirectory()
    {
        string projectDirectory = Directory.GetCurrentDirectory();
        // Look for the nearest directory containing a .csproj file
        string directory = Directory.GetCurrentDirectory();
        while (!Directory.GetFiles(directory, "*.csproj").Any())
        {
            string parentDirectory = Directory.GetParent(directory)?.FullName;
            if (parentDirectory == null || parentDirectory == directory)
            {
                return projectDirectory; // Reached the root directory without finding a .csproj file
            }

            directory = parentDirectory;
        }

        return Directory.GetParent(directory)?.FullName;
    }

    /// <summary>
    /// Draw plots for data in Stats structure.
    /// </summary>
    public static void Draw(Stats stats)
    {
        string projectDirectory = FindProjectDirectory();
        PlotSeries("Cost change", projectDirectory + "/plots/cost_per_step.svg", true, stats.Cost);
        PlotSeries("Temperature", projectDirectory + "/plots/temperature_per_step.svg", true, stats.Temperature);
        //PlotSeries("Threshold", projectDirectory + "/plots/threshold_per_step.svg", false, stats.Threshold);
    }
    /// <summary>
    /// General method for drawing data series.
    /// </summary>
    static void PlotSeries(string Title, string Filepath,  bool LineSeries, List<double> Data)
    {
        // Create new model object for plotting with given title.
        var model = new PlotModel { Title = Title };
        // Check if data series should be drawn as line series or point series.
        if (LineSeries)
        {
            // Create new line series with black colour. 
            var series = new LineSeries {
                Color = OxyColors.Black, 
            };
            // Transform data values from type double into DataPoint format.
            for (int i = 1; i <= Data.Count; i++) {
                series.Points.Add(new DataPoint(i, Data[i-1]));
            }
            // Add DataPoints to model.
            model.Series.Add(series);
        } else {
            // Create new point series with round shape and black colour. 
            var series = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 1,
                MarkerFill = OxyColors.Black, 
            };
            // Transform data values from type double into ScatterPoint format.
            for (int i = 1; i <= Data.Count; i++) {
                series.Points.Add(new ScatterPoint(i, Data[i-1]));
            }
            // Add ScatterPoints array to model.
            model.Series.Add(series);
        }

        // Create new exporter for svg file.
        var exporter = new SvgExporter { Width = 2500, Height = 400 };
        // Add model with plotted data to exporter and transform it to text.
        var svgText = exporter.ExportToString(model);
        // Write plots in svg format to file in Filepath.
        System.IO.File.WriteAllText(Filepath, svgText);
    }
}
