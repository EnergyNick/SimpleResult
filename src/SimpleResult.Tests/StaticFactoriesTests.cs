﻿using System.Diagnostics.CodeAnalysis;
using SimpleResult.Core;
using SimpleResult.Exceptions;
using SimpleResult.Tests.Helpers;

namespace SimpleResult.Tests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class StaticFactoriesTests
{
    [Fact]
    public void OkMethod_WhenInvoke_ShouldReturnSuccessResult()
    {
        // Act
        var result = Result.Ok();

        // Assert
        result.ShouldBeSuccess();
    }

    [Fact]
    public void OkMethod_WhenInvokeWithValue_ShouldReturnSuccessResultWithValidValue()
    {
        // Arrange
        const string refData = "Test123";
        const int valueData = 145;

        // Act
        var refResult = Result.Ok(refData);
        var valueResult = Result.Ok(valueData);

        // Assert
        refResult.ShouldBeSuccessAndEqualsValue(refData);
        valueResult.ShouldBeSuccessAndEqualsValue(valueData);
    }

    [Fact]
    public void OkMethod_WhenInvokeWithNullValue_ShouldReturnSuccessResultWithNullValue()
    {
        // Arrange
        const string? data = null;

        // Act
        var result = Result.Ok(data);

        // Assert
        result.ShouldBeSuccessAndEqualsValue(data);
    }

    [Fact]
    public void FailMethod_WhenInvokeWithStringErrorMessage_ShouldReturnFailedResultWithValidErrorReason()
    {
        // Arrange
        const string errorReason = "Very bad";

        // Act
        var result = Result.Fail(errorReason);
        var resultWithValue = Result.Fail<string>(errorReason);

        // Assert
        var expectedError = new Error(errorReason);
        result.ShouldBeFailed(expectedError);
        resultWithValue.ShouldBeFailed(expectedError);
    }

    [Fact]
    public void FailMethod_WhenInvokeWithError_ShouldReturnFailedResultWithValidError()
    {
        // Arrange
        var error = new Error("Very bad");

        // Act
        var result = Result.Fail(error);
        var resultWithValue = Result.Fail<string>(error);

        // Assert
        result.ShouldBeFailed(error);
        resultWithValue.ShouldBeFailed(error);
    }

    [Fact]
    public void FailMethod_WhenInvokeWithExceptionalError_ShouldReturnFailedResultWithValidError()
    {
        // Arrange
        var exception = new Exception("Test");
        var error = new ExceptionalError(exception);

        // Act
        var result = Result.Fail(exception);
        var resultWithValue = Result.Fail<string>(exception);

        // Assert
        result.ShouldBeFailed(error);
        resultWithValue.ShouldBeFailed(error);
    }

    [Fact]
    public void FailMethod_WhenInvokeWithErrors_ShouldReturnFailedResultWithValidErrors()
    {
        // Arrange
        var exception = new Exception("Test");
        var errors = new IError[]
        {
            new Error("Very bad"),
            new Error("So bad, but why..."),
            new ExceptionalError(exception)
        };

        // Act
        var result = Result.Fail(errors);
        var resultWithValue = Result.Fail<string>(errors);

        // Assert
        result.ShouldBeFailed(errors);
        resultWithValue.ShouldBeFailed(errors);
    }

    [Fact]
    public void FailMethod_WhenInvokeWithErrorsEnumerable_ShouldReturnFailedResultWithValidErrors()
    {
        // Arrange
        var exception = new Exception("Test");
        var errors = new IError[]
        {
            new Error("Very bad"),
            new Error("So bad, but why..."),
            new ExceptionalError(exception)
        }.AsEnumerable();

        // Act
        var result = Result.Fail(errors);
        var resultWithValue = Result.Fail<string>(errors);

        // Assert
        result.ShouldBeFailed(errors);
        resultWithValue.ShouldBeFailed(errors);
    }

    [Fact]
    public void FailMethod_WhenInvokeWithEmptyErrorsEnumerable_ShouldThrowResultException()
    {
        // Arrange
        var errors = Enumerable.Empty<IError>();

        // Act
        var resultAction = () => Result.Fail(errors);
        var resultWithValueAction = () => Result.Fail<string>(errors);

        // Assert
        resultAction.Should().Throw<InvalidResultOperationException>();
        resultWithValueAction.Should().Throw<InvalidResultOperationException>();
    }

    [Fact]
    public void TryMethod_WhenInvokeWithoutExceptions_ShouldReturnSuccessResult()
    {
        // Arrange
        var method = () => { };

        // Act
        var resultAction = () => Result.Try(method);

        // Assert
        method.Should().NotThrow();
        resultAction.Should().NotThrow();

        var result = resultAction();
        result.ShouldBeSuccess();
    }

