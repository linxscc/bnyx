using System;
using System.Collections;

public class CEvent
{
    protected Hashtable arguments;
    protected CEventType type;
    protected object sender;
    public CEventType Type
    {
        get
        {
            return this.type;
        }
        set
        {
            this.type = value;
        }
    }

    public IDictionary Params
    {
        get
        {
            return this.arguments;
        }
        set
        {
            this.arguments = (value as Hashtable);
        }
    }

    public object Sender
    {
        get
        {
            return this.sender;
        }
        set
        {
            this.sender = value;
        }
    }

    public override string ToString()
    {
        return this.type + "[" + ((this.sender == null) ? "null" : this.sender.ToString()) + "]";
    }

    public CEvent Clone()
    {
        return new CEvent(this.type, this.arguments, Sender);
    }

    public CEvent(CEventType type, Object sender)
    {
        this.Type = type;
        Sender = sender;
        if (this.arguments == null)
        {
            this.arguments = new Hashtable();
        }
    }

    public CEvent(CEventType type, Hashtable args, Object sender)
    {
        this.Type = type;
        this.arguments = args;
        Sender = sender;
        if (this.arguments == null)
            this.arguments = new Hashtable();
    }

}
public enum CEventType
{
    GAME_OVER,
    GAME_WIN,
    PAUSE,
    MOVE_TO_NEXT,
    NEXT_BATTALE_START,
    ENERGY_EMPTY,
    ENERGY_ENOUGH,
    GAME_DATA,
    GAME_START,
    GAME_EXIT,
    START_MENU,
    EXIT_DIAG,
}
