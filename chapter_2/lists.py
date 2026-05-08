#Aufgabe 1
print("\n===Aufgabe 1===")
my_list = ["apple", "banana", "orange", "grape", "strawberry"]
print(my_list)

#Aufgabe 2
print("\n===Aufgabe 2===")
print(my_list[0])
print(my_list[-1])
print(my_list[-2])

#Aufgabe 3
print("\n===Aufgabe 3===")
print([my_list[1:3]])
print(my_list[:2])
print(my_list[2:])

#Aufgabe 4
print("\n===Aufgabe 4===")
if "apple" in my_list:
    print("Yes, apple is in my list.")

#Aufgabe 5
print("\n===Aufgabe 5===")
my_list.append("mango")
print(my_list)
my_list.insert(2, "blueberry")
print(my_list)

#Aufgabe 6
print("\n===Aufgabe 6===")
my_list[0] = "pineapple"
print(my_list)
my_list[1:3] = ["watermelon", "peach"]
print(my_list)

#Aufgabe 7
print("\n===Aufgabe 7===")
my_list.remove("watermelon")
print(my_list)
my_list.pop(1)
print("Without peach: ", my_list)
temporary_list = my_list[:]
print("Temporary list: ", temporary_list)
temporary_list.clear()
print("Temporary list after clear: ", temporary_list)

#Aufgabe 8
print("\n===Aufgabe 8===")
copy_list = my_list.copy()
my_list.insert(1, "peach")
print("my_list: ", my_list)
print("copy_list: ", copy_list)

#Aufgabe 9
print("\n===Aufgabe 9===")
list_1 = [1, 2, 3]
list_2 = [4, 5, 6]
list_3 = list_1 + list_2
list_1.extend(list_2)
print(list_3)
print(list_1)

#Aufgabe 10
print("\n===Aufgabe 10===")
new_list = [3, 5, 8, 6, 1]
new_list.sort()
print(new_list)
new_list.reverse()
print(new_list)
sorted_list = sorted(new_list)
print(sorted_list)

#Aufgabe 11
print("\n===Aufgabe 11===")
print("grape is in my_list " + str(my_list.count("grape")) + " times")
print("index for mango is:", my_list.index("mango"))

#Aufgabe 12
print("\n===Aufgabe 12===")
upper_case_list = [fruits.upper() for fruits in my_list if fruits[0] == "s"]
print(upper_case_list)