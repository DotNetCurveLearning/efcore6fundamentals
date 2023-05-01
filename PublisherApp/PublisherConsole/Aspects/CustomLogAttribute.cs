using MethodBoundaryAspect.Fody.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublisherConsole.Aspects;

public sealed class CustomLogAttribute : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        Log.Information($"Init: {args.Method.DeclaringType.FullName}.{args.Method.Name} [{args.Arguments.Length}] params");
        foreach (var item in args.Method.GetParameters())
        {
            Log.Debug($"{item.Name}: {args.Arguments[item.Position]}");
        }
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        Log.Information($"Exit: [{args.ReturnValue}]");
    }

    public override void OnException(MethodExecutionArgs args)
    {
        Log.Error($"OnException: {args.Exception.GetType()}: {args.Exception.Message}");
    }
}
