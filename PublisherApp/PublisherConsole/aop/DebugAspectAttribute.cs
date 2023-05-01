using AspectInjector.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublisherConsole.aop;

[Aspect(Scope.Global)]
[Injection(typeof(DebugAspectAttribute))]
public sealed class DebugAspectAttribute : Attribute
{
    [Advice(Kind.Before, Targets = Target.Method)]
    public void DebugStart(
        [Argument(Source.Type)] Type type,
        [Argument(Source.Name)] string name)
    {
        Console.WriteLine($"[{DateTime.UtcNow}] Method {type.Name}.{name} started");
    }
    
    [Advice(Kind.After, Targets = Target.Method)]
    public void DebugFinish(
        [Argument(Source.Type)] Type type,
        [Argument(Source.Name)] string name)
    {
        Console.WriteLine($"[{DateTime.UtcNow}] Method {type.Name}.{name} finished");
    }
}
