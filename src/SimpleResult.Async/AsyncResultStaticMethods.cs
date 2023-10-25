namespace SimpleResult.Async;

public partial class AsyncResult
{
    /// <summary>
    /// Gets a successfully completed result.
    /// </summary>
    public static AsyncResult CompletedResult { get; } = new(Result.Ok());

    /// <summary>
    /// Create competed async result with provided result.
    /// </summary>
    /// <param name="result">Result for state of completed result</param>
    /// <returns>Completed result with state on input result</returns>
    public static AsyncResult FromResult(Result result) => new(result);

    /// <summary>
    /// Create competed async result with provided result.
    /// </summary>
    /// <param name="result">Result for state of completed result</param>
    /// <typeparam name="TValue">Type of results value</typeparam>
    /// <returns>Completed result with state on input result</returns>
    public static AsyncResult<TValue> FromResult<TValue>(Result<TValue> result) => new(result);

    /// <summary>
    /// Create competed async result from provided value.
    /// </summary>
    /// <param name="value">Value of new success result</param>
    /// <typeparam name="TValue">Type of value</typeparam>
    /// <returns>Completed result with successful state with input value</returns>
    public static AsyncResult<TValue> FromValue<TValue>(TValue value) => new(value);
}