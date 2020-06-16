using System;


namespace Tang
{
    public interface IValueMonitor
    {
        void Update();
        void Reset();
    }

    public class ValueMonitor<T> : IValueMonitor
    {
        public T oldValue;
        public T value;
        public Func<T> ValueGetter;
        public Action<T, T> Action;

        public ValueMonitor(Func<T> valueGetter, Action<T, T> action, bool callAtOnce = false)
        {
            ValueGetter = valueGetter;
            Action = action;
            
            if(callAtOnce == false)
                value = ValueGetter();
        }

        public void Reset()
        {
            value = default(T);
        }

        public void Update()
        {
            var getValue = ValueGetter();
            if ((value == null && getValue == null)
                || (value != null && value.Equals(ValueGetter())))
            {
            }
            else
            {
                oldValue = value;
                value = ValueGetter();
                Action(oldValue, value);
            }
        }
    }
}