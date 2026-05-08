print("\n===Aufgabe 1===")


def average_of_a_set_of_numbers(student_scores):
    if len(student_scores) == 0:
        return 0

    total_score = 0
    for student_score in student_scores:
        total_score += student_score

    return total_score / len(student_scores)


input_number = 2
divisor = 2
if input_number == 0 or input_number == 1:
    print("Not Prime")

while input_number % divisor != 0:
    if input_number % divisor == 0:
        print("Not Prime")
    else:
        divisor = divisor + 1
        if divisor == input_number:
            print("Prime")
