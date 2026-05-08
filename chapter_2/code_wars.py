def solution(text, ending):
    len_end = len(ending)
    len_text = len(text)
    index = len_text - len_end
    output = False
    i = 0
    if index < 0:
        return output
    for t in text:
        if i >= index:
            if t == ending[i - index]:
                output = True
            else:
                return False
        i += 1
    return output


print(solution("ails", "fails"))


def solution2(text, ending):
    return text.endswith(ending)


print(solution2("fails", "ails"))
