﻿namespace EffectiveResult.Abstractions;

public interface IValueStorage<TValue>
{
    ref readonly TValue? ValueOrDefault { get; }

    ref readonly TValue Value { get; }
}