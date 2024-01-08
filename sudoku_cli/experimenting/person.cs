namespace sudoku_cli;

class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person()
    {
        Name = "Default Name";
        Age = 0; // Or any default age you think is appropriate
    }

    public void SayHello()
    {
        Console.WriteLine($"Hello, my name is {Name} and I'm {Age} years old.");
    }

    public static void Exec(string[] args)
    {
        Person john = new Person();
        john.Name = "John";
        john.Age = 30;
        john.SayHello();

        Console.WriteLine("Hello, World!");
    }
}