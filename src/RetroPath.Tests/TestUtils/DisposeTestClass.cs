using System;

namespace RetroPath.Tests.TestUtils;

/// <summary>
/// Utility class for easy unit testing of methods involving <see cref="IDisposable"/> stuff.
/// </summary>
internal class DisposeTestClass : IDisposable
{
    public bool HasBeenDisposed { get; private set; }
            
    public void Dispose() => HasBeenDisposed = true;
}
