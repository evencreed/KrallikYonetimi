namespace KrallikYonetimi.Core;

public static class ConsoleInput
{
    public static int ReadChoice(string prompt, int min, int max, int onEndOfInput)
    {
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();
            if (text is null)
            {
                Console.WriteLine("Girdi akışı kapandı; varsayılan seçim kullanıldı.");
                return onEndOfInput;
            }

            if (int.TryParse(text.Trim(), out int choice) && choice >= min && choice <= max)
                return choice;

            Console.WriteLine($"Geçersiz giriş! {min} ile {max} arasında bir sayı yazın.");
        }
    }

    public static string ReadKingdomName()
    {
        Console.Write("Krallığınızın adı (boşsa Anadolu): ");
        string? name = Console.ReadLine();
        return string.IsNullOrWhiteSpace(name) ? "Anadolu" : name.Trim()[..Math.Min(name.Trim().Length, 30)];
    }
}
