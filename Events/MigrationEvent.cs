using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public class MigrationEvent : GameEvent
{
    public MigrationEvent() : base("Göç Dalgası", "Sığınmacı aileler krallığınızın kapısında bekliyor.") { }

    public override void Execute(Player player)
    {
        int choice = AskChoice("Göçmenleri kabul et (+20 nüfus, -35 gıda)",
            "Sınırları kapat (kaynaklar değişmez)");

        if (choice == 1)
        {
            player.AddResource(ResourceNames.Population, 20);
            player.AddResource(ResourceNames.Food, -35);
            Console.WriteLine("20 yeni vatandaş krallığınıza katıldı.");
        }
        else
        {
            Console.WriteLine("Sınırlar kapatıldı; kaynaklar değişmedi.");
        }
    }
}
