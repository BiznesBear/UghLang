using UghLang.Nodes;

namespace UghLang;

// TODO: Better exceptions (and change their names)

[Serializable]
public class UghException(string mess) : Exception(mess)
{

}

[Serializable]
public class InvalidSpellingException(ASTNode node) : UghException($"Invalid spelling of {node}")
{

}

[Serializable]
public class IncorrectAmountOfArgumentException(Name name) : UghException($"Incorrect count of arguments called for {name.Key}")
{

}