﻿using SimpleResult.Core;

namespace SimpleResult.Settings;

/// <summary>
/// Represent parameters of global settings of <see cref="Result"/>
/// </summary>
public class ResultParameters
{
    /// <summary>
    /// Provide converting of exception in try/catch blocks
    /// </summary>
    public required Func<Exception, IError> DefaultTryCatchHandler { get; init; }
}