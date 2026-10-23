using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Buildings;

public class GoldMine : Building
{
    public GoldMine() : base("Altın Madeni", new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 180,
        [ResourceNames.Food] = 20
    }) { }

    public override void Produce(Player player)
    {
        int amount = 80 * Level * player.GoldProductionPercent / 100;
        player.AddResource(ResourceNames.Gold, amount);
        Console.WriteLine($"  {Name} (Sv.{Level}): +{amount} altın");
    }
}
