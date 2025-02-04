namespace UghLang.Nodes;

public static class IReturnAnyHelpers
{
    public static T GetAny<T>(this IReturnAny any) => (T)any.AnyValue;
}

public interface IReturnAny
{
    /// <summary>
    /// Returns any value of the object
    /// </summary>
    public object AnyValue { get; }
}

public interface IReturn<T> : IReturnAny
{
    /// <summary>
    /// Returns value of the object
    /// </summary>
    public T Value { get; }
}

public interface IConstantValue
{
    public object GetConstantValue();
}

public class ConstValueNode<T>(T defalutValue) : ASTNode, IConstantValue, IReturn<T>
{
    public T Value { get; set; } = defalutValue;
    public object AnyValue => Value ?? throw new NullReferenceException("Null value in ValueNode");
    public object GetConstantValue() => AnyValue;
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
