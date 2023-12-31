﻿using System;

namespace SimpleResult.Extensions
{
    public static partial class ResultsThenExtensions
    {
        public static Result Then(this Result input, Action continuation)
        {
            if (input.IsSuccess)
                continuation();
            return input;
        }

        public static Result<TValue> Then<TValue>(this Result<TValue> input, Action<TValue> continuation)
        {
            if (input.IsSuccess)
                continuation(input.ValueOrDefault);
            return input;
        }

        public static Result<TOutput> Then<TOutput>(this Result input, Func<TOutput> continuation)
        {
            return input.IsSuccess
                ? continuation().ToResult()
                : input.ToResult<TOutput>();
        }
        
        public static Result<TOutput> Then<TOutput>(this Result input, Func<Result<TOutput>> continuation)
        {
            return input.IsSuccess
                ? continuation()
                : input.ToResult<TOutput>();
        }

        public static Result<TOutput> Then<TInput, TOutput>(this Result<TInput> input,
            Func<TInput, TOutput> continuation)
        {
            return input.IsSuccess
                ? continuation(input.ValueOrDefault).ToResult()
                : input.ToResult<TOutput>();
        }
        
        public static Result<TOutput> Then<TInput, TOutput>(this Result<TInput> input,
            Func<TInput, Result<TOutput>> continuation)
        {
            return input.IsSuccess
                ? continuation(input.ValueOrDefault)
                : input.ToResult<TOutput>();
        }
    }
}