using System.Text;
using KrallikYonetimi.Buildings;
using KrallikYonetimi.Events;
using KrallikYonetimi.Models;
using KrallikYonetimi.Units;

namespace KrallikYonetimi.Core;

public class Game
{
    private const int MaxTurns = 20;
    private readonly Manager<GameEvent> _events = new();
    private Player _player = new("Anadolu");
    private int _currentTurn = 1;
    private int _completedTurns;
    private int _zeroFoodTurns;
    private int _buildingsConstructed;
    private bool _taxCollectedThisTurn;
    private bool _bankrupt;
    private bool _finished;
    private string _finalMessage = "";

    public Game()
    {
        _events.Add(new DroughtEvent());
        _events.Add(new EnemyAttackEvent());
        _events.Add(new MigrationEvent());
        _events.Add(new PlagueEvent());
        _events.Add(new HarvestEvent());
        _events.Add(new MerchantEvent());
    }

    public void Start()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("============================================");
        Console.WriteLine("      KRALLIK YÖNETİMİ — STRATEJİ OYUNU");
        Console.WriteLine("============================================");
        Console.WriteLine($"Amaç: {MaxTurns} tur boyunca krallığınızı ayakta tutun.");
        _player = new Player(ConsoleInput.ReadKingdomName());

        while (!_finished)
        {
            _player.ResetTurnModifiers();
            _taxCollectedThisTurn = false;

            ShowStatus();
            GameEvent gameEvent = _events.GetAll()[Random.Shared.Next(_events.Count)];
            gameEvent.Execute(_player);
            CheckImmediateLoss();
            if (_finished)
                break;

            ActionMenu();
            if (_finished)
                break;

            EndTurn();
            _completedTurns = _currentTurn;
            CheckTurnEndOutcome();
            if (!_finished)
                _currentTurn++;
        }

