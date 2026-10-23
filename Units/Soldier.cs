using KrallikYonetimi.Core;

namespace KrallikYonetimi.Units;

public class Soldier : Unit
{
    public Soldier() : base("Piyade", 12, 8, 3, 2, new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 35,
        [ResourceNames.Food] = 10,
        [ResourceNames.Population] = 1
    }) { }
}
