using System;
using System.Reflection;
using JuliaNET.Stdlib;
using JuliaNET.Utils;

namespace JuliaNET.Statics
{
    public partial class JuliaStaticLibrary
    {
        internal static readonly MethodInfo
            getglobalsym = SharpReflect.GetMethod<Core.Julia, Any, Any>("GetGlobal"),
            getglobalstr = SharpReflect.GetMethod<Core.Julia, Any, string>("GetGlobal");

        private void GenerateVar(JuliaStaticModule mod,
                                 Any name,
                                 bool constant)
        {
            try
            {
                var varname = name.ToString();
                var ty = ResolveType(mod, name);

                //Throw away names we cant represent as they probably are internal
                if (!ConvertJuliaName(varname, out var netName))
                    return;

                var pb = mod.ModuleBuilder.CreateProperty(netName, ty);
                var mb = mod.ModuleBuilder.CreateGetMethod(pb);

                if (constant)
                    LoadUniversalConstant(mb, varname);
                else
                {
                    //Module.varname => Julia.GetGlobal(Module.Module, :varname)
                    mb.Load.FieldValue(mod.JModuleField);
                    LoadSymbol(mb, varname);
                    mb.Function.Invoke(getglobalsym);
                }

                mb.Return();
            }
            catch (Exception)
            {
                Console.WriteLine("Caught Error While Generating Constant:" + name);
                throw;
            }
        }
    }
}
