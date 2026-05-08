from stack import Stack


def parentheses(s):
    stack = Stack()
    for i in s:
        if i in "([{":
            stack.push(i)
        elif i in ")]}":
            match i:
                case ")" if stack.pop() != "(":
                    return False
                case "]" if stack.pop() != "[":
                    return False
                case "}" if stack.pop() != "{":
                    return False
    return stack.is_empty()


s = "()"  # True
print(parentheses(s))
s = "()[]{}"  # True
print(parentheses(s))
s = "(]"  # False
print(parentheses(s))
s = "([])"  # True
print(parentheses(s))
s = "((("  # False
print(parentheses(s))
print(parentheses("((("))  # False
print(parentheses(")))"))  # False
print(parentheses("([)]"))  # False
print(parentheses("{[]}"))  # True
print(parentheses(""))  # True
print(parentheses("def:(abc){}"))
