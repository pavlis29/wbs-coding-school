#Aufgabe 1
print("\n===Aufgabe 1===")
my_tuple = ("car", "bike", 7, 3, 0)

#Aufgabe 2
print("\n===Aufgabe 2===")
print(my_tuple)

#Aufgabe 3
print("\n===Aufgabe 3===")
print(my_tuple[0])
print(my_tuple[-1])

#Aufgabe 4
print("\n===Aufgabe 4===")
print(my_tuple[1:3])
print(my_tuple[:3])
print(my_tuple[2:])

#Aufgabe 5
print("\n===Aufgabe 5===")
if "bike" in my_tuple:
    print("Yes, bike is in my_tuple.")
else:
    print("No, bike isn't in my_tuple")

#Aufgabe 6
print("\n===Aufgabe 6===")
bike_count = my_tuple.count("bike")
print("Bike is in my_tuple", bike_count, "times")
first_7_index = my_tuple.index(7)
print("The first occurance of number 7 is at index", first_7_index)

#Aufgabe 7
print("\n===Aufgabe 7===")
element1, element2, element3, element4, element5 = my_tuple
print(element5, element4, element3, element2, element1)
*words, number1, number2, number3 = my_tuple
print("words:", words)
print("numbers:", number1, number2, number3)

#Aufgabe 8
print("\n===Aufgabe 8===")
another_tuple = (1, 2, 3, 4, 5)
new_tuple = my_tuple + another_tuple
print(new_tuple)
multiplied_tuple = new_tuple * 3
print(multiplied_tuple)
