namespace Services
{
    public struct LevelNumber
    {
        public LevelNumber(int value)
        {
            Value = value;
        }
        public readonly int Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    public struct UIMoneyValue
    {
        public UIMoneyValue(int value)
        {
            Value = value;
        }

        public readonly int Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    public struct MoneyValue
    {
        public MoneyValue(int value)
        {
            Value = value;
        }

        public readonly int Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    public struct EarnedValue
    {
        public EarnedValue(int value)
        {
            Value = value;
        }

        public readonly int Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
