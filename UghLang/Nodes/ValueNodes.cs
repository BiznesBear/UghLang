namespace UghLang.Nodes;

public interface IReturnAny
{
    public object AnyValue { get; }
}

public interface IReturn<T> : IReturnAny
{
    public T Value { get; set; }
}

public interface IConstantValue
{
    public object ConstantValue { get; }
}

public class ConstValueNode<T>(T defalutValue) : ASTNode, IConstantValue, IReturn<T>
{
    public T Value { get; set; } = defalutValue;
    public object AnyValue => Value ?? throw new NullReferenceException("Null value in ValueNode");
    public object ConstantValue => AnyValue;
}

public class ConstStringValueNode() : ConstValueNode<string>(string.Empty);
public class ConstIntValueNode() : ConstValueNode<int>(0);
public class ConstBoolValueNode() : ConstValueNode<bool>(false);
public class ConstFloatValueNode() : ConstValueNode<float>(0f);
public class ConstDoubleValueNode() : ConstValueNode<double>(0d);
public class ConstDecimalValueNode() : ConstValueNode<decimal>(0m);
public class ConstByteValueNode() : ConstValueNode<byte>(0);
public class ConstNullValueNode() : ASTNode, IReturn<object?>
{
    public object? Value { get; set; } 
    public object AnyValue => null!;
}
