SOLUTION_NAME = sudoku

all: build

build:
	dotnet build $(SOLUTION_NAME).sln

test:
	cd sudoku_test && dotnet test

run:
	dotnet run --project sudoku_cli

run-no-plots:
	dotnet run --project sudoku_cli -- --no-plots

clean:
	dotnet clean
	rm -rf ./sudoku_cli/bin
	rm -rf ./sudoku_cli/obj
	rm -rf ./sudoku_lib/bin
	rm -rf ./sudoku_lib/obj
	rm -rf ./sudoku_optimization/bin
	rm -rf ./sudoku_optimization/obj
	rm -rf ./sudoku_test/bin
	rm -rf ./sudoku_test/obj

restore:
	dotnet restore

.PHONY: all build test run run-no-plots clean restore
