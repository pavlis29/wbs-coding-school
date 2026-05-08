fruits = ["apple", "banana", "cherry"]
print("apple" in fruits)
fruits.insert(2, "peach")
print(fruits)
another_list = ["car", "bus", "tram"]
element = fruits.pop(3)
print(type(element))
print(fruits)
fruits.clear()
my_numbers = [1, 2, 3, 4, 5]
for num in my_numbers:
    print(num)
    for inner_num in my_numbers:
        print(inner_num)


alphabet = ["a", "b", "c", "d", "e"]

alphabet[1], alphabet[3] = alphabet[3], alphabet[1]

print(alphabet)