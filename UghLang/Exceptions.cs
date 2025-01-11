using UghLang.Nodes;
namespace UghLang;

// TODO: More, better, and more accurate errors
[Serializable]
public class UghException : Exception
{
    public UghException(string mess) => Debug.Error(mess);
}
public class UndefinedInstructions() : UghException($"Undefined instructions");
public class InvalidSpellingException(ASTNode node, string mess = "") : UghException($"Invalid spelling of {node}{mess}") { }
public class IncorrectArgumentsException(Name name) : UghException($"Incorrect arguments or their count called for `{name.Key}`") { }
public class MissingException(string thing, ASTNode node) : InvalidSpellingException(node, ": missing " + thing) { }
