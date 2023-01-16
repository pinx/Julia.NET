using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace JuliaNET.Utils
{
    public static class JuliaUtils
    {
        internal static string GetJuliaDir()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "julia",
                    Arguments = @"-e ""println(\""JULIAPPPATH$(Sys.BINDIR)JULIAPPPATH\"")""",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            var location = proc.StandardOutput.ReadToEnd();
            Regex rg = new("JULIAPPPATH(.+)JULIAPPPATH");
            var match = rg.Match(location);

            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static unsafe T* ToPointer<T>(this Span<T> s) where T : unmanaged => (T*)Unsafe.AsPointer(ref s.GetPinnableReference());
        public static unsafe T* ToPointer<T>(this T[] s) where T : unmanaged => (T*)GCHandle.Alloc(s, GCHandleType.Pinned).AddrOfPinnedObject();

        public static T[,] ToMatrix<T>(this T[][] source) where T : unmanaged
        {
            var dataOut = new T[source.Length, source[0].Length];
            var assertLength = source[0].Length;

            unsafe
            {
                for (var i = 0; i < source.Length; i++)
                {
                    if (source[i].Length != assertLength)
                    {
                        throw new InvalidOperationException("The given jagged array is not rectangular.");
                    }

                    fixed (T* pDataIn = source[i])
                    {
                        fixed (T* pDataOut = &dataOut[i, 0])
                        {
                            CopyBlockHelper.SmartCopy<T>(pDataOut, pDataIn, assertLength);
                        }
                    }
                }
            }

            return dataOut;
        }

        public static void PrintExp(this Exception x,
                                    TextWriter tw = null)
        {
            tw ??= Console.Out;
            tw.WriteLine(x.GetBaseException());

            var st = new StackTrace(x, true);
            var frames = st.GetFrames();

            for (int i = 0; i < frames.Length; i++)
            {
                var frame = frames[i];
                if (frame.GetFileLineNumber() < 1)
                    continue;
                tw.Write("File: " + frame.GetFileName());
                tw.Write(", Method:" + frame.GetMethod().Name);
                tw.Write(", LineNumber: " + frame.GetFileLineNumber());
                if (i == frames.Length - 1)
                {
                    tw.WriteLine();
                    break;
                }

                tw.WriteLine("  -->  ");
            }
        }

        public static void Print<T>(this T o) => Console.Write(o);
        public static void Println<T>(this T o) => Console.WriteLine(o);
    }
}
