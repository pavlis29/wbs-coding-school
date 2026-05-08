# 1. Sum of List Elements
# Create a function sum_list(numbers) that takes a list of numbers and returns their sum.
# Use a loop to iterate through the list.
# Use a variable to accumulate the total and return it.
print("\n===Aufgabe 1===")


def sum_list(numbers):
    x = 0
    for number in numbers:
        x += number
    return x


print(sum_list(range(1, 11)))


# 2. Repeated Greeting
# Write a function repeat_greeting(name, times) that prints a greeting to name a specified number of times.
# Use a loop to print the greeting repeatedly.
# Each greeting should be printed on a new line.
print("\n===Aufgabe 2===")


def repeat_greeting(name, times):
    x = 0
    while x < times:
        print("Hello", name)
        x += 1


repeat_greeting("Lucy", 5)


# 3. Factorial Calculation
# Define a function factorial(n) that returns the factorial of a given number n.
# Use a loop (such as a for or while loop) to multiply numbers from 1 to n.
# Return the factorial result.
print("\n===Aufgabe 3===")


def factorial(n):
    if n == 0:
        return 1
    x = n - 1
    y = n
    while x > 0:
        y *= x
        x -= 1
    return y


print(factorial(5))


# 4. Fibonacci Sequence Generator
# Create a function fibonacci(n) that returns a list containing the first n numbers of the Fibonacci sequence.
# Use loops and conditionals to calculate each Fibonacci number.
# Start the sequence with 0 and 1, then build the sequence up to n terms.
print("\n===Aufgabe 4===")


def fibonacci_seq_gen(n):
    first_number = 0
    second_number = 1
    result = []
    while len(result) < n:
        result.append(first_number)
        next_number = first_number + second_number
        first_number = second_number
        second_number = next_number
    return result


print(fibonacci_seq_gen(10))


def fibonacci_seq_gen(n):
    if n == 1:
        result = [0]
    elif n == 2:
        result = [0, 1]
    else:
        result = [0, 1]
        while len(result) <= n:
            result.append(result[-1] + result[-2])

    return result


print(fibonacci_seq_gen(15))


# 5. Maximum of Two Numbers
# Write a function max_of_two(a, b) that returns the larger of two numbers.
# Use an if...else conditional to compare a and b.
# Return the greater value.
print("\n===Aufgabe 5===")


def max_of_two(a, b):
    if a < b:
        return b
    elif a > b:
        return a
    else:
        return "The numbers are the same"


print(max_of_two(17, 17))


# 6. Print a Pattern with Nested Loops
# Design a function print_triangle(rows) that prints a right-angled triangle of asterisks (*) with a given number of rows.
# Use nested loops: an outer loop for rows and an inner loop for printing asterisks in each row.
# Each subsequent row should have one more asterisk than the previous row.
print("\n===Aufgabe 6===")


def print_triangle(rows):
    for row in range(1, rows + 1):
        x = 1
        if row != 1:
            print()
        while x <= row:
            print("*", end="")
            x += 1
    print()


print_triangle(10)
