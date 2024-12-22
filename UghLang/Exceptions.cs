using UghLang.Nodes;
namespace UghLang;


[Serializable]
public class UghException(string mess) : Exception(mess) { }

[Serializable]
public class InvalidSpellingException(ASTNode node) : UghException($"Invalid spelling of {node}") { }

[Serializable]
public class IncorrectArgumentsException(Name name) : UghException($"Incorrect arguments or their count called for {name.Key}") { }

[Serializable]
public class MissingThingException(string thing, ASTNode node) : UghException($"Missing {thing} for {node}") { }

[Serializable]
public class EmptyExpressionException(ASTNode node) : UghException($"Empty expression node in iteration {node.Parser.AST.CurrentIteration} ") { }