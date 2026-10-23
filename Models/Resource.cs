namespace KrallikYonetimi.Models;

public class Resource
{
    public string Name { get; }
    public int Amount { get; private set; }

    public Resource(string name, int amount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Kaynak adı boş olamaz.", nameof(name));
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Kaynak negatif olamaz.");

        Name = name;
        Amount = amount;
    }

    public void Increase(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        // Taşma durumunda üst sınırda kalır.
        Amount = (int)Math.Min(int.MaxValue, (long)Amount + amount);
    }

    public bool TryDecrease(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (Amount < amount)
            return false;

        Amount -= amount;
        return true;
    }

    public int DecreaseUpTo(long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        int decreased = (int)Math.Min(Amount, amount);
        Amount -= decreased;
        return decreased;
    }
}
