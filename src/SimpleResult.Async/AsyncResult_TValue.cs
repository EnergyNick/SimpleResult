using System.Runtime.CompilerServices;
using SimpleResult.Async.Internal;
using SimpleResult.Core;
using SimpleResult.Exceptions;

namespace SimpleResult.Async;

[AsyncMethodBuilder(typeof(AsyncResultTaskMethodBuilder<>))]
public sealed class AsyncResult<TValue> : IConclusion, IValueProvider<TValue>
{
    private static readonly TValue? DefaultTypeValue = default;
    private readonly Task<Result<TValue>> _awaitAction;

    /// <summary>
    /// Try get result value from async method or default, if is not completed yet or failed.
    /// </summary>
    public ref readonly TValue? ValueOrDefault => ref IsCompleted
        ? ref GetOrAwaitSynchronous().ValueOrDefault
        : ref DefaultTypeValue;

    /// <summary>
    /// Try get result value from async method or synchronous await method completion.
    /// </summary>
    /// <exception cref="OperationOnFailedResultException">
    /// Throw, if internal task throw exception or return failed result.
    /// </exception>
    public ref readonly TValue Value => ref GetOrAwaitSynchronous().Value;

    /// <summary>
    /// Return true, if result is not contain errors and async method is complete.
    /// </summary>
    public bool IsSuccess => IsCompleted && GetOrAwaitSynchronous().Errors.Count == 0;

    /// <summary>
    /// Return true, if result is contain errors and async method is complete.
    /// </summary>
    public bool IsFailed => IsCompleted && GetOrAwaitSynchronous().Errors.Count != 0;

    /// <summary>
    /// Return true, if internal task is complete.
    /// </summary>
    public bool IsCompleted => _awaitAction.IsCompleted;

    /// <inheritdoc />
    public IReadOnlyCollection<IError> Errors =>
        IsCompleted
            ? GetOrAwaitSynchronous().Errors
            : ArraySegment<IError>.Empty;

    internal AsyncResult(TValue value) => _awaitAction = Task.FromResult(Result.Ok(value));

    internal AsyncResult(Result<TValue> result) => _awaitAction = Task.FromResult(result);

    internal AsyncResult(Task<TValue> func)
    {
        _awaitAction = func.ContinueWith(task =>
        {
            // Use try/catch to obtain more complete information about exceptions.
            // (When checking manually, we lose the stacktrace and something else)
            // If I’m wrong, please write to me in Issue!
            try
            {
                var value = task.GetAwaiter().GetResult();
                return Result.Ok(value);
            }
            catch (Exception exception)
            {
                return Result.Fail<TValue>(exception);
            }
        });
    }

    internal AsyncResult(Task<Result<TValue>> func)
    {
        _awaitAction = func.ContinueWith(task =>
        {
            // Use try/catch to obtain more complete information about exceptions.
            // (When checking manually, we lose the stacktrace and something else)
            // If I’m wrong, please write to me in Issue!
            try
            {
                return task.GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                return Result.Fail<TValue>(exception);
            }
        });
    }

    internal AsyncResult(IError error) =>
        _awaitAction = Task.FromResult(Result.Fail<TValue>(error));

    internal AsyncResult(IEnumerable<IError> errors) =>
        _awaitAction = Task.FromResult(Result.Fail<TValue>(errors));

    /// <summary>
    /// Used for provide API for async/await mechanism.
    /// </summary>
    /// <returns>Exception safety awaiter of internal task</returns>
    public TaskAwaiter<Result<TValue>> GetAwaiter() => _awaitAction.GetAwaiter();

    /// <summary>
    /// Provide valid synchronous way to get result from asynchronous operation
    /// </summary>
    /// <returns>Return result of initial async operation</returns>
    public Result<TValue> GetOrAwaitSynchronous() => GetAwaiter().GetResult();

    public static implicit operator AsyncResult<TValue>(Task<TValue> task) => new(task);
    public static implicit operator AsyncResult<TValue>(Task<Result<TValue>> task) => new(task);
}