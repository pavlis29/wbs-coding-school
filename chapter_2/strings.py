# Aufgabe 1
print("\n===Aufgabe 1===")
first_name = "Pavel"
last_name = "Garlik"
bio = """My name is 
Pavel Garlik. I'm learning
Python"""

#Aufgabe 2
print("\n===Aufgabe 2===")
print(first_name[0])
print(last_name[0])
print(bio[0:10])

#Aufgabe 3
print("\n===Aufgabe 3===")
for char in first_name:
    print(char)

#Aufgabe 4
print("\n===Aufgabe 4===")
print(len(bio))

#Aufgabe 5
print("\n===Aufgabe 5===")
print("Python" in bio)
print("Java" not in bio)

#Aufgabe 6
print("\n===Aufgabe 6===")
print(first_name.upper())
print(last_name.lower())
bio = bio.strip()
bio = bio.replace("Python", "coding")
print(bio.split())

#Aufgabe 7
print("\n===Aufgabe 7===")
full_name = first_name + " " + last_name
print(full_name)

#Aufgabe 8
print("\n===Aufgabe 8===")
print(f"Hello, my name is {full_name} and I love Python!")
print("My full name is {} and I am {} years old.".format("Pavel Garlik", 42))

#Aufgabe 9
print("\n===Aufgabe 9===")
print("He said 'Python\'s great'")

#Aufgabe 10
print("\n===Aufgabe 10===")
print(bio.center(50))
print(full_name.count("a"))