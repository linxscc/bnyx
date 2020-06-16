



namespace Tang
{
    public class StateMonitor<T>
    {
        ValueMonitor<T> valueMonitor;

        System.Action<T> onBegin;
        System.Action<T> onUpdate;
        System.Action<T> onEnd;

        public StateMonitor(System.Func<T> valueGetter, System.Action<T> onBegin, System.Action<T> onUpdate, System.Action<T> onEnd)
        {
            this.onBegin = onBegin;
            this.onUpdate = onUpdate;
            this.onEnd = onEnd;

            valueMonitor = new ValueMonitor<T>(valueGetter, (T from, T to) =>
            {
                OnEnd(from);
                OnBegin(to);
            });
        }

        void OnBegin(T value)
        {
            if (onBegin != null)
                onBegin(value);
        }

        void OnUpdate(T value)
        {
            if (onUpdate != null)
                onUpdate(value);
        }

        void OnEnd(T value)
        {
            if (onEnd != null)
                onEnd(value);
        }

        public void Update()
        {
            OnUpdate(valueMonitor.value);
            valueMonitor.Update();
        }
    }
}