using System;
using System.Collections.Generic;

namespace JuliaNET.Core
{
    public class Options
    {
        public string JuliaDirectory;

        public List<string> Arguments = new();

        public int ThreadCount = 1;

        public int WorkerCount = 1;

        public int Optimize = 2;

        public string LoadSystemImage;

        public string EvaluationString;

        public bool UseSystemImageNativeCode = true;

        public bool HandleSignals = true;

        public bool PrecompileModules = true;

        public void Add(params object[] args)
        {
            foreach (var arg in args)
                Arguments.Add(arg.ToString());
        }

        private string AsJLString(bool b) => b ? "yes" : "no";

        internal void BuildArguments()
        {
            Add("");

            if (ThreadCount != 1)
                Add("-t", ThreadCount);

            if (WorkerCount != 1)
                Add("-p", WorkerCount);

            if (Optimize != 2)
                Add("-O", Optimize);

            if (EvaluationString != null)
                Add("-e", EvaluationString);

            if (LoadSystemImage != null)
                Add("-J", LoadSystemImage);

            if (!UseSystemImageNativeCode)
                Add("--sysimage-native-code=", AsJLString(UseSystemImageNativeCode));

            if (!PrecompileModules)
                Add("--compiled-modules=", AsJLString(PrecompileModules));

            if (!HandleSignals)
                Add("--handle-signals =", AsJLString(PrecompileModules));

            if (JuliaDirectory != null)
                Julia.JuliaDir = JuliaDirectory;
            else JuliaDirectory = Julia.JuliaDir;

            if (JuliaDirectory == null)
                throw new Exception("Julia Path Not Found");
        }
    }
}
