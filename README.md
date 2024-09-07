
# Exceptional Programming in C#

## Overview

This repository contains the source code and benchmarks referenced in the article ["Exceptional Programming in C#"](insert-link-to-article), which explores best practices for using exceptions in C# and alternative patterns to improve performance and code clarity.

### Key Topics Covered:
- **Flow Control Best Practices**: Why using exceptions for normal flow control should be avoided.
- **Performance Costs of Exceptions**: Benchmark data illustrating the performance penalties associated with exceptions.
- **Alternatives to Exceptions**: Implementation of the `Result<T>` pattern and the use of `TryParse` for handling predictable errors efficiently.

## Code and Benchmarks

The code in this repository includes benchmarks created using **BenchmarkDotNet** to measure the performance impact of exceptions in C#.

### Benchmarks Included:
1. **NoTryWithoutExceptionHandling**: Code without a `try-catch` block.
2. **TryWithoutExceptionHandling**: A `try-catch` block without an exception being thrown.
3. **TryWithExceptionHandling**: A `try-catch` block where an exception is thrown and caught.
4. **TryWithExceptionHandlingAndRethrow**: Catching an exception and rethrowing with an inner exception.
5. **TryWithExceptionNestedFunctionHandling**: Testing performance with nested function calls to simulate a deeper stack.
6. **ResultPatternWithSuccess/Failure**: Using the `Result<T>` pattern to avoid exceptions and compare performance between success and failure scenarios.

## How to Run the Benchmarks

To run the benchmarks yourself:

1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/ExceptionsBenchmarks.git
   ```

2. Navigate to the repository directory:
   ```bash
   cd ExceptionsBenchmarks
   ```

3. Ensure you have **.NET SDK** installed. You can download it [here](https://dotnet.microsoft.com/download).

4. Run the benchmarks using **BenchmarkDotNet**:
   ```bash
   dotnet run -c Release
   ```

## Results Overview

Here’s a sample of the results comparing different exception-handling techniques:

| Method                                    | Mean        | Error     | StdDev    | Score   |
|------------------------------------------ |------------:|----------:|----------:|--------:|
| NoTryWithoutExceptionHandling             |    266.6 ns |   2.53 ns |   2.24 ns | 1       |
| TryWithoutExceptionHandling               |    416.8 ns |   2.78 ns |   2.60 ns | 1.56    |
| TryWithExceptionHandling                  | 26,189.6 ns | 223.43 ns | 186.57 ns | 98.28   |
| TryWithExceptionHandlingAndRethrow        | 61,749.5 ns | 410.79 ns | 364.15 ns | 231.61  |
| TryWithExceptionNestedFunctionHandling    | 32,893.6 ns | 305.94 ns | 286.18 ns | 123.39  |
| ResultPatternWithSuccess                  |    197.8 ns |   0.52 ns |   0.49 ns | 0.74    |
| ResultPatternWithFailure                  |    197.7 ns |   0.41 ns |   0.37 ns | 0.74    |

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
