using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JuliaNET.Core
{
    internal static class Boot
    {
        internal static void jl_init_code(Options options,
                                          bool sharpInit)
        {
            if (sharpInit)
            {
                var arguments = options.Arguments.Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();
                if (arguments.Length != 0)
                {
                    int len = arguments.Length;
                    unsafe
                    {
                        var stringBytes = stackalloc byte*[arguments.Length];
                        GCHandle[] handles = new GCHandle[arguments.Length];

                        for (int i = 0; i < arguments.Length; ++i)
                        {
                            handles[i] = GCHandle.Alloc(Encoding.ASCII.GetBytes(arguments[i]), GCHandleType.Pinned);
                            stringBytes[i] = (byte*)handles[i].AddrOfPinnedObject();
                        }

                        JuliaCalls.jl_parse_opts(ref len, &stringBytes);

                        for (int i = 0; i < arguments.Length; i++)
                        {
                            handles[i].Free();
                        }
                    }
                }

                JuliaCalls.jl_init();
            }

            JPrimitive.primitive_init();
            JPrimitive.init_primitive_types();
            Julia.CheckExceptions();
            Julia.CheckExceptions();
        }
    }
}
