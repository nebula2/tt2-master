namespace TT2Master.Model.Types
{
    public class PInt
    {
        private int _oldValue;
        public int OldValue { get => _oldValue; set => _oldValue = value; }

        private int _currentValue;
        public int CurrentValue
        {
            get => _currentValue; set
            {
                _currentValue = value;

                if (value != OldValue)
                {
                    OldValue = value;
                    HasChanged = true;
                }
                else
                {
                    HasChanged = false;
                }
            }
        }

        public bool HasChanged { get; set; }

        public PInt(int value)
        {
            CurrentValue = value;
        }

        public override string ToString() => CurrentValue.ToString();
    }
}