namespace TT2Master.Model.Types
{
    public class PDouble
    {
        private double _oldValue;
        public double OldValue { get => _oldValue; set => _oldValue = value; }

        private double _currentValue;
        public double CurrentValue
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

        public PDouble(double value)
        {
            CurrentValue = value;
        }

        public override string ToString() => CurrentValue.ToString();
    }
}