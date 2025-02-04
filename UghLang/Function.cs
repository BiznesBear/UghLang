using System.Reflection;
using UghLang.Nodes;

namespace UghLang;

public abstract class BaseFunction(string name, Ugh ugh) : Name(name, 0)
{
    protected Ugh Ugh => ugh;
    public abstract void Invoke(IEnumerable<IReturnAny> args);
}

public class Function(string name, BlockNode node, ExpressionNode exprs) : BaseFunction(name, node.Ugh)
{
    public BlockNode BlockNode { get; } = node;
    private ExpressionNode ExpressionNode { get; } = exprs;


    public override void Invoke(IEnumerable<IReturnAny> args)
    {
        var lastFun = Ugh.Function;
        Ugh.Function = this;

        var nodes = ExpressionNode.GetNodes<NameNode>();

        var nameNodes = nodes as NameNode[] ?? nodes.ToArray();
        var arguments = args as IReturnAny[] ?? args.ToArray();

        if (nameNodes.Length != arguments.Length) throw new IncorrectArgumentsException(Key);

        for (int i = 0; i < nameNodes.Length; i++) // declaring local variables 
        {
            var nameNode = nameNodes[i];

            Variable v = new(nameNode.Token.StringValue, arguments[i].AnyValue);

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
    public override void Invoke(IEnumerable<IReturnAny> args)
    {
        Value = method.Invoke(null, args.Select(item => item.AnyValue).ToArray()) ?? 0;
    }
}

public class FunctionCallInfo(IEnumerable<IReturnAny> args)
{
    public IEnumerable<IReturnAny> Arguments { get; set; } = args;
    public BaseFunction? Function { get; set; }
    public void Invoke() => Function?.Invoke(Arguments);
}