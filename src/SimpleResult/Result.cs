﻿using System.Collections.Immutable;
using SimpleResult.Core;
using SimpleResult.Exceptions;

namespace SimpleResult;

public partial record Result : IConclusion
{
    private readonly ImmutableArray<IError> _errors = ImmutableArray<IError>.Empty;

    /// <inheritdoc />
    public bool IsSuccess => _errors.Length == 0;

    /// <inheritdoc />
    public bool IsFailed => _errors.Length != 0;

    /// <inheritdoc />
    public IReadOnlyCollection<IError> Errors => _errors;

    internal Result()
    { }

    internal Result(IError error) => _errors = ImmutableArray.Create(error);

    internal Result(params IError[] errors) => _errors = ImmutableArray.Create(errors);

    internal Result(IEnumerable<IError> errors) => _errors = errors.ToImmutableArray();

    public Result(Result other) => _errors = other._errors;

    /// <summary>
    /// Provide conversion to <see cref="Result{TNewValue}"/> with same reasons
    /// </summary>
    /// <value>New value of result, can be null only when result is false</value>
    /// <returns>Result with provided value, only if source is success</returns>
    /// <exception cref="ArgumentNullOnSuccessException">Can be thrown, if result is success and not provided new value</exception>
    public Result<TNewValue> ToResult<TNewValue>(TNewValue? value = default)
    {
        if (IsSuccess && value is null)
            throw new ArgumentNullOnSuccessException(nameof(value));

        return IsSuccess
            ? new Result<TNewValue>(value!)
            : new Result<TNewValue>(_errors);
    }
}