using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public class HarvestEvent : GameEvent
{
    public HarvestEvent() : base("Bereketli Hasat", "Beklenmedik bolluk yaşandı!") { }

    public override void Execute(Player player)
    {
        int choice = AskChoice("Mahsulü depola (+120 gıda)", "Mahsulü sat (+100 altın)");
        if (choice == 1)
        {
            player.AddResource(ResourceNames.Food, 120);
            Console.WriteLine("120 gıda depolandı.");
        }
        else
        {
            player.AddResource(ResourceNames.Gold, 100);
            Console.WriteLine("Hasat satıldı ve 100 altın kazanıldı.");
        }
    }
}
