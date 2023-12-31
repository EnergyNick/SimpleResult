﻿using System;
using System.Threading.Tasks;

namespace SimpleResult.Extensions
{
    public static partial class ResultsThenExtensions
    {
        public static async Task<Result> ThenAsync(this Result input, Func<Task> continuation)
        {
            if (input.IsSuccess)
                await continuation();
            return input;
        }

        public static async Task<Result<TValue>> ThenAsync<TValue>(this Result<TValue> input, 
            Func<TValue, Task> continuation)
        {
            if (input.IsSuccess)
                await continuation(input.ValueOrDefault);
            return input;
        }

        public static async Task<Result<TOutput>> ThenAsync<TOutput>(this Result input, 
            Func<Task<TOutput>> continuation)
        {
            return input.IsSuccess
                ? Result.Ok(await continuation())
                : input.ToResult<TOutput>();
        }
        
        public static async Task<Result<TOutput>> ThenAsync<TOutput>(this Result input, 
            Func<Task<Result<TOutput>>> continuation)
        {
            return input.IsSuccess
                ? await continuation()
                : input.ToResult<TOutput>();
        }

        public static async Task<Result<TOutput>> ThenAsync<TInput, TOutput>(this Result<TInput> input,
            Func<TInput, Task<TOutput>> continuation)
        {
            return input.IsSuccess
                ? Result.Ok(await continuation(input.ValueOrDefault))
                : input.ToResult<TOutput>();
        }
        
        public static async Task<Result<TOutput>> ThenAsync<TInput, TOutput>(this Result<TInput> input,
            Func<TInput, Task<Result<TOutput>>> continuation)
        {
            return input.IsSuccess
                ? await continuation(input.ValueOrDefault)
                : input.ToResult<TOutput>();
        }
    }
}