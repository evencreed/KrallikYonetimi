using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Buildings;

public class House : Building
{
    public House() : base("Ev", new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 140,
        [ResourceNames.Food] = 30
    }) { }

    public override void Produce(Player player)
    {
        int amount = 5 * Level;
        player.AddResource(ResourceNames.Population, amount);
        Console.WriteLine($"  {Name} (Sv.{Level}): +{amount} nüfus");
    }
}
