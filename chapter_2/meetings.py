def meetings(intervals):
    for i in range(len(intervals)):
        for j in range(i + 1, len(intervals)):
            if intervals[i][0] < intervals[j][1] and intervals[j][0] < intervals[i][1]:
                return False
    return True


def meetings_fast(intervals):
    intervals.sort(key=lambda interval: interval[0])
    for i in range(len(intervals) - 1):
        if intervals[i][1] > intervals[i + 1][0]:
            return False
    return True


intervals = [[0, 30], [5, 10], [15, 20]]
print(meetings(intervals))
intervals = [[7, 10], [2, 4]]
print(meetings(intervals))
intervals = [[5, 10], [0, 30]]
print(meetings(intervals))
intervals = [[0, 30], [5, 10], [15, 20]]
print(meetings_fast(intervals))
intervals = [[7, 10], [2, 4]]
print(meetings_fast(intervals))
intervals = [[5, 10], [0, 30]]
print(meetings_fast(intervals))
