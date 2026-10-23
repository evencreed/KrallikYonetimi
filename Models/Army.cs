using KrallikYonetimi.Core;
using KrallikYonetimi.Units;

namespace KrallikYonetimi.Models;

public class Army
{
    // Generic sınıfın ikinci kullanımı: Manager<Unit>.
    private readonly Manager<Unit> _units = new();

    public int Count => _units.Count;
    public int TotalPower => _units.GetAll().Sum(unit => unit.GetPower());
    public IReadOnlyList<Unit> Units => _units.GetAll();

    public void AddUnit(Unit unit) => _units.Add(unit);

    public int RemoveCasualties(int count)
    {
        int removed = 0;
        for (int i = 0; i < count && _units.Count > 0; i++)
        {
            int index = Random.Shared.Next(_units.Count);
            Unit victim = _units.GetAll()[index];
            if (_units.Remove(victim))
                removed++;
        }
        return removed;
    }

    public (int Gold, int Food) GetTotalUpkeep()
    {
        int gold = 0;
        int food = 0;
        foreach (Unit unit in _units.GetAll())
        {
            gold += unit.UpkeepGold;
            food += unit.UpkeepFood;
        }
        return (gold, food);
    }
}
