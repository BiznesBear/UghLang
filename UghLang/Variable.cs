namespace UghLang;
public enum DataType
{
    String,
    Int,
    Bool,
    Undefined
}
public interface IDynamic
{
    public dynamic Dynamic { get; set; }
}
public class Variable(string name, dynamic val) : IDisposable
{
    public static readonly Variable NULL = new("", 0);

    public string Name { get; } = name;
    
    private dynamic dnmc = val;

    public void Set(dynamic val) => dnmc = val;
    public dynamic Get() => dnmc;


    public void Dispose() => GC.SuppressFinalize(this);

    public override string ToString() => $"{nameof(Variable)} {{ Name = {Name}, Value = {Get()} }}";
}