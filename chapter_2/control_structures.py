import random

# 1. Basic If Condition
# Write a block that checks if a number is positive, negative, or zero using simple if, elif, and else. You must declare any necessary variable(s).
print("\n===Aufgabe 1===")
number = 7
if number > 0:
    print(f"Number {number} is positive.")
elif number < 0:
    print(f"Number {number} is negative")
else:
    print(f"Number {number} is zero.")


# 2. Grade Calculator
# Given a score between 0 and 100, use if, elif, and else to determine and print the corresponding grade (A, B, C, D, or F).
print("\n===Aufgabe 2===")
score = 58
if score <= 20 and score >= 0:
    print(f"Corresponding grade for score {score} is F")
elif score <= 40:
    print(f"Corresponding grade for score {score} is D")
elif score <= 60:
    print(f"Corresponding grade for score {score} is C")
elif score <= 80:
    print(f"Corresponding grade for score {score} is B")
elif score <= 100:
    print(f"Corresponding grade for score {score} is A")
else:
    print("Score must be between 0 and 100!")


# 3. Ternary Operator Practice
# Use a ternary operator to set a variable status to "adult" if age is 18 or older, and "minor" otherwise.
print("\n===Aufgabe 3===")
age = random.randint(1, 100)
status = "adult" if age >= 18 else "minor"
print(f"Age {age} is {status}")


# 4. For Loop over a List
# Iterate over a list of vehicles using a for loop and print an f-string with each vehicle.
print("\n===Aufgabe 4===")
vehicles = ["BMW", "Audi", "Skoda"]
for vehicle in vehicles:
    print(f"Vehicle: {vehicle}")


# 5. For Loop with Conditions
# Loop over numbers 1 to 10 using a for loop and print only the even numbers, skipping the odds using continue.
print("\n===Aufgabe 5===")
for number in range(1, 11):
    if number % 2:
        continue
    else:
        print(number)


# 6. While Loop Summation
# Use a while loop to sum all numbers from 1 to 100. Print the total sum at the end.
print("\n===Aufgabe 6===")
x = 1
y = 0
while x <= 100:
    y += x
    x += 1
else:
    print("Total sum: ", y)


# 7. Break out of a Loop
# Iterate over a list of words and break out of the loop as soon as a word with more than 5 letters is found. Print that word.
print("\n===Aufgabe 7===")
words = ["car", "dog", "elephant", "parrot"]
for word in words:
    if len(word) > 5:
        print(f"Word found: {word}")
        # break


# 8. Nested Loops
# Create a list of people and a list of pets. Loop over the lists in order to print all the possible combinations of persons and a respective pet.
print("\n===Aufgabe 8===")
people = ["Martin", "Peter", "Jana"]
pets = ["dog", "cat", "parrot"]
for person in people:
    for pet in pets:
        print(person, pet)


# 9. Loop with Else Clause
# Use a for loop to search for a specific value in a list. If the value is found, print a success message and break out of the loop.
# Otherwise, after the loop finishes, use an else clause to print that the value was not found.
print("\n===Aufgabe 9===")
numbers = [1, 2, 3, 4, 5]
x = 3
for number in numbers:
    if number == x:
        print(f"Number {x} found.")
        break
else:
    print("Value was not found.")


# 10. Pass Statement Usage
# Create an empty loop over a list of items using pass to practice structure without executing any code.
print("\n===Aufgabe 10===")
x = []
for y in x:
    pass


# 11. Pattern matching
# Create three lists: fruits, veggies, meat
# Add different food items to each list
# Create a match expression to determine if a given item in a variable item is a fruit, a veggie, a meat product or anything else.
# Remember you can create guards with if statements.
print("\n===Aufgabe 11===")
fruits = ["apple", "mango", "orange", "banana"]
veggies = ["carrot", "potato", "tomate"]
meats = ["chicken", "beef", "pork"]

item = "mango"

match item:
    case _ if item in fruits:
        print(f"{item} is in fruits")
    case _ if item in veggies:
        print(f"{item} is in veggies")
    case _ if item in meats:
        print(f"{item} is in meats")
    case _:
        print(f"{item} isn't in lists")
