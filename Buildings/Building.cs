using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Buildings;

public abstract class Building
{
    private readonly Dictionary<string, int> _baseCost;

    public string Name { get; }
    public int Level { get; private set; } = 1;
    public Dictionary<string, int> Cost => new(_baseCost);
    public virtual bool EnablesTraining => false;

    protected Building(string name, Dictionary<string, int> cost)
    {
        Name = name;
        _baseCost = new Dictionary<string, int>(cost);
    }

    public abstract void Produce(Player player);

    public Dictionary<string, int> GetUpgradeCost() => _baseCost.ToDictionary(
        kv => kv.Key,
        kv => (int)Math.Min(int.MaxValue, (long)kv.Value * (Level + 1)));

    public bool Upgrade(Player player)
    {
        if (Level >= 5 || !player.Spend(GetUpgradeCost()))
            return false;

        Level++;
        return true;
    }
}
