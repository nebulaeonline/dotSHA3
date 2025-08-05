using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotSHA3;

public sealed class SHA3_4x
{
    private Sha3_4x_Ctx _ctx;
    private readonly SHA3Algorithm _variant;

    static SHA3_4x()
    {
        SHA3Library.Init();
    }

    public SHA3_4x(SHA3Algorithm variant)
    {
        _variant = variant;
        SHA3Interop.sha3_4x_init(ref _ctx, variant);
    }

    public void Update(byte[][] inputs, int length)
    {
        if (inputs.Length != 4) throw new ArgumentException("Must provide exactly 4 input arrays.");

        unsafe
        {
            fixed (byte* ptr0 = inputs[0])
            fixed (byte* ptr1 = inputs[1])
            fixed (byte* ptr2 = inputs[2])
            fixed (byte* ptr3 = inputs[3])
            {
                byte*[] ptrs = { ptr0, ptr1, ptr2, ptr3 };
                fixed (byte** ptrArray = ptrs)
                {
                    SHA3Interop.sha3_4x_update(ref _ctx, (IntPtr)ptrArray, (UIntPtr)length);
                }
            }
        }
    }

    public void FinalizeHash(byte[][] outputs)
    {
        if (outputs.Length != 4) throw new ArgumentException("Must provide 4 output arrays.");
        unsafe
        {
            fixed (byte* out0 = outputs[0])
            fixed (byte* out1 = outputs[1])
            fixed (byte* out2 = outputs[2])
            fixed (byte* out3 = outputs[3])
            {
                byte*[] outs = { out0, out1, out2, out3 };
                fixed (byte** outPtrs = outs)
                {
                    SHA3Interop.sha3_4x_final(ref _ctx, (IntPtr)outPtrs);
                }
            }
        }
    }
}
