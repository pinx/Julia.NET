using JuliaNET.Core;
using JuliaNET.Stdlib;
using JuliaNET.Utils;

namespace Sandbox48
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var jo = new Options();
            // jo.LoadSystemImage = "my_sys_image_path";
            Julia.Init(jo);

            JModule myModule = Julia.Eval(@"
                    module T
                        f(m1, m2) = m1 .* m2
                    end");

            var m1 = new[] { 2, 3, 4 };
            var m2 = new[] { 3, 4, 5 };

            Any y = myModule
                .GetFunction("f")
                .Invoke(new Any(m1), new Any(m2));
            string.Join(",", y).Println();

            Julia.Exit();
        }
    }
}
