# Bonusaufgabe:
# Regeln:
# Kein "händisches" Zählen der Indexpositionen erlaubt
# Keine Künstliche Intelligenz
# Suchmaschine deiner Wahl erlaubt

# String
text = "My Python is better than your Python"

print("\nAufgabe 1")


# Finde den Index des Buchtabens "b"
for index, value in enumerate(text):
    if value == "b":
        print(index)

print("\nAufgabe 2")
# Kopiere das Word "better" aus dem Text und weise es einer neuen Variable zu.
if "better" in text:
    word = "better"
print(word)

print("\nAufgabe 3")
# Ändere alle "t"s zu "f"
new_text = text.replace("t", "f")
print(new_text)

print("\nAufgabe 4")
# Ändere den Datentyp des Textes in eine Liste
text = text.split()
print(text)

print("\nAufgabe 5")
# Mach aus der Liste wieder einen String
text = " ".join(text)
print(text)

print("\nAufgabe 6")
# Tausche die Position der beiden Buchstaben in dem Wort "is"
letter = list(text)
for i in range(len(letter)):
    if letter[i] == "i":
        letter[i] = "s"
        continue
    if letter[i] == "s":
        letter[i] = "i"
text = "".join(letter)
print(text)


print("\nAufgabe 7")
# Ändere jeden Anfangbuchstaben eines Wortes zu einem Großbuchstaben
new_text = text.split()
text = ""
for value in new_text:
    text = text + " " + value.capitalize()
text = text.strip()
print(text)

print("\nAufgabe 8")
# Entferne alle Vokale aus dem text
new_text = ""
letters = []
for letter in text:
    if letter.lower() not in "aeiou":
        letters.append(letter)

new_text = "".join(letters)
print(new_text)


print("\nAufgabe 9")
email = "karl@example.org"
# Filtere die Domain heraus und weise sie einer neuen Variable zu.
domain = ""
for i in range(len(email)):
    if email[i] == "@":
        domain = email[i + 1 :]
print(domain)

print("\nAufgabe 10")
# Rotiere das Wort python thonpy.
word = "python"
new_word = word[2:] + word[0:2]
print(new_word)

print("\nAufgabe 11")
# Erstelle eine Akronym aus den Anfangsbuchstaben jedes Wortes.
text_two = "You Only Live Once"
words = text_two.split()
akronym = ""
for word in words:
    akronym += word[0]

print(akronym)


print("\nAufgabe 12")
my_url = "http://www.example.org"
# Überprüfe, ob die Url mit "http://" beginnt.
if "http://" in my_url:
    print("Yes")
else:
    print("No")

print("\nAufgabe 13")
# Filtere die Zahl heraus und wandle sie in ein float um.
price = "The total is 45.99€"
number = ""
for letter in price:
    if letter.isdigit() or letter == ".":
        number += letter
number = float(number)
print(number)


# Lists
colours = ["red", "green", "blue", "yellow"]

print("\nAufgabe 14")
# Ändere "blue" zu "cyan"
colours[colours.index("blue")] = "cyan"
print(colours)

print("\nAufgabe 14")
# Tausche das erste Element mit dem letzten Element
colours[0], colours[-1] = colours[-1], colours[0]
print(colours)

print("\nAufgabe 14")
# Füge "magenta" ans Ende hinzu
colours.append("magenta")
print(colours)

print("\nAufgabe 15")
# Check ob "green" in der Liste ist
print("green" in colours)

print("\nAufgabe 16")
# Entferne "green" aus der Liste.
colours.remove("green")
print(colours)

print("\nAufgabe 17")
# Erstelle eine neue Liste mit den letzten drei Element aus "colours" in umgekehrte Reihenfolge
new_colours = colours[-1:0:-1]
print(new_colours)

person = {"firstname": "karl", "lastname": "karlsen", "age": 30}

print("\nAufgabe 18")
# Füge einen neuen Key zu person hinzu
person.update({"job": "software engineer"})
print(person)

print("\nAufgabe 19")
# Überprüfe, ob "age" als key in person existiert
print("age" in person)

print("\nAufgabe 20")
string_one = "abrakadabra"
# Nutze ein Dictionary um die Häufigkeit jedes einzelnen Buchstaben innerhalb "string_one" zu ermitteln.
string_two = {}
for i in string_one:
    if i in string_two:
        string_two[i] += 1
    else:
        string_two[i] = 1

print(string_two)


print("\nAufgabe 21")
# Nimm die folgende Listen und baue daraus ein Dictionary. Die Element aus first_list sind die keys und die Elemente aus second_list die values.
first_list = [1, 2, 3]
second_list = ["A", "B", "C"]
for


print("\nAufgabe 22")
# Schreibe eine Funktion, die uns den Pfad innerhalb dieses Dateisystems zum Ordner "python" wiedergibt.
# Lösung: home/user/desktop/solutions/python

file_system = {
    "home": {
        "user": {
            "projects": [""],
            "desktop": {
                "private": {"pictures": ["a.jpg", "b.jpg", "c.jpg"]},
                "job": {"tasks": ["task_a", "task_b"]},
                "solutions": {"python": ["two_sum"]},
            },
        }
    }
}
