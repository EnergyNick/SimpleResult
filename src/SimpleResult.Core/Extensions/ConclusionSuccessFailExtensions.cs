﻿using SimpleResult.Core;

namespace SimpleResult.Extensions;

public static class ConclusionSuccessFailExtensions
{
    /// <summary>
    /// Provide chaining method for action on success result
    /// </summary>
    /// <param name="input">Source of conclusion</param>
    /// <param name="continuation">Action for invoke on success</param>
    /// <returns>Conclusion from <paramref name="input"/></returns>
    public static TConclusion OnSuccess<TConclusion>(this TConclusion input, Action continuation)
        where TConclusion : IConclusion
    {
        if (input.IsSuccess)
            continuation();

        return input;
    }

    /// <summary>
    /// Call action only if <see cref="input"/> is failed
    /// </summary>
    /// <param name="input">Source of conclusion</param>
    /// <param name="onFailAction">Action for invoke on fail</param>
    /// <typeparam name="TConclusion">Type of conclusion</typeparam>
    /// <returns>Conclusion from <paramref name="input"/></returns>
    public static TConclusion OnFail<TConclusion>(this TConclusion input, Action onFailAction)
        where TConclusion : IConclusion
    {
        if (input.IsFailed)
            onFailAction();

        return input;
    }

    /// <summary>
    /// Call action only if <see cref="input"/> is failed
    /// </summary>
    /// <param name="input">Source of conclusion</param>
    /// <param name="onFailAction">Action for invoke on fail</param>
    /// <typeparam name="TConclusion">Type of conclusion</typeparam>
    /// <returns>Conclusion from <paramref name="input"/></returns>
    public static TConclusion OnFail<TConclusion>(this TConclusion input, Action<IEnumerable<IError>> onFailAction)
        where TConclusion : IConclusion
    {
        if (input.IsFailed)
            onFailAction(input.Errors);

        return input;
    }
}