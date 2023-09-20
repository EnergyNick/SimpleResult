using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using SimpleResult.Async.Internal;
using SimpleResult.Core;
using SimpleResult.Exceptions;

namespace SimpleResult.Async;

// https://devblogs.microsoft.com/premier-developer/extending-the-async-methods-in-c/
// https://habr.com/ru/articles/470830/

[AsyncMethodBuilder(typeof(AsyncResultTaskMethodBuilder))]
public partial record AsyncResult : IConclusion
{
    private readonly Task<Result> _awaitAction = Task.FromResult(Result.Ok());
    private ImmutableArray<IError> _errors = ImmutableArray<IError>.Empty;

    /// <summary>
    /// Return true, if result is not contain errors and async method is complete.
    /// </summary>
    public bool IsSuccess => _errors.Length == 0 && IsCompleted;

    /// <summary>
    /// Return true, if result is contain errors and async method is complete.
    /// </summary>
    public bool IsFailed => _errors.Length != 0 && IsCompleted;

    /// <summary>
    /// Return true, if internal task is complete.
    /// </summary>
    public bool IsCompleted => _awaitAction.IsCompleted;

    /// <inheritdoc />
    public IReadOnlyCollection<IError> Errors => _errors;

    internal AsyncResult()
    { }

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
                _errors = ImmutableArray.Create<IError>(new ExceptionalError(exception));
                return Result.Fail(_errors);
            }
        });
    }

    internal AsyncResult(IError error) => _errors = ImmutableArray.Create(error);

    internal AsyncResult(IEnumerable<IError> errors, bool isFailed = true)
    {
        _errors = errors is IError[] arrayErrors
            ? ImmutableArray.Create(arrayErrors)
            : errors.ToImmutableArray();

        if (isFailed && _errors.Length == 0)
            throw new InvalidResultOperationException("Can't create failed result without errors");
    }

    /// <summary>
    /// Used for provide API for async/await mechanism.
    /// </summary>
    /// <returns>Exception safety awaiter of internal task</returns>
    public TaskAwaiter<Result> GetAwaiter() => _awaitAction.GetAwaiter();
}