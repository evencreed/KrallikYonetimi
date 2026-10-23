using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public class EnemyAttackEvent : GameEvent
{
    public EnemyAttackEvent() : base("Düşman Saldırısı", "Sınırda düşman birlikleri görüldü!") { }

    public override void Execute(Player player)
    {
        int enemyPower = Random.Shared.Next(25, 101);
        Console.WriteLine($"Düşman gücü tahmini: {enemyPower}; ordunuzun gücü: {player.Army.TotalPower}");
        int choice = AskChoice("Savaş", "Haraç öde (-120 altın)");

        if (choice == 1)
        {
            Fight(player, enemyPower);
        }
        else
        {
            var tribute = new Dictionary<string, int> { [ResourceNames.Gold] = 120 };
            if (player.Spend(tribute))
                Console.WriteLine("Haraç ödendi; düşman geri çekildi.");
            else
            {
                Console.WriteLine("Haraç için altın yetmedi. Savaş başladı!");
                Fight(player, enemyPower);
            }
        }
    }

    private static void Fight(Player player, int enemyPower)
    {
        if (player.Army.TotalPower >= enemyPower)
        {
            player.RecordBattle(true);
            player.AddResource(ResourceNames.Gold, 80);
            Console.WriteLine("Zafer! Düşmanın 80 altınlık ganimeti ele geçirildi.");
        }
        else
        {
            player.RecordBattle(false);
            int casualties = player.Army.RemoveCasualties(Math.Max(1, (player.Army.Count + 1) / 2));
            player.AddResource(ResourceNames.Population, -12);
            player.AddResource(ResourceNames.Food, -35);
            Console.WriteLine($"Yenilgi! {casualties} asker kaybettiniz; -12 nüfus, -35 gıda.");
        }
    }
}
