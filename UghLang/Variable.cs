using UghLang.Nodes;

namespace UghLang;
public class Variable(string name, object val) : IDisposable, IReturnAny
{
    public static readonly Variable NULL = new("", 0);
    public string Name { get; } = name;
    public object Value { get; set; } = val;

    public object AnyValue => Value;


    public void Dispose() => GC.SuppressFinalize(this);
    public override string ToString() => $"{nameof(Variable)} {{ Name = {Name}, Value = {Value} }}";
}