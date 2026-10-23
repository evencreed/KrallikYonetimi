using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public class DroughtEvent : GameEvent
{
    public DroughtEvent() : base("Kuraklık", "Uzun süredir yağmur yağmıyor; tarlalar susuz kaldı.") { }

    public override void Execute(Player player)
    {
        int choice = AskChoice("Komşulardan gıda al (-150 altın, +100 gıda)",
            "Önlem alma (-10 nüfus, bu tur çiftlik üretimi yarıya iner)");

        if (choice == 1)
        {
            var cost = new Dictionary<string, int> { [ResourceNames.Gold] = 150 };
            if (player.Spend(cost))
            {
                player.AddResource(ResourceNames.Food, 100);
                Console.WriteLine("100 gıda depoya eklendi.");
            }
            else
            {
                Console.WriteLine("150 altın yok! Halk zarar gördü ve üretim düştü.");
                ApplyDamage(player);
            }
        }
        else
        {
            ApplyDamage(player);
            Console.WriteLine("Kuraklık, nüfusu ve bu turdaki gıda üretimini etkiledi.");
        }
    }

    private static void ApplyDamage(Player player)
    {
        player.AddResource(ResourceNames.Population, -10);
        player.ApplyFoodProductionPenalty(50);
    }
}
