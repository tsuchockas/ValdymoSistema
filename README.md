PostgreSQL windows versiją galima atsisiųsti iš: https://www.postgresql.org/download/windows/
Atsisiųsti .Net Core programinės įrangos kurimo komplektą iš https://dotnet.microsoft.com/download
Įsirašyti .Net Core karkasą
Paleisti "publish.bat" failą, jis sukurs projekto išeities aplanką (Publish\ValdymoSistema\netcoreapp_win10-x64), kuriame bus ".exe" failas ir sistemą bus galima paleisti kompiuteriuose, kurie neturi .Net Core prog. įrangos

Norint užtikrinti sklandų veikimą, pirmiausia turi būti paleista valdymo sistema ir po to paleidžiama jungiklio programa.
Prieš paleidžiant sistemą, reikia pakoreguoti "ConnectionStrings\DefaultConnection" lauką. Parašyti duomenų bazės adresą (Host), pavadinimą (database), prisijungimo duomenis (Username, Password)
SMTP ir MQTT serverių duomenų nekeisti.
Administratoriaus ir darbuotojo paskyrų duomenis nerekomenduojama keisti.
Pakeisti operatoriaus adresą į tokį, kuris gali gauti el. laiškus (vienkartinių el. pašto dėžučių įrankis - https://temp-mail.org/en)
Šviestuvų duomenys yra perskaitomi iš "SeedData.json" failo ir sugeneruojami duomenų bazės įrašai. 
Šviestuvų jungimo įvykiai yra sugeneruojami atsitiktinai visiems šviestuvams duomenų bazėje paleidimo metu. Įvykiai generuojami nuo "2021-05-11 08:00" laiko.