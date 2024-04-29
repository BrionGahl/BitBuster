from enum import Enum
import random

class RoomType(Enum):
    NONE = 0
    NORMAL = 1
    NORMAL_BIG = 2
    NORMAL_LONG = 3
    NORMAL_TALL = 4
    START = 5
    BOSS = 6
    TREASURE = 7
    SECRET = 8
    STORE = 9


class Dungeon():

    def __init__(self, l: int) -> None:
        self.level = l
        self.room_limit = random.randrange(3) + 5 + self.level * 2
        
    def print_map(self) -> None:
        for row in self.mapgrid:
            print(row)   

    def generate(self) -> None:
        self.room_queue = []
        self.end_rooms = []
        
        self.mapgrid = [[0] * 9 for i in range (8)]
        self.mapgrid[3][4] = RoomType.START.value
        
        room_count = 1
        self.room_queue.append((4,3))

        neighbors = []

        while (len(self.room_queue) > 0):
            print(f"Generating room: {room_count} / {self.room_limit}")
            curr_room = self.room_queue.pop(0)




if __name__ == "__main__":
    dungeon = Dungeon(1)
    dungeon.generate()
    dungeon.print_map()