    [Fact]
    public void TryMethod_WhenInvokeWithReturnValueAndWithoutExceptions_ShouldReturnSuccessResultWithValidValue()
    {
        // Arrange
        const string refData = "Result is real!";
        const int valueData = 145;

        var refMethod = () => refData;
        var valueMethod = () => valueData;

        // Act
        var refAction = () => Result.Try(refMethod);
        var valueAction = () => Result.Try(valueMethod);

        // Assert
        refMethod.Should().NotThrow();
        refAction.Should().NotThrow();

        valueMethod.Should().NotThrow();
        valueAction.Should().NotThrow();

        var refResult = refAction();
        refResult.ShouldBeSuccessAndReferenceEqualsValue(refData);
        var valueResult = valueAction();
        valueResult.ShouldBeSuccessAndEqualsValue(valueData);
    }

    [Fact]
    public void TryMethod_WhenInvokeWithExceptions_ShouldReturnFailedResult()
    {
        // Arrange
        var expectedException = new Exception("Oops");
        var expectedError = new ExceptionalError(expectedException);

        Action action = () => throw expectedException;

        // Act
        var resultAction = () => Result.Try(action);

        // Assert
        action.Should().Throw<Exception>();
        resultAction.Should().NotThrow();

        var result = resultAction();
        result.ShouldBeFailed(expectedError);
    }

    [Fact]
    public void TryMethod_WhenInvokeWithReturnValueAndWithExceptionsAndCustomHandler_ShouldReturnFailedResultWithValidError()
    {
        // Arrange
        var exception = new Exception("Monads are not bad!");
        Func<int> refMethod = () => throw exception;
        Func<string> valueMethod = () => throw exception;

        var error = new Error("So bad");
        var handler = (Exception _) => error;

        // Act
        var refAction = () => Result.Try(refMethod, handler);
        var valueAction = () => Result.Try(valueMethod, handler);

        // Assert
        refMethod.Should().Throw<Exception>();
        refAction.Should().NotThrow();

        valueMethod.Should().Throw<Exception>();
        valueAction.Should().NotThrow();

        var refResult = refAction();
        refResult.ShouldBeFailed(error);
        var valueResult = valueAction();
        valueResult.ShouldBeFailed(error);
    }

    [Fact]
    public async Task TryAsyncMethod_WhenInvokeWithoutExceptions_ShouldReturnSuccessResult()
    {
        // Arrange
        var method = () => Task.CompletedTask;

        // Act
        var resultAction = async () => await Result.TryAsync(method);

        // Assert
        await method.Should().NotThrowAsync();
        await resultAction.Should().NotThrowAsync();

        var result = await resultAction();
        result.ShouldBeSuccess();
    }

    [Fact]
    public async Task TryAsyncMethod_WhenInvokeWithReturnValueAndWithoutExceptions_ShouldReturnSuccessResultWithValidValue()
    {
        // Arrange
        const string refData = "Result is real!";
        const int valueData = 145;

        var refMethod = () => Task.FromResult(refData);
        var valueMethod = () => Task.FromResult(valueData);

        // Act
        var refAction = () => Result.TryAsync(refMethod);
        var valueAction = () => Result.TryAsync(valueMethod);

        // Assert
        await refMethod.Should().NotThrowAsync();
        await refAction.Should().NotThrowAsync();

        await valueMethod.Should().NotThrowAsync();
        await valueAction.Should().NotThrowAsync();

        var refResult = await refAction();
        refResult.ShouldBeSuccessAndReferenceEqualsValue(refData);
        var valueResult = await valueAction();
        valueResult.ShouldBeSuccessAndEqualsValue(valueData);
    }

    [Fact]
    public async Task TryAsyncMethod_WhenInvokeWithExceptions_ShouldReturnFailedResult()
    {
        // Arrange
        var expectedException = new Exception("Oops");
        var expectedError = new ExceptionalError(expectedException);

        Func<Task> action = () => throw expectedException;

        // Act
        var resultAction = () => Result.TryAsync(action);

        // Assert
        await action.Should().ThrowAsync<Exception>();
        await resultAction.Should().NotThrowAsync();

        var result = await resultAction();
        result.ShouldBeFailed(expectedError);
    }

    [Fact]
    public async Task TryAsyncMethod_WhenInvokeWithReturnValueAndWithExceptionsAndCustomHandler_ShouldReturnFailedResultWithValidError()
    {
        // Arrange
        var exception = new Exception("Monads are not bad!");
        Func<Task<int>> refMethod = () => throw exception;
        Func<Task<string>> valueMethod = () => throw exception;

        var error = new Error("So bad");
        var handler = (Exception _) => error;

        // Act
        var refAction = () => Result.TryAsync(refMethod, handler);
        var valueAction = () => Result.TryAsync(valueMethod, handler);

        // Assert
        await refMethod.Should().ThrowAsync<Exception>();
        await refAction.Should().NotThrowAsync();

        await valueMethod.Should().ThrowAsync<Exception>();
        await valueAction.Should().NotThrowAsync();

        var refResult = await refAction();
        refResult.ShouldBeFailed(error);
        var valueResult = await valueAction();
        valueResult.ShouldBeFailed(error);
    }
}