        ShowFinalScreen();
    }

    private void ShowStatus()
    {
        Console.WriteLine($"\n========== TUR {_currentTurn} / {MaxTurns} ==========");
        Console.WriteLine($"Krallık: {_player.Name}");
        ShowResources();
        Console.WriteLine($"Binalar: {GetBuildingSummary()}");
        Console.WriteLine($"Ordu: {GetArmySummary()}");
        Console.WriteLine("--------------------------------------------");
    }

    private void ShowResources()
    {
        Console.WriteLine($"Altın: {_player.GetResource(ResourceNames.Gold)}  " +
            $"Gıda: {_player.GetResource(ResourceNames.Food)}  " +
            $"Nüfus: {_player.GetResource(ResourceNames.Population)}  " +
            $"Askeri Güç: {_player.Army.TotalPower}");
    }

    private string GetBuildingSummary()
    {
        return _player.Buildings.Count == 0
            ? "Henüz bina yok"
            : string.Join(", ", _player.Buildings.GetAll()
                .Select(building => $"{building.Name} (Sv.{building.Level})"));
    }

    private string GetArmySummary()
    {
        if (_player.Army.Count == 0)
            return "Henüz asker yok";

        return string.Join(", ", _player.Army.Units
            .GroupBy(unit => unit.Name)
            .Select(group => $"{group.Count()} {group.Key}"));
    }

    private void ActionMenu()
    {
        bool endTurn = false;
        while (!endTurn && !_finished)
        {
            Console.WriteLine("\n1) Bina inşa et");
            Console.WriteLine("2) Asker eğit");
            Console.WriteLine("3) Vergi topla");
            Console.WriteLine("4) Pazar / Ticaret");
            Console.WriteLine("5) Krallık raporu");
            Console.WriteLine("6) Turu bitir");
            Console.WriteLine("0) Oyundan çık");
            int choice = ConsoleInput.ReadChoice("Seçiminiz: ", 0, 6, 0);

            switch (choice)
            {
                case 1:
                    BuildingMenu();
                    break;
                case 2:
                    TrainingMenu();
                    break;
                case 3:
                    CollectTaxes();
                    break;
                case 4:
                    MarketMenu();
                    break;
                case 5:
                    ShowReport();
                    break;
                case 6:
                    endTurn = true;
                    break;
                case 0:
                    _finished = true;
                    _finalMessage = "Oyuncu oyundan çıktı.";
                    break;
                default:
                    Console.WriteLine("Geçersiz menü seçimi.");
                    break;
            }

            CheckImmediateLoss();
        }
    }

    private void BuildingMenu()
    {
        Console.WriteLine("\n--- İNŞAAT ---");
        Console.WriteLine("1) Çiftlik (120 altın, 20 gıda) | Her tur +75 gıda/seviye");
        Console.WriteLine("2) Altın Madeni (180 altın, 20 gıda) | Her tur +80 altın/seviye");
        Console.WriteLine("3) Ev (140 altın, 30 gıda) | Her tur +5 nüfus/seviye");
        Console.WriteLine("4) Kışla (220 altın, 40 gıda) | Asker eğitimini açar");
        Console.WriteLine("5) Mevcut bir binayı yükselt (en çok Sv.5)");
        Console.WriteLine("0) Geri dön");
        int choice = ConsoleInput.ReadChoice("Seçiminiz: ", 0, 5, 0);

        Building? building = null;
        switch (choice)
        {
            case 1:
                building = new Farm();
                break;
            case 2:
                building = new GoldMine();
                break;
            case 3:
                building = new House();
                break;
            case 4:
                building = new Barracks();
                break;
            case 5:
                UpgradeBuilding();
                return;
            case 0:
                return;
            default:
                Console.WriteLine("Bilinmeyen bina.");
                return;
        }

        if (_player.Spend(building.Cost))
        {
            _player.Buildings.Add(building);
            _buildingsConstructed++;
            Console.WriteLine($"{building.Name} inşa edildi!");
        }
        else
        {
            Console.WriteLine($"Kaynak yetersiz. Eksik: {_player.MissingResources(building.Cost)}");
        }
    }

    private void UpgradeBuilding()
    {
        if (_player.Buildings.Count == 0)
        {
            Console.WriteLine("Yükseltilecek bina yok.");
            return;
        }

        Console.WriteLine("\n--- BİNA YÜKSELTME ---");
        for (int i = 0; i < _player.Buildings.Count; i++)
        {
            Building b = _player.Buildings.GetAll()[i];
            string price = b.Level < 5 ? FormatCost(b.GetUpgradeCost()) : "Azami seviye";
            Console.WriteLine($"{i + 1}) {b.Name} (Sv.{b.Level}) | {price}");
        }
        Console.WriteLine("0) Geri dön");
        int index = ConsoleInput.ReadChoice("Bina numarası: ", 0, _player.Buildings.Count, 0);
        if (index == 0)
            return;

        Building selected = _player.Buildings.GetAll()[index - 1];
        if (selected.Level >= 5)
            Console.WriteLine("Bu bina zaten en yüksek seviyede.");
        else if (!_player.HasEnough(selected.GetUpgradeCost()))
            Console.WriteLine($"Kaynak yetersiz. Eksik: {_player.MissingResources(selected.GetUpgradeCost())}");
        else if (selected.Upgrade(_player))
            Console.WriteLine($"{selected.Name} Sv.{selected.Level} oldu!");
    }

    private void TrainingMenu()
    {
        if (!_player.CanTrain)
        {
            Console.WriteLine("Asker eğitmek için önce kışla inşa etmelisiniz.");
            return;
        }

        Console.WriteLine("\n--- ASKER EĞİTİMİ ---");
        Console.WriteLine("1) Piyade (35 altın, 10 gıda, 1 nüfus) | Güç: 20");
        Console.WriteLine("2) Okçu (55 altın, 15 gıda, 1 nüfus) | Güç: 25");
        Console.WriteLine("3) Şövalye (90 altın, 25 gıda, 2 nüfus) | Güç: 60");
        Console.WriteLine("0) Geri dön");
        int choice = ConsoleInput.ReadChoice("Seçiminiz: ", 0, 3, 0);

        Unit? unit = null;
        switch (choice)
        {
            case 1:
                unit = new Soldier();
                break;
            case 2:
                unit = new Archer();
                break;
            case 3:
                unit = new Knight();
                break;
            case 0:
                return;
            default:
                return;
        }

        int populationCost = unit.Cost.GetValueOrDefault(ResourceNames.Population);
        if (_player.GetResource(ResourceNames.Population) <= populationCost)
        {
            Console.WriteLine("Krallıkta en az 1 sivil kalmalı; asker eğitimi yapılamadı.");
        }
        else if (_player.Spend(unit.Cost))
        {
            _player.Army.AddUnit(unit);
            Console.WriteLine($"1 {unit.Name} eğitildi. Askeri güç: {_player.Army.TotalPower}");
        }
        else
        {
            Console.WriteLine($"Kaynak yetersiz. Eksik: {_player.MissingResources(unit.Cost)}");
        }
    }

    private void CollectTaxes()
    {
        if (_taxCollectedThisTurn)
        {
            Console.WriteLine("Her tur yalnızca bir kez vergi toplanabilir.");
            return;
        }

        int income = Math.Max(50, _player.GetResource(ResourceNames.Population));
        _player.AddResource(ResourceNames.Gold, income);
        _player.AddResource(ResourceNames.Population, -3);
        _taxCollectedThisTurn = true;
        Console.WriteLine($"+{income} altın toplandı; ağır vergi nedeniyle -3 nüfus.");
    }

    private void MarketMenu()
    {
        Console.WriteLine("\n--- PAZAR ---");
        Console.WriteLine("1) 40 altın karşılığında 60 gıda al");
        Console.WriteLine("2) 70 gıda sat, 35 altın kazan");
        Console.WriteLine("0) Geri dön");
        int choice = ConsoleInput.ReadChoice("Seçiminiz: ", 0, 2, 0);

        switch (choice)
        {
            case 1:
                if (_player.Spend(new Dictionary<string, int> { [ResourceNames.Gold] = 40 }))
                {
                    _player.AddResource(ResourceNames.Food, 60);
                    Console.WriteLine("60 gıda satın alındı.");
                }
                else
                    Console.WriteLine("Alım için en az 40 altın gerekli.");
                break;
            case 2:
                if (_player.Spend(new Dictionary<string, int> { [ResourceNames.Food] = 70 }))
                {
                    _player.AddResource(ResourceNames.Gold, 35);
                    Console.WriteLine("70 gıda satıldı, +35 altın.");
                }
                else
                    Console.WriteLine("Satış için en az 70 gıda gerekli.");
                break;
            case 0:
                break;
            default:
                Console.WriteLine("Geçersiz pazar seçimi.");
                break;
        }
    }

    private void ShowReport()
    {
        Console.WriteLine("\n========== KRALLIK RAPORU ==========");
        ShowResources();
        Console.WriteLine($"Binalar ({_player.Buildings.Count}): {GetBuildingSummary()}");
        Console.WriteLine($"Ordu ({_player.Army.Count} birim): {GetArmySummary()}");
        var upkeep = _player.Army.GetTotalUpkeep();
        Console.WriteLine($"Ordu bakım gideri / tur: {upkeep.Gold} altın, {upkeep.Food} gıda");
        Console.WriteLine($"Savaşlar: {_player.WonBattles} galibiyet / {_player.LostBattles} mağlubiyet");
        Console.WriteLine($"Ardışık sıfır gıda turu: {_zeroFoodTurns}/3");
        Console.WriteLine($"Bu turdaki üretim katsayıları: Gıda %{_player.FoodProductionPercent}, " +
            $"Altın %{_player.GoldProductionPercent}");
        Console.WriteLine("=====================================");
    }

    private void EndTurn()
    {
        Console.WriteLine("\n--- TUR SONU ---");
        Console.WriteLine("Bina üretimleri:");
        if (_player.Buildings.Count == 0)
            Console.WriteLine("  Üretim yapan bina yok.");

        // Çok biçimlilik: bina türüne bakmadan her binanın Produce metodu çağrılır.
        for (int i = 0; i < _player.Buildings.Count; i++)
            _player.Buildings.GetAll()[i].Produce(_player);

        var upkeep = _player.Army.GetTotalUpkeep();
        int population = _player.GetResource(ResourceNames.Population);
        int civilianFood = (population + 3) / 4;
        int totalFoodNeed = civilianFood + upkeep.Food;
        int currentFood = _player.GetResource(ResourceNames.Food);
        _player.AddResource(ResourceNames.Food, -totalFoodNeed);
        Console.WriteLine($"Gıda tüketimi: -{totalFoodNeed} (halk: {civilianFood}, ordu: {upkeep.Food})");

        if (currentFood < totalFoodNeed)
        {
            int deficit = totalFoodNeed - currentFood;
            int deaths = Math.Max(3, (deficit + 1) / 2);
            _player.AddResource(ResourceNames.Population, -deaths);
            Console.WriteLine($"Açlık! Yetersiz gıda nedeniyle {deaths} nüfus kaybı yaşandı.");
        }
        else if (_player.GetResource(ResourceNames.Food) >= 50)
        {
            _player.AddResource(ResourceNames.Population, 2);
            Console.WriteLine("Düzenli beslenme sayesinde +2 nüfus.");
        }

        int currentGold = _player.GetResource(ResourceNames.Gold);
        if (currentGold >= upkeep.Gold)
        {
            _player.AddResource(ResourceNames.Gold, -upkeep.Gold);
            Console.WriteLine($"Ordu bakım ödemesi: -{upkeep.Gold} altın.");
        }
        else
        {
            _player.AddResource(ResourceNames.Gold, -currentGold);
            _bankrupt = true;
            Console.WriteLine($"İFLAS! Ordu bakımı ({upkeep.Gold} altın) ödenemedi.");
        }

        if (_player.GetResource(ResourceNames.Food) == 0)
            _zeroFoodTurns++;
        else
            _zeroFoodTurns = 0;

        Console.WriteLine($"Gıda tükenme sayacı: {_zeroFoodTurns}/3");
        ShowResources();
    }

    private void CheckImmediateLoss()
    {
        if (!_finished && _player.GetResource(ResourceNames.Population) <= 0)
        {
            _finished = true;
            _finalMessage = "KAYBETTİNİZ: Krallığın nüfusu sıfıra düştü.";
        }
    }

    private void CheckTurnEndOutcome()
    {
        if (_player.GetResource(ResourceNames.Population) <= 0)
        {
            _finished = true;
            _finalMessage = "KAYBETTİNİZ: Krallığın nüfusu sıfıra düştü.";
        }
        else if (_zeroFoodTurns >= 3)
        {
            _finished = true;
            _finalMessage = "KAYBETTİNİZ: Üç tur boyunca gıda tükenince isyan çıktı.";
        }
        else if (_bankrupt)
        {
            _finished = true;
            _finalMessage = "KAYBETTİNİZ: Ödenemeyen asker bakım giderleri yüzünden iflas ettiniz.";
        }
        else if (_currentTurn >= MaxTurns)
        {
            _finished = true;
            _finalMessage = "KAZANDINIZ! Krallığınız 20 tur boyunca ayakta kaldı.";
        }
    }

    private void ShowFinalScreen()
    {
        long score = _player.GetResource(ResourceNames.Gold)
            + _player.GetResource(ResourceNames.Food)
            + 6L * _player.GetResource(ResourceNames.Population)
            + 10L * _player.Army.TotalPower
            + 80L * _player.Buildings.Count
            + 100L * _player.WonBattles
            - 75L * _player.LostBattles
            + 50L * _completedTurns;

        Console.WriteLine("\n============================================");
        Console.WriteLine("                OYUN SONU");
        Console.WriteLine("============================================");
        Console.WriteLine(_finalMessage);
        Console.WriteLine($"Krallık: {_player.Name}");
        Console.WriteLine($"Tamamlanan tur: {_completedTurns} / {MaxTurns}");
        Console.WriteLine($"Son oynanan tur: {_currentTurn}");
        ShowResources();
        Console.WriteLine($"Toplam bina sayısı: {_player.Buildings.Count}");
        Console.WriteLine($"Oyun boyunca inşa edilen bina: {_buildingsConstructed}");
        Console.WriteLine($"Kazanılan savaş: {_player.WonBattles}");
        Console.WriteLine($"Kaybedilen savaş: {_player.LostBattles}");
        Console.WriteLine($"Puan: {Math.Max(0L, score)}");
        Console.WriteLine("============================================");
    }

    private static string FormatCost(Dictionary<string, int> cost)
    {
        return string.Join(", ", cost.Select(kv => $"{kv.Value} {Player.Translate(kv.Key)}"));
    }
}
