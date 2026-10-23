using KrallikYonetimi.Core;

namespace KrallikYonetimi.Units;

public class Archer : Unit
{
    public Archer() : base("Okçu", 20, 5, 4, 2, new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 55,
        [ResourceNames.Food] = 15,
        [ResourceNames.Population] = 1
    }) { }
}
