using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Buildings;

public class Farm : Building
{
    public Farm() : base("Çiftlik", new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 120,
        [ResourceNames.Food] = 20
    }) { }

    public override void Produce(Player player)
    {
        int amount = 75 * Level * player.FoodProductionPercent / 100;
        player.AddResource(ResourceNames.Food, amount);
        Console.WriteLine($"  {Name} (Sv.{Level}): +{amount} gıda");
    }
}
