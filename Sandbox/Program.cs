using System;
using JuliaNET.Core;
using JuliaNET.Stdlib;
using JuliaNET.Utils;

try
{
    var jo = new Options();
    // jo.LoadSystemImage = "my_sys_image_path";
    Julia.Init(jo);

    Julia.Eval("include(\"test.jl\")");

    var topMod = Julia.Eval("TopMod");
    var subMod = topMod.GetGlobal("SubMod");
    var subF = subMod.GetFunction("SubF");
    var topF = topMod.GetFunction("TopF");
    
    Julia.Exit();
}
catch (Exception e)
{
    e.PrintExp();
}
