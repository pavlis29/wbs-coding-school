class Node:
    def __init__(self, value):
        self.value = value
        self.next = None


class LinkedList:
    def __init__(self):
        self.head = None
        self.tail = None
        self.length = 0

    def push(self, value):
        node = Node(value)
        self.length += 1
        if not self.head:
            self.head = node
        else:
            self.tail.next = node
        self.tail = node

    def addFirst(self, value):
        if self.length == 0:
            self.push(value)
        else:
            node = Node(value)
            node.next = self.head
            self.head = node
            self.length += 1

    def _find(self, index):
        if index >= self.length:
            return None
        current = self.head
        for i in range(index):
            current = current.next
        return current

    def insertAt(self, index, value):
        if index < 0 or index > self.length:
            return None
        if index == self.length:
            self.push(value)
        elif index == 0:
            self.addFirst(value)
        else:
            new_node = Node(value)
            node = self._find(index - 1)
            new_node.next = node.next
            node.next = new_node
            self.length += 1

    def printList(self):
        node = self.head
        while node:
            print(node.value)
            node = node.next

    def delete(self, index):
        if index == 0:
            head = self.head
            if head:
                self.head = head.next
            else:
                self.head = None
                self.tail = None
            self.length -= 1
            return head.value

        node = self._find(index - 1)
        excise = node.next
        if not excise:
            return None
        node.next = excise.next
        if not node.next:
            self.tail = node
        self.length -= 1
        return excise.value

    def turn_around(self):
        if self.length < 2:
            return None
        node = self.tail
        for i in range(self.length - 1):
            self.insertAt(i, node.value)
            self.delete(self.length - 1)
            node = self.tail

    def reverse(self):
        if self.length < 2:
            return
        current = self.head.next
        self.tail = self.head
        self.tail.next = None
        previous = self.head
        for i in range(self.length - 1):
            next_node = current.next
            current.next = previous
            previous = current
            current = next_node
        self.head = previous

    def reverse2(self):
        if self.length < 2:
            return
        start = self.head
        self.head, self.tail = self.tail, self.head
        self.stack_rekur(start)
        start.next = None

    def stack_rekur(self, item):
        if item.next is None:
            return
        self.stack_rekur(item.next)
        item.next.next = item


linked_list = LinkedList()
linked_list.push("A")
linked_list.push("B")
linked_list.push("C")
linked_list.push("D")
linked_list.push("E")
# linked_list.turn_around()
# linked_list.reverse()
linked_list.reverse2()
linked_list.printList()
