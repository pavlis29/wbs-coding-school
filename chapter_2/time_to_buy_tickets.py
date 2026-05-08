from my_queue import Queue


def time_to_buy_tickets(tickets, k):
    queue = Queue(tickets)
    num_of_tickets = tickets[k]
    time = 0
    while num_of_tickets:
        i = queue.dequeue()
        i -= 1
        if i > 0:
            queue.enqueue(i)
        if k == 0:
            k = queue.size() - 1
            num_of_tickets -= 1
        else:
            k -= 1
        time += 1
    return time


tickets = [2, 3, 2]
k = 2
print(time_to_buy_tickets(tickets, k))
tickets = [5, 1, 1, 1]
k = 0
print(time_to_buy_tickets(tickets, k))
