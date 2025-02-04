using UghLang.Nodes;

namespace UghLang;

public abstract class Name(string name, object val) : IDisposable, IReturnAny
{
    public string Key { get; } = name;

    protected object value = val;
    public object Value
    {
        get => value;
        set
        {
            if (this is IConstantValue)
                throw new ValidOperationException("tried set constant value");
            this.value = value;
        }
    }
    public object AnyValue => Value;


    public T? GetAsOrNull<T>() where T : Name => this as T ?? null;
    public T GetAs<T>() where T : Name => GetAsOrNull<T>() ?? throw new UghException($"{GetType()} is not {typeof(T)}");

    public void Dispose() => GC.SuppressFinalize(this);
    public override string ToString() => $"{nameof(Name)}{{{nameof(Key)} = {Key}; {nameof(Value)} = {Value}}}";
}

public class Variable(string name, object value) : Name(name, value) { }

public class Constant(string name, object value) : Variable(name, value), IConstantValue
{
    public object GetConstantValue() => AnyValue;
}

public class AssemblyConst(string name, Type[] assembly) : Constant(name, assembly)
{
    public Type[] Assembly { get; } = assembly;
}
