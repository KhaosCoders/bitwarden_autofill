using System;

namespace Bitwarden_Autofill;

public partial class App
{
    private void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            Log.Fatal((Exception)args.ExceptionObject, "Unhandled exception");

        UnhandledException += (sender, args) =>
            Log.Fatal(args.Exception, "Unhandled exception");
    }
}
