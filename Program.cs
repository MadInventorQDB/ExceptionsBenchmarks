using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ExceptionsBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ExceptionBenchmark>();
        }
    }

    // Basic Result Pattern implementation
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

    public class ExceptionBenchmark
    {
        private const int Iterations = 1000; // You can change this value to modify the number of iterations
        private const double PercentExceptions = 1.0; // This percentage controls the number of exceptions

        // Calculate how many exceptions should be thrown based on the percentage
        private readonly int ExceptionThreshold = (int)(Iterations * (PercentExceptions / 100.0));

        [Benchmark]
        public void NoTryWithoutExceptionHandling()
        {
            for (int i = 0; i < Iterations; i++)
            {
                if (i < ExceptionThreshold) // Trigger exception for the calculated percentage
                {
                    Exception dummyException = new Exception("dummy");
                }
            }
        }

        [Benchmark]
        public void TryWithoutExceptionHandling()
        {
            for (int i = 0; i < Iterations; i++)
            {
                try
                {
                    if (i < ExceptionThreshold)
                    {
                        Exception dummyException = new Exception("dummy");
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [Benchmark]
        public void TryWithExceptionHandling()
        {
            for (int i = 0; i < Iterations; i++)
            {
                try
                {
                    if (i < ExceptionThreshold)
                    {
                        throw new Exception("dummy");
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        
        [Benchmark]
        public void TryWithExceptionHandlingAndRethrow()
        {
            for (int i = 0; i < Iterations; i++)
            {
                try
                {
                    try
                    {
                        if (i < ExceptionThreshold)
                        {
                            throw new Exception("dummy");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Rethrowing exception", ex);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        
        [Benchmark]
        public void TryWithExceptionNestedFunctionHandling()
        {
            for (int i = 0; i < Iterations; i++)
            {
                try
                {
                    if (i < ExceptionThreshold)
                    {
                        CallNestedFunctionWithException();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [Benchmark]
        public void TryWithoutExceptionNestedFunctionHandling()
        {
            for (int i = 0; i < Iterations; i++)
            {
                try
                {
                    if (i < ExceptionThreshold)
                    {
                        CallNestedFunctionWithoutException();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [Benchmark]
        public void ResultPatternWithSuccess()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var result = CallNestedFunctionWithResultPattern(i < ExceptionThreshold);
                if (!result.IsSuccess)
                {
                    // Handle failure (no exception thrown, just return an error result)
                }
            }
        }

        [Benchmark]
        public void ResultPatternWithFailure()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var result = CallNestedFunctionWithResultPattern(i < ExceptionThreshold);
                if (!result.IsSuccess)
                {
                    // Handle failure without throwing exceptions
                    var error = result.Error;
                }
            }
        }

        #region No Exception Nested Methods

        public void CallNestedFunctionWithoutException()
        {
            CallAnotherNestedFunctionWithoutException();
        }

        private void CallAnotherNestedFunctionWithoutException()
        {
            CallAnotherLevelNestedFunctionWithoutException();
        }

        private void CallAnotherLevelNestedFunctionWithoutException()
        {
            FinalFunctionWithoutException();
        }

        private void FinalFunctionWithoutException()
        {
            // There is an exception, but don't throw it.
            var dummyException = new NotImplementedException("This is an intentional error for demonstration purposes.");
            return;
        }

        #endregion

        #region Exception Methods

        private static void CallNestedFunctionWithException()
        {
            CallAnotherNestedFunctionWithException();
        }

        private static void CallAnotherNestedFunctionWithException()
        {
            CallAnotherLevelNestedFunctionWithException();
        }

        private static void CallAnotherLevelNestedFunctionWithException()
        {
            FinalFunction();
        }

        private static void FinalFunction()
        {
            throw new NotImplementedException("This is an intentional error for demonstration purposes.");
        }

        #endregion

        #region Result Pattern Methods

        // Use the Result pattern to avoid exceptions
        public Result<string> CallNestedFunctionWithResultPattern(bool simulateError)
        {
            return simulateError
                ? Result<string>.Failure("An error occurred")
                : CallAnotherNestedFunctionWithResultPattern();
        }

        private Result<string> CallAnotherNestedFunctionWithResultPattern()
        {
            return CallAnotherLevelNestedFunctionWithResultPattern();
        }

        private Result<string> CallAnotherLevelNestedFunctionWithResultPattern()
        {
            return FinalFunctionWithResultPattern();
        }

        private Result<string> FinalFunctionWithResultPattern()
        {
            // Normally would throw an exception, but we'll return a failure result instead
            return Result<string>.Failure("This is an intentional error for demonstration purposes.");
        }

        #endregion
    }
}
