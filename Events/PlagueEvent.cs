using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public class PlagueEvent : GameEvent
{
    public PlagueEvent() : base("Salgın Hastalık", "Krallığınızda tehlikeli bir hastalık yayılıyor.") { }

    public override void Execute(Player player)
    {
        int choice = AskChoice("Şifacı tut (-110 altın)",
            "Karantina uygula (-8 nüfus, bu tur maden üretimi yarıya iner)");

        if (choice == 1)
        {
            var cost = new Dictionary<string, int> { [ResourceNames.Gold] = 110 };
            if (player.Spend(cost))
                Console.WriteLine("Şifacılar hastalığı kontrol altına aldı.");
            else
            {
                Console.WriteLine("Şifacı için para yok! Karantina uygulandı.");
                Quarantine(player);
            }
        }
        else
        {
            Quarantine(player);
            Console.WriteLine("Karantina nüfus ve maden üretimini azalttı.");
        }
    }

    private static void Quarantine(Player player)
    {
        player.AddResource(ResourceNames.Population, -8);
        player.ApplyGoldProductionPenalty(50);
    }
}
