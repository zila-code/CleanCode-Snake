# Refaktoring konzolové hry Snake (C#)

Tento projekt obsahuje refaktorovanou verzi kódu pro klasickou hru Snake. Hlavním cílem bylo vyčistit původní nepřehledný kód a upravit ho podle principů Clean Code.

## Klíčové změny a úpravy

### 1. Oddělení logiky od GUI (Decoupling)
Původní kód míchal herní pravidla a vykreslování do konzole dohromady. Vytvořil jsem proto samostatnou třídu `SnakeEngine`, která řeší čistě jen herní logiku (výpočet souřadnic, posun hada, kolize). Tato třída záměrně neobsahuje žádné příkazy pro výpis do konzole. Vykreslování se řeší až v hlavní třídě `Program`. Díky tomu je logika nezávislá a hru by bylo možné snadno napojit na jiné uživatelské rozhraní.

### 2. Rozdělení do tříd a struktur
Celá hra byla původně napsaná v jediné dlouhé metodě `Main`. Logiku jsem rozdělil do menších celků s jasnou zodpovědností. Pro souřadnice jsem navíc vytvořil třídu `Position`, takže tělo hada se nyní udržuje v jednom přehledném listu namísto dvou zmatečných polí pro osy X a Y.

### 3. Bezpečnější typy (Enum)
Původní ukládání směru pohybu do textových řetězců (např. "UP", "DOWN") jsem nahradil výčtovým typem `Direction`. Kód je díky tomu bezpečnější a odpadá riziko chyb způsobených obyčejným překlepem.

### 4. Zpřehlednění herní smyčky
Odstranil jsem původní nepřehledné odpočítávání času. Nová herní smyčka je rozdělena do tří logických a snadno čitelných kroků:
1. `HandleInput` (načtení vstupu od uživatele)
2. `UpdateGameStep` (přepočítání stavu hry a posun hada)
3. `Render` (samotné vykreslení aktuálního stavu do konzole)

Rychlost hry je nyní jednoduše a srozumitelně řízena pomocí `Thread.Sleep()`.

### 5. Zlepšení čitelnosti a pojmenování
V kódu jsem nahradil nesrozumitelné a cizojazyčné názvy proměnných za ustálené anglické pojmy. Pevně zakódované číselné hodnoty, jako je velikost hracího pole, jsem přesunul do jasně pojmenovaných proměnných.

## Závěr
Výsledkem refaktoringu je kód, který je mnohem lépe čitelný, rozdělený do logických bloků a splňuje základní pravidla pro udržitelnost softwaru.