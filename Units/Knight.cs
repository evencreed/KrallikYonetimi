using KrallikYonetimi.Core;

namespace KrallikYonetimi.Units;

public class Knight : Unit
{
    public Knight() : base("Şövalye", 30, 20, 7, 4, new Dictionary<string, int>
    {
        [ResourceNames.Gold] = 90,
        [ResourceNames.Food] = 25,
        [ResourceNames.Population] = 2
    }) { }

    // Şövalyeler zırhlı oldukları için ilave savunma gücü kazanır.
    public override int GetPower() => base.GetPower() + Defense / 2;
}
