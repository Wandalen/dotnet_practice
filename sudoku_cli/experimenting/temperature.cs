namespace sudoku_cli;
// using System;

class TemperatureConverter
{
    public static void Exec(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: TemperatureConverter <C or F> <temperature>");
            return;
        }

        string unit = args[0].ToLower();
        double temperature;

        if (!double.TryParse(args[1], out temperature))
        {
            Console.WriteLine("Invalid temperature format.");
            return;
        }

        double fahrenheit, celsius, kelvin;

        if (unit == "c")
        {
            // Convert from Celsius to Fahrenheit
            fahrenheit = (temperature * 9 / 5) + 32;
            celsius = temperature;
            kelvin = temperature + 273.15;
        }
        else if (unit == "f")
        {
            // Convert from Fahrenheit to Celsius
            celsius = (temperature - 32) * 5 / 9;
            fahrenheit = temperature;
            kelvin = (temperature - 32) * 5 / 9 + 273.15;
        }
        else
        {
            Console.WriteLine("Invalid unit. Please use 'C' or 'F' as the first argument.");
            return;
        }

        Console.WriteLine($"{temperature}°{unit.ToUpper()} is equivalent to:");
        Console.WriteLine($"  {fahrenheit}°F");
        Console.WriteLine($"  {celsius}°C");
        Console.WriteLine($"  {kelvin}K");
    }
}