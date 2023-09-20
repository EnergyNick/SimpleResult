using System.Runtime.CompilerServices;

namespace SimpleResult.Async.Internal;

public sealed class AsyncResultTaskMethodBuilder
{
    private AsyncTaskMethodBuilder _builder = AsyncTaskMethodBuilder.Create();
    private AsyncResult? _result;

    public static AsyncResultTaskMethodBuilder Create() => new();

    public void SetResult() => _builder.SetResult();

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine =>
        _builder.Start(ref stateMachine);

    public AsyncResult Task
    {
        get
        {
            _result ??= new AsyncResult(_builder.Task);

            return _result;
        }
        set => _result = value;
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    public void SetException(Exception e) => _builder.SetException(e);

    public void SetStateMachine(IAsyncStateMachine stateMachine) => _builder.SetStateMachine(stateMachine);
}

public sealed class AsyncResultTaskMethodBuilder<TValue>
{
    private AsyncTaskMethodBuilder<TValue> _builder = AsyncTaskMethodBuilder<TValue>.Create();

    private AsyncResult<TValue>? _result;

    public static AsyncResultTaskMethodBuilder<TValue> Create() => new();

    public void SetResult(TValue value) => _builder.SetResult(value);

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine =>
        _builder.Start(ref stateMachine);

    public AsyncResult<TValue> Task
    {
        get
        {
            _result ??= new AsyncResult<TValue>(_builder.Task);

            return _result;
        }
        set => _result = value;
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    public void SetException(Exception e) => _builder.SetException(e);

    public void SetStateMachine(IAsyncStateMachine stateMachine) => _builder.SetStateMachine(stateMachine);
}