from collections import deque

class AsyncQueue:
    def __init__(self):
        self.items = deque()
        self.waiting = deque()

    def put(self, item):
        self.items.append(item)
        if self.waiting:
            sched.call_soon(self.waiting.popleft())

    def get(self):
        if not self.items:
            #Suspend myself (somehow)
            self.waiting.append(
                lambda: self.get(callback))

        else:
            callback(self.items.popleft())



def producer(queue, count, n=0):
    if n < count:
        print('Producing', n)
        queue.put(n)
        sched.call_later(1, lambda: producer(queue, count, n+1))
    else:
        queue.put(None)

def consumer(queue):
    def got_it(item):
        if item is not None:
            print('Consumed', item)
            sched.call_soon(lambda: consumer(queue))

    queue.get(callback = got_it)




        

import queue , threading, time
q = queue.Queue()
threading.Thread(target = producer, args =(q, 10)).start()
threading.Thread(target = consumer, args = (q,)).start()