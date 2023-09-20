using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using SimpleResult.Async.Internal;
using SimpleResult.Core;
using SimpleResult.Exceptions;

namespace SimpleResult.Async;

[AsyncMethodBuilder(typeof(AsyncResultTaskMethodBuilder<>))]
public record AsyncResult<TValue> : IConclusion
{
    private readonly Task<Result<TValue>> _awaitAction;
    private ImmutableArray<IError> _errors = ImmutableArray<IError>.Empty;
    private TValue? _value;

    /// <summary>
    /// Get value or return default, if internal task is failed or return failed result.
    /// </summary>
    public TValue? ValueOrDefault => _value;

    /// <summary>
    /// Try get value from async method or synchronous await method completion.
    /// </summary>
    /// <exception cref="OperationOnFailedResultException">
    /// Throw, if internal task throw exception or return failed result.
    /// </exception>
    public TValue Value
    {
        get
        {
            if (!IsCompleted)
                _awaitAction.GetAwaiter().GetResult();

            if (IsFailed)
                throw new OperationOnFailedResultException("Get value");

            return _value!;
        }
    }

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

    internal AsyncResult(TValue value)
    {
        _value = value;
        _awaitAction = Task.FromResult(Result.Ok(_value));
    }

    internal AsyncResult(Task<TValue> func)
    {
        _awaitAction = func.ContinueWith(task =>
        {
            // Use try/catch to obtain more complete information about exceptions.
            // (When checking manually, we lose the stacktrace and something else)
            // If I’m wrong, please write to me in Issue!
            try
            {
                _value = task.GetAwaiter().GetResult();
                return Result.Ok(_value);
            }
            catch (Exception exception)
            {
                _errors = ImmutableArray.Create<IError>(new ExceptionalError(exception));
                return Result.Fail<TValue>(_errors);
            }
        });
    }

    internal AsyncResult(IError error)
    {
        _errors = ImmutableArray.Create(error);
        _awaitAction = Task.FromResult(Result.Fail<TValue>(_errors));
    }

    internal AsyncResult(IEnumerable<IError> errors, bool isFailed = true)
    {
        _errors = errors is IError[] arrayErrors
            ? ImmutableArray.Create(arrayErrors)
            : errors.ToImmutableArray();
        _awaitAction = Task.FromResult(Result.Fail<TValue>(_errors));

        if (isFailed && _errors.Length == 0)
            throw new InvalidResultOperationException("Can't create failed result without errors");
    }

    /// <summary>
    /// Used for provide API for async/await mechanism.
    /// </summary>
    /// <returns>Exception safety awaiter of internal task</returns>
    public TaskAwaiter<Result<TValue>> GetAwaiter() => _awaitAction.GetAwaiter();
}