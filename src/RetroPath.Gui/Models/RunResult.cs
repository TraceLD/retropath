using System;

namespace RetroPath.Gui.Models;

public record RunResult(Exception? Exception = null)
{
    public bool IsSuccess => Exception is null;
}