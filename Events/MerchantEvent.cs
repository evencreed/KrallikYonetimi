using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public class MerchantEvent : GameEvent
{
    public MerchantEvent() : base("Tüccar Kervanı", "Yabancı tüccarlar pazarda ticaret yapmak istiyor.") { }

    public override void Execute(Player player)
    {
        int choice = AskChoice("Gıda satın al (-70 altın, +95 gıda)",
            "Kervanı vergilendir (+45 altın, -3 nüfus)");

        if (choice == 1)
        {
            var cost = new Dictionary<string, int> { [ResourceNames.Gold] = 70 };
            if (player.Spend(cost))
            {
                player.AddResource(ResourceNames.Food, 95);
                Console.WriteLine("95 gıda satın alındı.");
            }
            else
                Console.WriteLine("Alım için yeterli altın yok; kervan ayrıldı.");
        }
        else
        {
            player.AddResource(ResourceNames.Gold, 45);
            player.AddResource(ResourceNames.Population, -3);
            Console.WriteLine("45 altın vergi toplandı; tüccarlarla yaşanan gerilim 3 kişinin göçmesine yol açtı.");
        }
    }
}
