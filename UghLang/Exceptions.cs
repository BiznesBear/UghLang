using UghLang.Nodes;

namespace UghLang;

// TODO: Better exceptions (and change their names)
// TODO: More exceptions 
// TODO: Add any exceptions
// TODO: Any errord handling

[Serializable]
public class UghException(string mess) : Exception(mess)
{

}

[Serializable]
public class InvalidSpellingException(ASTNode node) : UghException($"Invalid spelling of {node}")
{

}

[Serializable]
public class IncorrectArgumentsException(Name name) : UghException($"Incorrect arguments or their count called for {name.Key}")
{

}