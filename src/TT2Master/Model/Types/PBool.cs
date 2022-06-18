namespace TT2Master.Model.Types
{
    public class PBool
    {
        private bool _oldValue;
        public bool OldValue { get => _oldValue; set => _oldValue = value; }

        private bool _currentValue;
        public bool CurrentValue
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

        public PBool(bool value)
        {
            CurrentValue = value;
        }

        public override string ToString() => CurrentValue.ToString();
    }
}