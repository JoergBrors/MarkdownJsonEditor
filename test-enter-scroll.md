# Test für Enter und Scroll

## Zeile 1
Dies ist die erste Zeile.

## Zeile 2
Dies ist die zweite Zeile.

## Zeile 3
Dies ist die dritte Zeile.

## Zeile 4
Dies ist die vierte Zeile.

## Zeile 5
Dies ist die fünfte Zeile.

## Zeile 6
Dies ist die sechste Zeile.

## Zeile 7
Dies ist die siebte Zeile.

## Zeile 8
Dies ist die achte Zeile.

## Zeile 9
Dies ist die neunte Zeile.

## Zeile 10
Dies ist die zehnte Zeile.

## Test-Anweisungen

1. Drücken Sie **Enter** zwischen den Zeilen - es sollte eine neue Zeile erstellt werden
2. Scrollen Sie im Preview nach unten
3. Ändern Sie etwas im Editor (z.B. Text hinzufügen)
4. Die Preview sollte **NICHT** nach oben springen, sondern an der gleichen Stelle bleiben

## Code-Block Test

```javascript
function test() {
    console.log("Zeile 1");
    console.log("Zeile 2");
    console.log("Zeile 3");
}
```

## Farbmarkierungen Test

==Gelbe Markierung==

<span style='background-color: #90EE90'>Grüne Markierung</span>

<span style='background-color: #FFB6C1'>Rote Markierung</span>

<span style='background-color: #ADD8E6'>Blaue Markierung</span>

## Tab-Test

Hier ein Tab:	<- Tab-Zeichen
	Eingerückter Text mit Tab
		Doppelt eingerückter Text

## Liste mit Einrückung

- Punkt 1
	- Unterpunkt 1.1 (mit Tab)
	- Unterpunkt 1.2 (mit Tab)
- Punkt 2
	- Unterpunkt 2.1 (mit Tab)

## Ende des Tests

Wenn Sie ganz nach unten scrollen und dann oben im Editor etwas ändern, sollte die Preview hier unten bleiben!
