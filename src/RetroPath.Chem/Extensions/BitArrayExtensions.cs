using System.Collections;

namespace RetroPath.Chem.Extensions;

public static class BitArrayExtensions
{
    public static int FastGetCardinality(this BitArray bitArray)
    {
        var ints = new int[(bitArray.Count >> 5) + 1];

        bitArray.CopyTo(ints, 0);

        var count = 0;

        // fix for not truncated bits in last integer that may have been set to true with SetAll()
        ints[^1] &= ~(-1 << (bitArray.Count % 32));

        for (var i = 0; i < ints.Length; i++)
        {
            var c = ints[i];

            // Stanford magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
            unchecked
            {
                c -= ((c >> 1) & 0x55555555);
                c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
            }

            count += c;
        }

        return count;
    }

    public static BitArray NewAnd(this BitArray ba1, BitArray ba2)
    {
        var newBa = new BitArray(ba1.Count);

        for (var i = 0; i < ba1.Count; i++)
        {
            newBa[i] = ba1[i] && ba2[i];
        }

        return newBa;
    }
}