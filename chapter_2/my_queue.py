class Queue:
    def __init__(self, items=None):
        if items is None:
            self.queue = []
        else:
            self.queue = items.copy()

    def __str__(self):
        return "Queue: " + str(self.queue)

    def enqueue(self, element):
        self.queue.append(element)

    def dequeue(self):
        if self.isEmpty():
            return "Queue is empty"
        return self.queue.pop(0)

    def peek(self):
        if self.isEmpty():
            return "Queue is empty"
        return self.queue[0]

    def isEmpty(self):
        return len(self.queue) == 0

    def size(self):
        return len(self.queue)
