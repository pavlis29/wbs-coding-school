def destination_city(paths):
    city = {}
    city.update(paths)
    for index, value in enumerate(paths):
        if value[1] not in city.keys():
            return value[1]


paths = [["London", "New York"], ["New York", "Lima"], ["Lima", "Sao Paulo"]]
print(destination_city(paths))
paths = [["B", "C"], ["D", "B"], ["C", "A"]]
print(destination_city(paths))
paths = [["A", "Z"]]
print(destination_city(paths))
