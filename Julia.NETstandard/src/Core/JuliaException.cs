using System;
using JuliaNET.Stdlib;

//Written by Johnathan Bizzano
namespace JuliaNET.Core
{
    public class JuliaException : Exception
    {
        private static string GetMessage(Any ptr)
        {
            try
            {
                return "JuliaException(\"" + JPrimitive.sprintF
                           .UnsafeInvoke(JPrimitive.showerrorF, ptr, JPrimitive.catch_backtraceF.UnsafeInvoke())
                           .UnboxString() + "\")";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Writing Exception To Console!");
                Console.WriteLine(e);
                Console.WriteLine(ptr.ToString());
                throw;
            }
        }

        public JuliaException(Any excep) : base(GetMessage(excep)) => JuliaCalls.jl_exception_clear();
    }
}
