using KrallikYonetimi.Buildings;
using KrallikYonetimi.Core;

namespace KrallikYonetimi.Models;

public class Player
{
    private readonly Dictionary<string, int> _resources = new(StringComparer.OrdinalIgnoreCase)
    {
        [ResourceNames.Gold] = 500,
        [ResourceNames.Food] = 300,
        [ResourceNames.Population] = 100
    };

    public string Name { get; }

    // Kopya döndürülür: dış kod sözlüğü değiştirerek kaynakları bozamaz.
    public Dictionary<string, int> Resources => new(_resources, StringComparer.OrdinalIgnoreCase);
    public Manager<Building> Buildings { get; } = new();
    public Army Army { get; } = new();
    public int WonBattles { get; private set; }
    public int LostBattles { get; private set; }
    public int FoodProductionPercent { get; private set; } = 100;
    public int GoldProductionPercent { get; private set; } = 100;
    public bool CanTrain => Buildings.GetAll().Any(building => building.EnablesTraining);

    public Player(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Anadolu" : name.Trim();
    }

    public int GetResource(string name) => _resources.TryGetValue(name, out int amount) ? amount : 0;

    public bool HasEnough(Dictionary<string, int> cost)
    {
        return cost.All(kv => kv.Value >= 0
            && _resources.TryGetValue(kv.Key, out int amount)
            && amount >= kv.Value);
    }

    public bool Spend(Dictionary<string, int> cost)
    {
        if (!HasEnough(cost))
            return false;

        // Ön kontrol başarılı olduğu için işlemler bütün olarak uygulanır.
        foreach (var kv in cost)
        {
            var resource = new Resource(kv.Key, _resources[kv.Key]);
            resource.TryDecrease(kv.Value);
            _resources[kv.Key] = resource.Amount;
        }
        return true;
    }

    public void AddResource(string name, int amount)
    {
        if (!_resources.ContainsKey(name))
            throw new ArgumentException($"Tanımsız kaynak: {name}", nameof(name));

        var resource = new Resource(name, _resources[name]);
        if (amount >= 0)
            resource.Increase(amount);
        else
            resource.DecreaseUpTo(-(long)amount);

        _resources[name] = resource.Amount;
    }

    public void ApplyFoodProductionPenalty(int percentage)
    {
        FoodProductionPercent = Math.Clamp(percentage, 0, 100);
    }

    public void ApplyGoldProductionPenalty(int percentage)
    {
        GoldProductionPercent = Math.Clamp(percentage, 0, 100);
    }

    public void ResetTurnModifiers()
    {
        FoodProductionPercent = 100;
        GoldProductionPercent = 100;
    }

    public void RecordBattle(bool victory)
    {
        if (victory)
            WonBattles++;
        else
            LostBattles++;
    }

    public string MissingResources(Dictionary<string, int> cost)
    {
        var missing = cost.Where(kv => GetResource(kv.Key) < kv.Value)
            .Select(kv => $"{Translate(kv.Key)}: {kv.Value - GetResource(kv.Key)}");
        return string.Join(", ", missing);
    }

    public static string Translate(string name) => name switch
    {
        ResourceNames.Gold => "Altın",
        ResourceNames.Food => "Gıda",
        ResourceNames.Population => "Nüfus",
        _ => name
    };
}
