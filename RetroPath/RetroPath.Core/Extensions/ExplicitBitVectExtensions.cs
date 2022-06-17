using System.Collections;
using GraphMolWrap;

namespace RetroPath.Core.Extensions;

public static class ExplicitBitVectExtensions
{
    public static BitArray ToCsBitArray(this ExplicitBitVect bv)
    {
        var bvCount = (int)bv.getNumBits();
        var arr = new BitArray(bvCount);

        for (var i = 0; i < bvCount; i++) 
        {
            if (bv.getBit((uint)i))
            {
                arr.Set(i, true);
            }
        }

        return arr;
    }
}