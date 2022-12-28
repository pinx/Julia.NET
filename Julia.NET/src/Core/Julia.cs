﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Julia.NET.Stdlib;
using Julia.NET.Utils;

//Written by Johnathan Bizzano
namespace Julia.NET.Core
{
    public class Julia
    {
        public static bool IsInitialized { get; private set; }
        private static string _juliaDir;
        public static bool PreviouslyLoaded { get; private set; }
        private static object _gclock = new();
        public static string LibDirectory => MString(JuliaCalls.jl_get_libdir());
        public static bool IsInstalled => JuliaDir != null;

        public static string JuliaDir
        {
            get
            {
                if (_juliaDir != null)
                    return _juliaDir;
                _juliaDir = JLUtils.GetJuliaDir();
                return _juliaDir;
            }
            set => _juliaDir = value;
        }

        public static void Init() => Init(new JuliaOptions());
        public static void Init(JuliaOptions options) => Init(options, true);

        internal static void Init(JuliaOptions options,
                                  bool sharpInit)
        {
            if (IsInitialized) return;
            IsInitialized = true;

            var startTime = DateTime.Now;
            var memSize = GC.GetTotalMemory(false);

            if (PreviouslyLoaded)
                throw new InvalidOperationException("Cannot Close And Reopen Julia in the Same Process");

            try
            {
                options.BuildArguments();
                var env = Environment.CurrentDirectory;
                Environment.CurrentDirectory = options.JuliaDirectory;
                JuliaDll.Open();
                JuliaBoot.jl_init_code(options, sharpInit);
                Environment.CurrentDirectory = env;
                PreviouslyLoaded = true;

                var time = DateTime.Now - startTime;
                var bytes = GC.GetTotalMemory(false) - memSize;
                Console.WriteLine("Initialized Julia.NET in " + time.Milliseconds + " ms and " + bytes + " bytes");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed To Initialize Julia.NET!");
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool Isa(Any v,
                               Any t) => JuliaCalls.jl_isa(v, t) != 0;

        public static void SetGlobal(Any m,
                                     string sym,
                                     Any val) => SetGlobal(m, JuliaCalls.jl_symbol(sym), val);

        public static void SetGlobal(Any m,
                                     Any sym,
                                     Any val)
        {
            JuliaCalls.jl_set_global(m, sym, val);
            CheckExceptions();
        }

        public static Any GetGlobal(Any m,
                                    Any sym)
        {
            var val = JuliaCalls.jl_get_global(m, sym);
            CheckExceptions();
            return val;
        }

        public static Any Symbol(string str) => JuliaCalls.jl_symbol(str);

        public static Any GetGlobal(Any m,
                                    string sym) => GetGlobal(m, Symbol(sym));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void CheckExceptions()
        {
            if (JuliaCalls.jl_exception_occurred() != IntPtr.Zero)
                throw new JuliaException(JuliaCalls.jl_exception_occurred());
        }

        public static void Exit(int code = 0)
        {
            if (!IsInitialized) return;
            IsInitialized = false;

            Console.WriteLine("Disposing Julia.NET");
            JuliaCalls.jl_atexit_hook(code);
            JuliaDll.Close();
        }

        public static Any Eval(string str)
        {
            var val = JuliaCalls.jl_eval_string(str);
            CheckExceptions();
            return val;
        }

        public static Any Eval(string str,
                               Any module) => JPrimitive.ievalF.Invoke(str, module);

        public static void PUSH_GC(Span<Any> values)
        {
            lock (_gclock) JuliaGC.JL_GC_PUSHARGS(values);
        }

        public static void POP_GC()
        {
            lock (_gclock) JuliaGC.JL_GC_POP();
        }

        public static string UnboxString(Any val) => Marshal.PtrToStringAnsi(JuliaCalls.jl_string_ptr(val));

        public static unsafe Any AllocStruct(Any type,
                                             Span<Any> vals) => JuliaCalls.jl_new_structv(type, vals.ToPointer(), (uint)vals.Length);

        private static string MString(IntPtr p)
        {
            CheckExceptions();
            return Marshal.PtrToStringAnsi(p);
        }
    }


    public class CSharp
    {
#if NET
        [UnmanagedCallersOnly]
#endif
        public static int Init(IntPtr julia_bindir)
        {
            var jo = new JuliaOptions();
            jo.JuliaDirectory = Marshal.PtrToStringUni(julia_bindir);
            Julia.Init(jo, false);
            return 0;
        }
    }
}
