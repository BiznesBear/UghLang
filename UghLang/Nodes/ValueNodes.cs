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
public class StringValueNode : AnyValueNode<string> { public StringValueNode() : base(string.Empty) { } }
public class IntValueNode : AnyValueNode<int> { public IntValueNode() : base(0) { } }
public class BoolValueNode : AnyValueNode<bool> { public BoolValueNode() : base(false) { } }
public class FloatValueNode : AnyValueNode<float> { public FloatValueNode() : base(0f) { } }
