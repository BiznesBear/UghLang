using System.Reflection;
using UghLang.Nodes;

namespace UghLang;

public abstract class BaseFunction(string name, Rnm rnm) : Name(name, 0)
{
    protected Rnm Rnm => rnm;
    public abstract void Invoke(IReturnAny[] args);
}

public class Function(string name, BlockNode node, string[] localNames) : BaseFunction(name, node.Rnm)
{
    public BlockNode BlockNode { get; } = node;
    
    public override void Invoke(IReturnAny[] args)
    {
        var lastFun = Rnm.Function;
        Rnm.Function = this;

        if (localNames.Length != args.Length) 
            throw new IncorrectArgumentsException(Key);

        for (int i = 0; i < localNames.Length; i++) // declaring local variables 
        {
            Variable v = new(localNames[i], args[i].AnyValue);

            Rnm.RegisterName(v);
            BlockNode.LocalNames.Add(v);
        }

        BlockNode.ForceExecute();
        BlockNode.FreeLocalNames();

        Rnm.Function = lastFun;
    }
}

public class ModuleFunction(string name, Rnm rnm, MethodInfo method) : BaseFunction(name, rnm)
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