using UghLang.Nodes;
namespace UghLang;

public class UghException : Exception
{
    public override string? StackTrace => Debug.EnabledMessages? base.StackTrace : string.Empty;
    public UghException(string message = "") => Debug.Ugh(message);
}

public class UndefinedInstructions() : UghException("Undefined instructions");
public class InvalidSpellingException(ASTNode node, string? sufix = null) : UghException($"Invalid spelling of {node}{sufix}");
public class ExpectedException(string obj, ASTNode node) : InvalidSpellingException(node, ": expected " + obj);
public class ValidOperationException(string oper) : UghException($"Valid operation: {oper}");
public class IncorrectArgumentsException(string name) : ValidOperationException($"Incorrect arguments or their count called for `{name}`");
