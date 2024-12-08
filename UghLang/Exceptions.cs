namespace UghLang;


[Serializable]
public class NullVariableRefrenceException(Token token) : Exception($"Cannot find variable named {token.StringValue}") { }
