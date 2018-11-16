namespace CodePlex.Tools.TraceMyNet
{
    using System;
    using System.Windows.Forms;
    using System.IO;
    using System.Threading;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Length == 0) break;
                if (arg[0] == '/' || arg[0] == '-')
                {
                    if (arg.Length == 1) break;
                    string option = arg.Substring(1);
                    if (option == "help" || option == "h" || option == "?")
                    {
                        TraceMyNet.Debug = true;
                        throw new NotImplementedException();
                    }
                    else if (option.StartsWith("debug") || option.StartsWith("verbose") || option.StartsWith("d") || option.StartsWith("v"))
                    {
                        TraceMyNet.Debug = true;
                        int index = option.IndexOf(':');
                        if (index > 0)
                        {
                            TraceMyNet.LogFile = new StreamWriter(option.Substring(index + 1), true);
                        }
                        else
                        {
                            TraceMyNet.LogFile = Console.Out;
                        }
                    }
                    else if (option.StartsWith("config") || option.StartsWith("c"))
                    {
                        TraceMyNet.Debug = true;
                        int index = option.IndexOf(':');
                        using (StreamReader r = new StreamReader(option.Substring(index + 1), true))
                        {
                            ConfigurationData cd = ConfigurationData.Parse(r.ReadToEnd());
                            TraceMyNet.Configuration = cd;
                        }
                    }
                    else if (option.StartsWith("noUi") || option.StartsWith("n"))
                    {
                        TraceMyNet.NoUi = true;
                    }
                }
                else if (File.Exists(arg))
                {
                    TraceMyNet.CurrentFileName = arg;
                }
            }

            if (TraceMyNet.NoUi)
            {
                TraceMyNet program = new TraceMyNet(null);
                // TODO: need an exit strategy
                new ManualResetEvent(false).WaitOne();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TraceMyNet());
            }
        }
    }
}
