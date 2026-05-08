#Aufgabe 1
print("\n===Aufgabe 1===")
fruits = {"date", "apple", "orange", "elderberry", "strawberry"}
print(fruits)

#Aufgabe 2
print("\n===Aufgabe 2===")
if "apple" in fruits:
    print("Yes, apple is in fruits.")
else:
    print("No, apple isn't in fruits")

#Aufgabe 3
print("\n===Aufgabe 3===")
fruits.add("grape")
print(fruits)
more_fruits = {"date", "apple", "pear", "mango", "pineapple"}
fruits.update(more_fruits)
print(fruits)

#Aufgabe 4
print("\n===Aufgabe 4===")
fruits.remove("grape")
print(fruits)
fruits.discard("grape")
fruit = fruits.pop()
print("Removed element:", fruit)
print("Fruits set after pop:", fruits)
copy_of_fruits = fruits.copy()
print("Copy of fruits:", copy_of_fruits)
copy_of_fruits.clear()
print("Copy of fruits after clear:", copy_of_fruits)

#Aufgabe 5
print("\n===Aufgabe 5===")
set_a = {"date", "apple", "pear", "kiwi", "melon"}
set_b = {"kiwi", "melon", "peach", "orange"}
set_c = set_a.union(set_b)
print("Union:", set_c)
set_c = set_a.intersection(set_b)
print("Intersection:", set_c)
set_c = set_a.difference(set_b)
print("Difference:", set_c)
set_c = set_a.symmetric_difference(set_b)
print("Symmetric_difference:", set_c)

#Aufgabe 6
print("\n===Aufgabe 6===")
set_a.difference_update(set_b)
print("difference_update:", set_a)
set_a = {"date", "apple", "pear", "kiwi", "melon"}
set_a.intersection_update(set_b)
print("intersection_update:", set_a)
set_a = {"date", "apple", "pear", "kiwi", "melon"}
set_b = {"kiwi", "melon", "peach", "orange"}
print("set_a:", set_a)
print("set_b", set_b)
set_a.update(set_b)
print("Update:", set_a)

#Aufgabe 7
print("\n===Aufgabe 7===")
print("set_a:", set_a)
set_b = {"pear", "melon", "kiwi"}
print("set_b:", set_b)
print(set_b.issubset(set_a))
print(set_a.issuperset(set_b))
print(set_a.isdisjoint(set_b))