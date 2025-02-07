using System.Reflection;
using UghLang.Nodes;

namespace UghLang;

public abstract class BaseFunction(string name, Ugh ugh) : Name(name, 0)
{
    protected Ugh Ugh => ugh;
    public abstract void Invoke(IReturnAny[] args);
}

public class Function(string name, BlockNode node, string[] localNames) : BaseFunction(name, node.Ugh)
{
    public BlockNode BlockNode { get; } = node;
    private readonly string[] localNames = localNames;

    public override void Invoke(IReturnAny[] args)
    {
        var lastFun = Ugh.Function;
        Ugh.Function = this;

        if (localNames.Length != args.Length) 
            throw new IncorrectArgumentsException(Key);

        for (int i = 0; i < localNames.Length; i++) // declaring local variables 
        {
            Variable v = new(localNames[i], args[i].AnyValue);

            Ugh.RegisterName(v);
            BlockNode.LocalNames.Add(v);
        }

        BlockNode.ForceExecute();
        BlockNode.FreeLocalNames();

        Ugh.Function = lastFun;
    }
}

public class ModuleFunction(string name, Ugh ugh, MethodInfo method) : BaseFunction(name, ugh)
{
    public override void Invoke(IReturnAny[] args)
    {
        Value = method.Invoke(null, args.Select(item => item.AnyValue).ToArray()) ?? 0;
    }
}

public class FunctionCallInfo(IReturnAny[] args)
{
    public IReturnAny[] Arguments { get; set; } = args;
    public BaseFunction? Function { get; set; }
    public void Invoke() => Function?.Invoke(Arguments);
}