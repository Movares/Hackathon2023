# Hackathon 2023
In deze repository vind je de bestanden voor de IMX Integratie Hackathon.

## Situatie
De volgende situatie doet zich voort. Er wordt een IMX geleverd door ProRail waarmee een bepaald ingenieurs bureau zijn ontwerp op gaat uitwerken. Tijdens het ontwerpen wordt een object (bijvoorbeeld sein “XYZ”) aangepast omdat deze foutief geplaatst is. Terwijl dit gebeurt wordt een nieuwe IMX geleverd waarin dit sein is verbeterd. 

![Image](https://github.com/Movares/Hackathon2023/blob/main/integratie.png?raw=true)
 
Dit probleem kan je op verschillende manieren oplossen. De eerste twee opdrachten zijn bedoeld om je op gang te helpen.
De opdracht moet je vervolgens uitvoeren op de volgende bestanden:
-	“Ontwerp IMX.xml”
-	“Uitlever IMX.xml”

## Vereisten
Voor de volgende opdracht heb je een beetje kennis nodig van IMX en van railverkeerstechniek of treinbeveiliging. Daarnaast is het handig als je een script / programma schrijft waarmee je een IMX kan analyseren. (Maar een duidelijke flowchart is ook een optie Met als doel om de jury te overtuigen)

### Opdracht 1 - objecten matchen met puic (max 1 sterren per object)
Vindt dezelfde objecten die voorkomen in de IMX bestanden op basis van PUICS.

**Uitkomst**: lever een lijst met alle PUICS die dubbel voorkomen.

**Toelichting**: dit is van belang om twee dataleveringen met elkaar te vergelijken, waarbij identieke objecten die zelfstandig worden vastgelegd aan elkaar gekoppeld worden. Een voorbeeld hiervan bestaat uit de Signal’s.

### Opdracht 2 - objecten matchen met andere attributen (max 10 sterren per object)
Zoek dezelfde objecten die voorkomen in de IMX bestanden, maar zelf geen puic hebben. Zoek hiervoor op basis van andere attributen zoals bijvoorbeeld de puic van een ouderobject, refs (puic referenties) of names. Leg hierbij wel goed vast welke problemen je eventueel kan tegenkomen. 

**Uitkomst**: beschrijf je methode en lever een lijst van alle objecten die dubbel voorkomen 

**Toelichting**: dit is van belang om kindelementen en relevante objecten die wel uniek zijn, maar geen unieke identificatie puic hebben, met elkaar te vergelijken. Voorbeelden hiervan zijn de ReflectorPost’s of de MicroLink’s.

### Opdracht 3 - objecten matchen zonder ids (zoals PUICs of refs) te gebruiken (max sterren per object)
Overleg met je team leden hoe je nog meer objecten kan matchen zonder een PUIC,-ref of name attribuut te gebruiken. Hierbij is het belangrijk om goed uit te leggen waarom je met bepaalde attributen een match kan maken. 

**Uitkomst**: beschrijf je methode en lever een lijst van alle objecten die dubbel voorkomen 

**Toelichting**: dit is ook erg van belang als objecten via twee verschillende processen / bronnen worden gegenereerd. Op basis van overeenkomstige (fysieke) eigenschappen kunnen objecten die in beide processen zijn ontworpen alsnog geïdentificeerd worden, zodat na integratie één ontworpen, geïntegreerd product kan overblijven.
