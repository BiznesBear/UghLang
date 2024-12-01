namespace UghLang;
public enum RefrenceType
{
    Value, // clear value
    Other // variable or other refrence
}
public record Refrence(object Ref, RefrenceType Type)
{
    public object Get(Master odd)
    {
        if (Type == RefrenceType.Value)
            return Ref;
        else return odd.GetVariable(Ref.ToString() ?? "").Value;
    }
}