using UghLang.Nodes;

namespace UghLang;

// TODO: Better exceptions 

[Serializable]
public class UghException : Exception
{
    public UghException() 
    {
        
    }

    public UghException(string mess) : base(mess)
    {

    }
}

[Serializable]
public class InvalidSpellingException : UghException
{
    public InvalidSpellingException(ASTNode node) : base($"Invalid spelling of {node.GetType()}")
    {

    }
}