def maze_runner(maze, directions):
    up_down, right_left = find_start(maze)
    for direction in directions:
        match direction:
            case "N":
                up_down -= 1
            case "S":
                up_down += 1
            case "W":
                right_left -= 1
            case "E":
                right_left += 1
        pos = position(maze, up_down, right_left)
        if pos != None:
            return pos
    return "Lost"


def find_start(maze):
    i = 0
    for maz in maze:
        j = 0
        for ma in maz:
            if ma == 2:
                return i, j
            j += 1
        i += 1


def position(maze, up_down, right_left):
    if up_down < 0 or right_left < 0:
        return "Dead"
    try:
        pos = maze[up_down][right_left]
    except:
        return "Dead"
    match pos:
        case 3:
            return "Finish"
        case 1:
            return "Dead"


print(
    maze_runner(
        [
            [1, 1, 1, 1, 1, 1, 1],
            [1, 0, 0, 0, 0, 0, 3],
            [1, 0, 1, 0, 1, 0, 1],
            [0, 0, 1, 0, 0, 0, 1],
            [1, 0, 1, 0, 1, 0, 1],
            [1, 0, 0, 0, 0, 0, 1],
            [1, 2, 1, 0, 1, 0, 1],
        ],
        ["N", "N", "N", "N", "N", "E", "E", "E", "E", "E"],
    )
)
