# Krallık Yönetimi — C# / .NET 8 Konsol Strateji Oyunu

Harici kütüphane gerektirmeyen, 20 turluk, sıra tabanlı kaynak yönetimi oyunu.

## Kurulum ve çalıştırma

.NET SDK 8 veya daha yenisini yükleyin. Terminalde projenin kök dizinine gelin:

```bash
dotnet run
```

Derlemek için:

```bash
dotnet build
```

Windows, macOS ve Linux üzerinde .NET 8 ile kullanılabilir. Tüm konsol metinleri Türkçedir.

## Oyun akışı

Her tur: (1) durum ekranı, (2) altı farklı olaydan rastgele seçilen biri, (3) olay kararı,
(4) tekrar açılan eylem menüsü, (5) bina üretimi / beslenme / ordu bakım ödemesi / nüfus artışı,
(6) kazanma-kaybetme kontrolü şeklinde ilerler.

Eylem menüsünden bina kurabilir, bina yükseltebilir, kışla kurduktan sonra asker eğitebilir,
vergi toplayabilir, gıda alıp satabilir, rapor görebilir, turu tamamlayabilir veya çıkabilirsiniz.
Her tur **en fazla bir defa** vergi toplanabilir; diğer eylemler kaynak oldukça tekrarlanabilir.

Başlangıç: **500 altın**, **300 gıda**, **100 nüfus**, bina ve asker yok.
Askeri güç bir kaynak olarak saklanmaz, doğrudan `Army.TotalPower` ile hesaplanır.

### Binalar

| Bina | Kurulum | Her tur üretim / etki |
|---|---|---|
| Çiftlik | 120 altın, 20 gıda | +75 gıda × seviye |
| Altın Madeni | 180 altın, 20 gıda | +80 altın × seviye |
| Ev | 140 altın, 30 gıda | +5 nüfus × seviye |
| Kışla | 220 altın, 40 gıda | Asker eğitimini açar |

Binalar 5. seviyeye kadar yükseltilebilir. Yükseltme maliyeti = kurulum maliyeti × (mevcut seviye + 1).
Olaylar o tur için gıda/altın üretimini azaltabilir. Bina üretimi OOP çok biçimliliği ile
`Building.Produce(Player)` çağrısı üzerinden hesaplanır.

### Birimler

| Birim | Eğitim maliyeti | Güç | Tur başına bakım |
|---|---|---:|---|
| Piyade | 35 altın, 10 gıda, 1 nüfus | 20 | 3 altın, 2 gıda |
| Okçu | 55 altın, 15 gıda, 1 nüfus | 25 | 4 altın, 2 gıda |
| Şövalye | 90 altın, 25 gıda, 2 nüfus | 60 | 7 altın, 4 gıda |

Şövalyenin `GetPower()` override'ı savunmasının yarısı kadar ek güç sağlar.
Nüfusun son kişisi eğitime alınamaz.

### Rastgele olaylar

`DroughtEvent`, `EnemyAttackEvent`, `MigrationEvent`, `PlagueEvent`, `HarvestEvent`,
`MerchantEvent`. Her biri iki farklı seçenek gösterir; yetersiz altın durumlarında güvenli
alternatif/sonuç uygulanır. Düşman gücü **25 ile 100 (dahil)** arasında rastgele seçilir.
`Army.TotalPower >= enemyPower` ise savaş kazanılır ve 80 altın alınır; kaybedilirse
mevcut askerlerin yaklaşık yarısı rastgele kaybedilir, ayrıca 12 nüfus ve 35 gıda gider.
Ordusuz savaş kaybında doğal olarak asker kaybı sıfırdır.

### Tur sonu ve oyun sonu

- Sivil gıda ihtiyacı: `tavan(nüfus / 4)`; ordunun bakım gıdası eklenir.
- Gıda ihtiyacı karşılanamazsa açlıktan en az 3 nüfus kaybı oluşur (açık arttıkça kayıp artar).
- Gıda ihtiyacı tamamen karşılanır ve tur sonunda en az 50 gıda kalırsa +2 doğal nüfus artar.
- Bina üretimi tüketim ve asker bakım ücretinden **önce** yapılır.
- **Kazanma:** 20. turun sonunu geçerli bir krallıkla bitirmek.
- **Kaybetme:** nüfusun sıfır olması, **ardışık 3 tur sonunda** gıdanın sıfır olması,
  veya asker bakım altını yetmediği için iflas etmek. Son olayda altın negatif tutulmaz:
  ödenmemiş bakım `_bankrupt` durumu olarak temsil edilir ve oyun kaybedilir.
