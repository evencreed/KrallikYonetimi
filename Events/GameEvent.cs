using KrallikYonetimi.Core;
using KrallikYonetimi.Models;

namespace KrallikYonetimi.Events;

public abstract class GameEvent
{
    public string Title { get; }
    public string Description { get; }

    protected GameEvent(string title, string description)
    {
        Title = title;
        Description = description;
    }

    protected int AskChoice(string optionA, string optionB)
    {
        Console.WriteLine($"\n!!! OLAY: {Title.ToUpperInvariant()} !!!");
        Console.WriteLine(Description);
        Console.WriteLine($"1) {optionA}");
        Console.WriteLine($"2) {optionB}");
        return ConsoleInput.ReadChoice("Seçiminiz: ", 1, 2, 2);
    }

    public abstract void Execute(Player player);
}
