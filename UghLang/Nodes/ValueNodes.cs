namespace UghLang.Nodes;


public interface IReturnAny
{
    public object AnyValue { get; } 
}

public interface IReturn<T> : IReturnAny
{
    public T Value { get; set; }
}

public class AnyValueNode<T>(T defalutValue) : ASTNode, IReturn<T>
{
    public T Value { get; set; } = defalutValue;
    public object AnyValue => Value ?? throw new UghException("Cannot find value");
}

public class ConstStringValueNode : AnyValueNode<string> { public ConstStringValueNode() : base(string.Empty) { } }
public class ConstIntValueNode : AnyValueNode<int> { public ConstIntValueNode() : base(0) { } }
public class ConstBoolValueNode : AnyValueNode<bool> { public ConstBoolValueNode() : base(false) { } }
public class ConstFloatValueNode : AnyValueNode<float> { public ConstFloatValueNode() : base(0f) { } }
