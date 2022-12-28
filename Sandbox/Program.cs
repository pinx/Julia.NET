using System;
using Julia.NET.Core;
using Julia.NET.Stdlib;
using Julia.NET.Utils;

try
{
    var jo = new JuliaOptions();
    // jo.LoadSystemImage = "my_sys_image_path";
    Julia.NET.Core.Julia.Init(jo);

    JModule myModule = Julia.NET.Core.Julia.Eval(@"
                    module T
                        add!(m1, m2) = m1 .+= m2
                    end
                    using Main.T
                    return T");

    var m1 = new[] { 2, 3, 4 };
    var m2 = new[] { 3, 4, 5 };

    myModule.GetFunction("add!").Invoke(new Any(m1), new Any(m2));
    string.Join(",", m1).Println();

    Julia.NET.Core.Julia.Exit();
}
catch (Exception e)
{
    e.PrintExp();
}
