# Create a new class library project named sudoku_lib in the sudoku_lib directory
dotnet new classlib -o sudoku_lib
# Create a new class library project named optimization in the optimization directory
dotnet new classlib -o sudoku_optimization
# Create a new console application project named sudoku_cli in the sudoku_cli directory
dotnet new console -o sudoku_cli
# Create a new MSTest project named sudoku_test in the sudoku_test directory
dotnet new mstest -o sudoku_test

# Create a new solution file for the projects
dotnet new sln
# Add the sudoku_cli project to the solution
dotnet sln add ./sudoku_cli
# Add the sudoku_lib project to the solution
dotnet sln add ./sudoku_lib
# Add the optimization project to the solution
dotnet sln add ./sudoku_optimization
# Add the sudoku_test project to the solution
dotnet sln add ./sudoku_test

# Add a reference from sudoku_cli to sudoku_lib
dotnet add ./sudoku_cli reference sudoku_lib
# Add a reference from optimization to sudoku_lib
dotnet add ./sudoku_optimization reference sudoku_lib
# Add a reference from sudoku_test to optimization
dotnet add ./sudoku_test reference sudoku_optimization

# to run tests
cd sudoku_test && dotnet test
# to run application
dotnet run --project sudoku_cli
# to run without plotting 
dotnet run --project sudoku_cli -- --no-plots
