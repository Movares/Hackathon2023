# Hackathon 2023
In deze repository vind je de bestanden voor de IMX Integratie Hackathon.

## Vereisten
Voor de volgende opdracht heb je een beetje kennis nodig van IMX en van railverkeerstechniek of trein beveiliging. Daarnaast is het handig als je een script / programma schrijft waarmee je een IMX kan analyseren. (Maar een duidelijke flowchart is ook een optie.)
Situatie
De volgende situatie doet zich voort. Er wordt een IMX geleverd door ProRail waarmee een bepaald ingenieurs bureau zijn ontwerp op gaat uitwerken. Tijdens het ontwerpen wordt een object (bijvoorbeeld sein “XYZ”) aangepast omdat deze foutief geplaatst is. Terwijl dit gebeurd wordt een nieuwe IMX geleverd waarin dit sein is verbeterd. 

![Image](https://github.com/Movares/Hackathon2023/blob/main/integratie.png?raw=true)
 
Dit probleem kan je op verschillende manieren oplossen. De eerste twee opdrachten zijn bedoeld om je op gang te helpen.
De opdracht moet je vervolgens uitvoeren op de volgende bestanden:
-	“Ontwerp IMX.xml”
-	“Uitlever IMX.xml”

### Opdracht 1 - objecten matchen met puic (1 punt per object)
Vind dezelfde objecten die voorkomen in de IMX bestanden op basis van PUICS.

Uitkomst: lever een lijst met alle PUICS die dubbel voorkomen.

### Opdracht 2 - objecten matchen met andere attributen (10 punten per object)
Verwijderd alle PUICS uit bestanden en zoek dezelfde objecten die voorkomen in de IMX bestanden op basis van andere attributen zoals bijvoorbeeld refs (puic referenties) of names. Leg hierbij wel goed vast welke problemen je eventueel kan tegenkomen. 

Uitkomst: beschrijf je methode en lever een lijst van alle objecten die dubbel voorkomen 

### Opdracht 3 - objecten matchen zonder ids (zoals PUICs of refs) te gebruiken (100 punten per object)
Verwijder alle PUICS, Refs en Names uit bestanden en overleg met je team leden hoe je nog meer objecten kan matchen zonder een PUIC,-ref of name attribuut te gebruiken. Hierbij is het belangrijk om goed uit te leggen waarom je met bepaalde attributen een match kan maken. 

Uitkomst: beschrijf je methode en lever een lijst van alle objecten die dubbel voorkomen 
