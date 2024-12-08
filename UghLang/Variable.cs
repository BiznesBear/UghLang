namespace UghLang;
public enum DataType
{
    String,
    Int,
    Undefined
}

public class Variable(string name, dynamic val) : IDisposable
{
    public static readonly Variable NULL = new("", 0);
    public string Name { get; } = name;
    private dynamic value  = val; 

    public void Set(dynamic val) => value = val;
    public dynamic Get() => value;

    public void Dispose() => GC.SuppressFinalize(true);
    public override string ToString() => $"{nameof(Variable)} {{ Name = {Name}, Value = {Get()} }}";
}