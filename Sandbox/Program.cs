using System;
using System.CommandLine;
using System.IO;
using JuliaNET.Core;
using JuliaNET.Stdlib;
using JuliaNET.Utils;

class Program
{
    static void Main(string[] args)
    {
        var fileOption = new Option<FileInfo>(
                                              name: "--file",
                                              description: "Julia file to execute");
        var commandOption = new Option<string>(
                                               name: "--command",
                                               description: "Julia function to execute");
        var rootCommand = new RootCommand("Run Julia code from .NET host");
        rootCommand.Add(fileOption);
        rootCommand.Add(commandOption);
        rootCommand.SetHandler((file,
                                command) =>
        {
            RunFunction(ReadFile(file), command);
        }, fileOption, commandOption);
    }

    private static void RunFunction(string programText,
                                    string functionName)
    {
        try
        {
            var jo = new Options();
            // jo.LoadSystemImage = "my_sys_image_path";
            Julia.Init(jo);

            JModule myModule = Julia.Eval(@"
                    module T
                        add!(m1, m2) = m1 .+= m2
                    end
                    using Main.T
                    return T");

            var m1 = new[] { 2, 3, 4 };
            var m2 = new[] { 3, 4, 5 };

            myModule.GetFunction("add!").Invoke(new Any(m1), new Any(m2));
            string.Join(",", m1).Println();

            Julia.Exit();
        }
        catch (Exception e)
        {
            e.PrintExp();
        }
    }

    private static string ReadFile(FileInfo file)
    {
        try
        {
            return File.ReadAllText(file.FullName);
        }
        catch (Exception)
        {
            return @"
                    module T
                        add!(m1, m2) = m1 .+= m2
                    end
                    using Main.T
                    return T";
        }
    }
}
