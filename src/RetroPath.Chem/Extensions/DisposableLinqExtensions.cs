namespace RetroPath.Chem.Extensions;

public static class DisposableLinqExtensions
{
    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence and disposes the remaining elements.
    /// </summary>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="count">The number of elements to return.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <returns>A sequence that contains the specified number of elements from the start of the input sequence.</returns>
    ///
    /// <remarks>For <see cref="IDisposable.Dispose"/> to take effect the <see cref="IEnumerable{T}"/> must be materialised via ToList().</remarks>
    public static IEnumerable<TSource> TakeAndDispose<TSource>(this IEnumerable<TSource> source, int count) 
        where TSource : IDisposable
    {
        var iCount = 0;
        
        foreach (var element in source)
        {
            if (iCount < count)
            {
                yield return element;
            }
            else
            {
                element?.Dispose();
            }
            
            iCount++;
        }
    }
}