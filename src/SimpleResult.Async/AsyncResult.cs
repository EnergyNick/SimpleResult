using System.Runtime.CompilerServices;
using SimpleResult.Async.Internal;
using SimpleResult.Core;

namespace SimpleResult.Async;

[AsyncMethodBuilder(typeof(AsyncResultTaskMethodBuilder))]
public sealed partial class AsyncResult : IConclusion
{
    private readonly Task<Result> _awaitAction;

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
    public IReadOnlyCollection<IError> Errors => IsCompleted
        ? GetOrAwaitSynchronous().Errors
        : ArraySegment<IError>.Empty;

    internal AsyncResult(Result result) => _awaitAction = Task.FromResult(result);

    internal AsyncResult(Task action)
    {
        _awaitAction = action.ContinueWith(task =>
        {
            // Use try/catch to obtain more complete information about exceptions.
            // (When checking manually, we lose the stacktrace and something else)
            // If I’m wrong, please write to me in Issue!
            try
            {
                task.GetAwaiter().GetResult();
                return Result.Ok();
            }
            catch (Exception exception)
            {
                return Result.Fail(exception);
            }
        });
    }

    internal AsyncResult(Task<Result> action)
    {
        _awaitAction = action.ContinueWith(task =>
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
                return Result.Fail(exception);
            }
        });
    }

    internal AsyncResult(IError error) =>
        _awaitAction = Task.FromResult(Result.Fail(error));

    internal AsyncResult(IEnumerable<IError> errors) =>
        _awaitAction = Task.FromResult(Result.Fail(errors));

    /// <summary>
    /// Used for provide API for async/await mechanism.
    /// </summary>
    /// <returns>Exception safety awaiter of internal task</returns>
    public TaskAwaiter<Result> GetAwaiter() => _awaitAction.GetAwaiter();

    /// <summary>
    /// Provide valid synchronous way to get result from asynchronous operation
    /// </summary>
    /// <returns>Return result of initial async operation</returns>
    public Result GetOrAwaitSynchronous() => GetAwaiter().GetResult();

    public static implicit operator AsyncResult(Task task) => new(task);
    public static implicit operator AsyncResult(Task<Result> task) => new(task);
}