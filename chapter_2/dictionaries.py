# 1. Create and Print a Dictionary
# Create a dictionary representing a person with keys such as "name", "age", and "city".
# Print the entire dictionary to the screen.
print("\n===Aufgabe 1===")
person = {"name": "Pavel", "age": "42", "city": "Karlsbad"}
print(person)


# 2. Access Dictionary Elements
# Print the value associated with the key "name" using square brackets.
# Use the get() method to safely retrieve the value for a key that might not exist (e.g., "email"), providing a default value.
# Print all keys, values, and items of the dictionary using keys(), values(), and items() methods.
print("\n===Aufgabe 2===")
print(person["name"])
print(person.get("email", "default_value"))
print(person.keys())
print(person.values())
print(person.items())


# 3. Check for Key Existence
# Check if the key "age" exists in the dictionary using the in keyword.
# Print a message based on whether the key is found or not.
print("\n===Aufgabe 3===")
if "age" in person:
    print("Yes, 'key' is in person.")
else:
    print("No, 'key' isn't in person.")


# 4. Change and Update Dictionary Elements
# Update the value associated with "city" directly by assignment.
# Use the update() method to change multiple key-value pairs or add new ones (e.g., add "occupation": "Engineer").
print("\n===Aufgabe 4===")
person["city"] = "Wassertrüdingen"
print(person)
person.update({"age": "43", "occupation": "Engineer"})
print(person)


# 5. Add New Items to the Dictionary
# Add a new key-value pair (e.g., "country": "USA") using direct assignment.
# Use update() to add another new key-value pair (e.g., "hobby": "cycling").
print("\n===Aufgabe 5===")
person["country"] = "Germany"
person.update({"hobby": "music"})
print(person)


# 6. Remove Items from the Dictionary
# Remove an item by key using pop() and print the removed value.
# Use popitem() to remove the last inserted key-value pair, and print the pair removed.
# Delete a specific key-value pair using the del keyword.
# Clear the entire dictionary using clear() and print the empty dictionary.
print("\n===Aufgabe 6===")
removed_item = person.pop("age")
print("Removed item:", removed_item)
p = person.popitem()
print("Removed last inserted key-value pair:", p)
del person["city"]
print(person)
person.clear()
print(person)


# 7. Copy a Dictionary
# Create a shallow copy of your dictionary using the copy() method or dict() constructor.
# Modify the original dictionary and show that the copy remains unchanged.
print("\n===Aufgabe 7===")
person = {"name": "Pavel", "age": "42", "city": "Karlsbad"}
another_person = person.copy()
person["name"] = "Martin"
print("Person:", person)
print("Another person:", another_person)


# 8. Using setdefault()
# Use setdefault() to retrieve the value of a key that exists, and then for a key that doesn’t exist, adding it with a default value.
# Print the dictionary to observe changes.
print("\n===Aufgabe 8===")
age = person.setdefault("age", 50)
country = person.setdefault("country", "Germany")
print("setdefault existing key 'age':", age)
print("setdefault new key 'country':", country)
print("person after setdefault:", person)


