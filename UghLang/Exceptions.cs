using UghLang.Nodes;
namespace UghLang;

public class UghException : Exception
{
    public override string? StackTrace => Debug.EnabledMessages? base.StackTrace : string.Empty;
    public UghException(string message) => Debug.Error(message);
}

public class UndefinedInstructions() : UghException("Undefined instructions");
public class InvalidSpellingException(ASTNode node, string? sufix = null) : UghException($"Invalid spelling of {node}{sufix}") { }
public class ExpectedException(string obj, ASTNode node) : InvalidSpellingException(node, ": expected " + obj) { }
public class UnExpectedException(string obj, ASTNode node) : InvalidSpellingException(node, ": unexpected " + obj) { }
public class IncorrectArgumentsException(Name name) : UghException($"Incorrect arguments or their count called for `{name.Key}`") { }
