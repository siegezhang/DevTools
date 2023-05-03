namespace DevTools.Core.Models;

// Model for the SampleDataService. Replace with your own model.
public class MessageInfo
{
    public bool IsOpen
    {
        get;
        set;
    }

    public string Message
    {
        get;
        set;
    }


    public override string ToString() => $"{IsOpen} {Message}";
}