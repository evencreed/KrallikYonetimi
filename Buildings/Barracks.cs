using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Buildings;

public class Barracks : Building
{
    public Barracks() : base("Kışla", new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 220,
        [ResourceNames.Food] = 40
    }) { }

    public override bool EnablesTraining => true;

    public override void Produce(Player player)
    {
        // Kışlanın pasif üretimi yoktur; ordu eğitme eylemini açar.
        Console.WriteLine($"  {Name} (Sv.{Level}): asker eğitimine hazır");
    }
}
