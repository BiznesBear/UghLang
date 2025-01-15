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
    public object AnyValue => Value ?? throw new NullReferenceException("Null value in ValueNode");
}

public class ConstStringValueNode() : AnyValueNode<string>(string.Empty);
public class ConstIntValueNode() : AnyValueNode<int>(0);
public class ConstBoolValueNode() : AnyValueNode<bool>(false);
public class ConstFloatValueNode() : AnyValueNode<float>(0f);
public class ConstDoubleValueNode() : AnyValueNode<double>(0d);
