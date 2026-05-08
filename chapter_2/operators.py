#Aufgabe 1
print("\n===Aufgabe 1===")
a = 15
b = 4
print("Addition:", a + b)
print("Subtraction:", a - b)
print("Multiplication:", a * b)
print("Division:", a / b)
print("Floor Division:", a // b)
print("Modulus:", a % b)
print("Exponentiation:", a ** b)

#Aufgabe 2
print("\n===Aufgabe 2===")
x = 10
x += 5
print("x after += 5:", x)
x -= 3
print("x after -= 3:", x)
x *= 2
print("x after *= 2:", x)
x /= 4
print("x after /= 4:", x)

#Aufgabe 3
print("\n===Aufgabe 3===")
print("a == b:", a == b)
print("a != b:", a != b)
print("a > b:", a > b)
print("a < b:", a < b)
print("a >= b:", a >= b)
print("a <= b:", a <= b)

#Aufgabe 4
print("\n===Aufgabe 4===")
is_python_fun = True
is_java_fun = False

print("True and False:", is_python_fun and is_java_fun)
print("True or False:", is_python_fun or is_java_fun)
print("not True:", not is_python_fun)
print("not False:", not is_java_fun)


#Aufgabe 5
print("\n===Aufgabe 5===")
list1 = [1, 2, 3]
list2 = list1
list3 = [1, 2, 3]

print("list1 is list2:", list1 is list2)
print("list1 is not list2:", list1 is not list2)
print("list1 is list3:", list1 is list3)


#Aufgabe 6
print("\n===Aufgabe 6===")
text = "Python programming is fun!"

print("Python" in text)
print("Java" not in text)


#Aufgabe 7
print("\n===Aufgabe 7===")
a = 5
b = 3

print("Bitwise AND:", a & b)
print("Bitwise OR:", a | b)
print("Bitwise XOR:", a ^ b)
print("Left shift:", a << 1)
print("Right shift:", b >> 1)


#Aufgabe 8
print("\n===Aufgabe 8===")
print("Without parentheses:", 2 + 3 * 2 ** 2)
print("With parentheses:", (2 + 3) * 2 ** 2)