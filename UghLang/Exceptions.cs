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
