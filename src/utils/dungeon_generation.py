from enum import Enum
import random

def foo1():
    print("Printed 1")

def foo2():
    print("Printed 2")

def foo3():
    print("Printed 3")

if __name__ == "__main__":
    fooMap = {
        "1": foo1,
        "2": foo2,
        "3": foo3
    }  

    x = input("Enter num: ")

    fooMap[x]()