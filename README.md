Na vrátnici budovy je počítač, ve kterém běží program umožňující sledovat a ovládat různá
zařízení v budově, která jsou zapojena do systému. Mezi taková zařízení patří například dveře, led
panely zobrazující textová hlášení, reproduktory, které mohou hrát hudbu, nebo poplašný signál, a
čtečky karet.

Napište takový program v C++ (nejlépe podle standardu C++17), nebo C#, který bude při
svém běhu udržovat v paměti reprezentaci stromové struktury různých zařízení. Strom bude mít tři
úrovně: kořen, skupiny zařízení a jednotlivá zařízení ve skupinách. Každá skupina může obsahovat
zařízení různých typů. Za běhu programu bude možné přidávat a odebírat skupiny, přidávat a
odebírat zařízení do/ze skupiny a měnit hodnoty vlastností zařízení (kromě vlastnosti Type).

Vlastnosti a funkce společné pro zařízení libovolného typu budou:
  - vlastnost Type, která bude mít konstantní hodnotu závislou na konkrétní implementaci
zařízení
  - vlastnost Id, která musí být mezi zařízeními ve stromu unikátní
  - vlastnost Name, kterou bude možné za běhu změnit
  - (virtuální) funkce GetCurrentState vracející textový popis aktuálního stavu
Vlastnosti specifické pro určitý typ zařízení:
  - Zařízení typu LedPanel bude mít vlastnost Message typu textový řetězec (string).
  - Zařízení typu Door bude mít vlastnosti Locked, Open, OpenForTooLong, OpenedForcibly
typu bool a vlastnost State výčtového typu. Pro vlastnosti Locked, Open,
OpenForTooLong a OpenedForcibly nebude vyhrazen žádný paměťový prostor, ale
budou se číst z a ukládat do jednotlivých bitů vlastnosti State.
  - Zařízení typu Speaker bude mít vlastnost Sound, která bude nabývat hodnot: None,
Music, Alarm, a dále vlastnost Volume typu číslo s plovoucí řádovou čárkou
  - Zařízení typu CardReader bude mít vlastnost AccessCardNumber typu string. Při pokusu
o zápis její hodnoty nejprve proběhne kontrola, zdali je délka řetězce sudá, ne větší než
16, a zda obsahuje pouze hexadecimální číslice. Byl-li předán nevyhovující řetězec, dojde
k chybě, jinak se předá transformační funkci ReverseBytesAndPad a její výsledek se uloží
jako hodnota vlastnosti AccessCardNumber. ReverseBytesAndPad je funkce jejíž jediný
parametr i návratová hodnota jsou typu string, která převrátí pořadí bytů (tedy dvojic
znaků, pořadí znaků ve dvojicích zůstane zachováno) v řetězci, a pokud je kratší než 16
znaků, doplní ho zleva samými nulami do délky 16. Př.: "A01234DE7FFF" ->
"0000FF7FDE3412A0"

Po každé změně vlastnosti zařízení se na standardní výstup vypíše textový popis aktuálního stavu
změněného zařízení. Po každé změně struktury stromu se standardní výstup vypíše textová
reprezentace celého stromu a jeho prvků. Za výpisy po změnách nebude zodpovědná část kódu, která
změnu vyvolala, ale stane se tak v rámci obsluhy události změny.

Program po spuštění vytvoří strom s několika skupinami, do kterých vloží několik instancí různých
zařízení, a následně po malých časových prodlevách provede různé operace: přidání, smazání,
přejmenování zařízení, přesun zařízení z jedné skupiny do jiné, změnu textu na LedPanelu,
zamknutí/odemknutí dveří, změnu zvuku reproduktoru. Poté přejde do interaktivního režimu, v němž
uživatel z klávesnice může zadávat příkazy umožňující provádět výpis celého stromu, všechny druhy
manipulace se zařízeními i skupinami a ukončení programu.
