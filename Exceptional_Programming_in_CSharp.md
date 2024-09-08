
# Exceptional Programming in C#

## Introduction
If you want to be an exceptional C# programmer, understanding exceptions is critical. While exceptions are powerful tools within the language, misusing them can be like using a screwdriver on a nail or a hammer on a screw. The right tool should always match the job at hand. In this article, we'll explore when and how to use exceptions properly, ensuring that you're making optimal choices for performance and code clarity.

First, we'll discuss why using exceptions for Flow Control can be detrimental and what best practices should be followed. Then, we'll examine the Performance/Cost of exceptions, supported by benchmark data to illustrate their potential impact on your applications. Finally, we’ll dive into the `Result<T>` pattern as an alternative to exceptions, providing an efficient and clean solution for handling predictable errors.

## Section 1: Flow Control Best Practices
Exceptions should be the exception. If you know there is going to be a problem to check for, then check for it. Files sometimes don't exist, you can't divide by zero, and parsing sometimes fails. Let's look at parsing as an example, using both exception handling and the `TryParse` method.

### Exception Example:
```csharp
try
{
    int value = int.Parse("123");
}
catch (FormatException)
{
    // Handle parsing error
}
```

### TryParse Example:
```csharp
bool isValid = int.TryParse("123", out int value);
if (isValid)
{
    // Handle success
}
else
{
    // Handle failure without exception
}
```

These approaches are functionally the same. The code using exceptions is slightly simpler, so why not use that? You can, but only if performance isn't a concern. There are times when performance matters and times when it doesn't.

Would you say a 1% failure rate is exceptional? So, what is the performance cost of an exception occurring 1% of the time?

## Section 2: Performance Costs
Let's look at how I gathered the numbers. I used **BenchmarkDotNet** to run the calculations. If you’re curious and want to experiment yourself, here’s the ([GitHub link](https://github.com/MadInventorQDB/ExceptionsBenchmarks)) https://github.com/MadInventorQDB/ExceptionsBenchmarks to the code. I’ll summarize the key points here.

In all the test cases, I made sure to create new exceptions so that the memory allocation remains consistent, allowing for a fair comparison focused purely on the performance of using exceptions. While creating an exception does have a cost, the larger impact comes from throwing and catching that exception. I'm not going to dig into the complexities of unwinding the stack here, but I do use nested function calls to deepen the stack and demonstrate how that affects the cost of throwing an exception. All the test methods are set up with an exception occurring 1% of the time. 

Now, let’s dive into the numbers:

| Method                                    | Mean        | Error     | StdDev    | Score   |
|------------------------------------------ |------------:|----------:|----------:|--------:|
| NoTryWithoutExceptionHandling             |    266.6 ns |   2.53 ns |   2.24 ns |     1   |
| TryWithoutExceptionHandling               |    416.8 ns |   2.78 ns |   2.60 ns |   1.56  |
| TryWithExceptionHandling                  | 26,189.6 ns | 223.43 ns | 186.57 ns |  98.28  |
| TryWithExceptionHandlingAndRethrow        | 61,749.5 ns | 410.79 ns | 364.15 ns | 231.61  |
| TryWithExceptionNestedFunctionHandling    | 32,893.6 ns | 305.94 ns | 286.18 ns | 123.39  |
| TryWithoutExceptionNestedFunctionHandling |    491.3 ns |   3.02 ns |   2.68 ns |   1.84  |

Looking at the **NoTryWithoutExceptionHandling**, this test creates an exception without even having a `try-catch` block. Interestingly, the fastest code with an exception is one where you can't use it. If we normalize the numbers based on this fastest test, we can assign it a score of 1. This score is the best we're going to see for tests involving exceptions.

Next is **TryWithoutExceptionHandling**, where an exception is created with a `try-catch` block, but it’s not thrown. Yes, adding the `try-catch` block does add overhead, even if the exception is never thrown. This gives it a score of 1.56.

For **TryWithExceptionHandling**, we actually throw and catch the exception. This is where the major costs come into play, resulting in a score of 98.28. Remember, this is happening only 1% of the time, but it is still 98 times slower.

I also included the **TryWithExceptionHandlingAndRethrow** test, where an exception is caught and rethrown with an inner exception. I've seen this in code, and the cost of this approach should be known. It scores 231.61, making it the slowest of all the tests.

Stack depth also carries a performance cost. While an in-depth discussion on the stack is beyond the scope of this article, I’ve included **TryWithExceptionNestedFunctionHandling** and **TryWithoutExceptionNestedFunctionHandling** to expose the performance hit without diving into the complexities. Both tests use a three-method call chain to deepen the stack. The cost is not insignificant, with the chain alone scoring 1.84, and adding the exception bumps the score up to 123.39.

Now that we understand the performance costs, the question remains: without a better solution, why even bother counting the cost with no alternative?

## Section 3: Alternatives to Exceptions
You could avoid using any code that throws exceptions, but that’s not a practical solution. The result pattern offers a solid alternative. Instead of throwing an exception, you store the error in an object and return it. This approach requires checking for error states, which is significantly less expensive than throwing and catching exceptions, as the numbers have shown.

### `Result<T>` Struct:
```csharp
public struct Result<T>
{
    public T Value { get; }
    public string Error { get; }
    public bool IsSuccess => Error == null;

    private Result(T value, string error)
    {
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(value, null);
    public static Result<T> Failure(string error) => new Result<T>(default, error);
}
```

This structure allows you to easily represent both success and failure cases while maintaining performance.

### Example:
Here’s an example where a method attempts an to divide by zero. That would throw an exception, but checking for zero prevents this and sets an error:

```csharp
public static Result<int> Divide(int numerator, int denominator)
{
    if (denominator == 0)
        return Result<int>.Failure("Cannot divide by zero");

    return Result<int>.Success(numerator / denominator);
}

Result<int> result = Divide(10, 0);

if (result.IsSuccess)
{
    Console.WriteLine($"Success! Value is {result.Value}");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### Benchmark Results:
| Method                                    | Mean        | Error     | StdDev    | Score   |
|------------------------------------------ |------------:|----------:|----------:|--------:|
| ResultPatternWithSuccess                  |    197.8 ns |   0.52 ns |   0.49 ns |  0.74   |
| ResultPatternWithFailure                  |    197.7 ns |   0.41 ns |   0.37 ns |  0.74   |

The result pattern is a more efficient and clear alternative to exceptions for handling predictable outcomes, improving performance in critical scenarios.

## Conclusion
The use of exceptions should be reserved for truly exceptional cases. When performance is critical, alternatives like `TryParse` and the `Result<T>` pattern provide a much more efficient and predictable error handling strategy. Writing performant code today means avoiding unnecessary exceptions, and your future self will thank you for it.
