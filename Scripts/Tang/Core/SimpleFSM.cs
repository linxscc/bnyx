using System;
using System.Collections.Generic;

public class SimpleFSM
{
    public class Event
    {
        public Event(string fromState, string toState, Func<bool> cond, Action action)
        {
            this.fromState = fromState;
            this.toState = toState;
            this.cond = cond;
            this.action = action;
        }

        private string fromState;
        private string toState;
        private Func<bool> cond;
        private Action action;

        public string FromState { get { return fromState; } }
        public string ToState { get { return toState; } }
        public Func<bool> Cond { get { return cond; } }
        public Action Action { get { return action; } }
    }

    private List<string> states = new List<string>();
    private List<Event> events = new List<Event>();
    private string state = null;
    private bool isBegin = false;

    public void Begin()
    {
        isBegin = true;
    }

    public void SetState(string name)
    {
        state = name;
    }

    public void AddState(string name)
    {
        states.Add(name);
    }

    public void AddEvent(string fromState, string toState, Func<bool> cond, Action action)
    {
        events.Add(new Event(fromState, toState, cond, action));
    }

    public void Update()
    {
        if (isBegin == false)
            return;

        foreach (var evt in events)
        {
            if (evt.FromState == state)
            {
                if (evt.Cond())
                {
                    evt.Action();
                    state = evt.ToState;
                }
            }
        }
    }
}