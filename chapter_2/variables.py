name = "Pavel"
age = 42
height = 1.89
print(f"My name is {name}, I'm {age} years old and I'm {height} meters tall")
print(type(name))
print(type(age))
print(type(height))
age_str = str(age)
print("My name si " + name + " and I'm " + age_str + " years old.")
global_message = "message"
def test():
    global global_message
    global_message = "new message"

test()
print(global_message)