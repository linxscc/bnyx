using UnityEngine;

public abstract class BaseCtl
{
    public abstract void handleEvent(CEvent evt);

    public abstract void init(GameObject gb);
    public void DispatchEvent(CEvent evt)
    {
        EventCenter.Instance.DispatchEvent(evt);
    }
}