- Rastgele savaş kaybetmek tek başına oyun sonu değildir; birim kaybı yaratır.
- Gıda ve nüfus negatif olmaz: kaynak azaltımı mevcut miktarda durdurulur.
- Oyuncunun erken çıkması kazanma/kaybetmeden ayrı raporlanır.

**Puan hesabı:** altın + gıda + 6 × nüfus + 10 × ordu gücü + 80 × bina sayısı
+ 100 × galibiyet − 75 × mağlubiyet + 50 × tamamlanan tur. Negatifse 0 gösterilir.

## OOP ve teknik gereksinimler

- **`Player`**: `Resources` (`Dictionary<string,int>`), `Buildings` (`Manager<Building>`),
  `Army` (composition); `HasEnough`, `Spend`, `AddResource`.
- **`Resource`**: `Name`, `Amount`, `Increase`, `TryDecrease`, `DecreaseUpTo`.
  **Kullanımı:** `Player.Spend` ve `Player.AddResource` içinde kaynak değerlerini güvenle
  değiştirirken kullanılır. Envanterin asıl depolama şekli `Dictionary<string,int>`'tir.
  `Player.Resources` dışarıya bir **kopya** verir; gerçek değerler yalnızca metotlarla değişir.
- **`Building` (abstract)**: `Name`, `Level`, `Cost`, `Produce` ve `Upgrade`;
  türevleri `Farm`, `GoldMine`, `House`, `Barracks`.
- **`Unit` (abstract)**: `Name`, `AttackPower`, `Defense`, `UpkeepGold`, `UpkeepFood`,
  `GetPower`; türevleri `Soldier`, `Archer`, `Knight`.
- **`Army`**: `Manager<Unit>` ile askerleri tutar, `TotalPower` hesaplanır;
  `AddUnit`, `RemoveCasualties`, `GetTotalUpkeep`.
- **`GameEvent` (abstract)**: `Title`, `Description`, `Execute(Player)`;
  6 ayrı somut olay sınıfı.
- **`Manager<T> where T : class`**: `Add`, `Remove`, `GetAll`, `Find`, `Count`.
  Hem `Manager<Building>` hem `Manager<Unit>` hem de `Manager<GameEvent>`
  olarak kullanılır; sadece tek tip için yazılmamıştır.
- **`Game`**: tur akışı, menüler, durum, rapor ve oyun sonunu yönetir.
- **`Program.cs`**: yalnızca `new Game().Start()` çağrısı.

`while` ana oyun ve eylem döngüsünde; `for` bina üretimi, yükseltme listesi ve
asker kaybında; `switch-case` inşaat, eğitim, pazar ve ana menüde; `if-else`
olaylarda, kaynak kontrollerinde ve oyun sonu kontrolünde kullanılmıştır.
Tüm sayısal kullanıcı girdileri `int.TryParse` ve sınır denetimi ile işlenir.
Boş/harfli/taşan sayı hatalı girişler kullanıcıya tekrar sorulur.

## Proje ağacı

```text
KrallikYonetimi/
├── KrallikYonetimi.csproj
├── Program.cs
├── README.md
├── Core/
│   ├── Game.cs
│   ├── Manager.cs
│   ├── ConsoleInput.cs
│   └── ResourceNames.cs
├── Models/
│   ├── Player.cs
│   ├── Resource.cs
│   └── Army.cs
├── Buildings/
│   ├── Building.cs
│   ├── Farm.cs
│   ├── GoldMine.cs
│   ├── House.cs
│   └── Barracks.cs
├── Units/
│   ├── Unit.cs
│   ├── Soldier.cs
│   ├── Archer.cs
│   └── Knight.cs
└── Events/
    ├── GameEvent.cs
    ├── DroughtEvent.cs
    ├── EnemyAttackEvent.cs
    ├── MigrationEvent.cs
    ├── PlagueEvent.cs
    ├── HarvestEvent.cs
    └── MerchantEvent.cs
```
