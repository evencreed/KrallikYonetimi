namespace KrallikYonetimi.Units;

public abstract class Unit
{
    private readonly Dictionary<string, int> _cost;

    public string Name { get; }
    public int AttackPower { get; }
    public int Defense { get; }
    public int UpkeepGold { get; }
    public int UpkeepFood { get; }
    public Dictionary<string, int> Cost => new(_cost);

    protected Unit(string name, int attackPower, int defense,
        int upkeepGold, int upkeepFood, Dictionary<string, int> cost)
    {
        Name = name;
        AttackPower = attackPower;
        Defense = defense;
        UpkeepGold = upkeepGold;
        UpkeepFood = upkeepFood;
        _cost = new Dictionary<string, int>(cost);
    }

    public virtual int GetPower() => AttackPower + Defense;
}
