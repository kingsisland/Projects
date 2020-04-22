import math


class SudokuSolver:
    def __init__(self, sudoku_table):
        self.sudoku_table = sudoku_table
        self.row_sets = list(set() for _ in range(9))
        self.col_sets = list(set() for _ in range(9))
        self.nonet = dict()
        for i in range(3):
            for j in range(3):
                nonet_name = str(i) + str(j)
                self.nonet[nonet_name] = set()

        #  Filling out the row sets, col sets and the nonet sets. Preprocessing the initial numbers present on the board
        for i in range(9):
            for j in range(9):
                if self.sudoku_table[i, j] == -1:
                    continue
                self.row_sets[i].add(self.sudoku_table[i, j])
                self.col_sets[j].add(self.sudoku_table[i, j])
                nonet_name = str(math.floor(i / 3)) + str(math.floor(j / 3))
                self.nonet[nonet_name].add(self.sudoku_table[i, j])

    def solve(self):
        is_solved = None
        flag = False
        for i in range(9):
            if flag:
                break
            for j in range(9):
                if self.sudoku_table[i, j] != -1:
                    continue

                # If we come across an empty slot
                is_solved = self.try_numbers_recursion(i, j)
                flag = True
                break
        if is_solved:
            print(self.sudoku_table)
        else:
            print("The sudoku puzzle could not be solved, sorry!")

    def try_numbers_recursion(self, i, j):
        # print("recursion %s" %((i,j ),))
        nonet_name = str(math.floor(i / 3)) + str(math.floor(j / 3))
        for num in range(1, 10):

            if self.row_sets[i].__contains__(num) or self.col_sets[j].__contains__(num) or \
                    self.nonet[nonet_name].__contains__(num):
                continue
            # We proceed to add the number to all the row, col and nonet sets and the sudoku table
            self.row_sets[i].add(num)
            self.col_sets[j].add(num)
            self.nonet[nonet_name].add(num)
            self.sudoku_table[i, j] = num
            if (i, j) == (8, 8):  # Dont allow the function to call for out of bound indexes
                return True

            # Then we call the next element
            row, col = i, j
            flag = False
            while row < 9:
                while col < 9:
                    if self.sudoku_table[row, col] == -1:
                        flag = True
                        break
                    col += 1
                if flag:
                    break
                row += 1
                col = 0  # reset the col

            if not self.try_numbers_recursion(row, col):
                self.row_sets[i].remove(num)
                self.col_sets[j].remove(num)
                self.nonet[nonet_name].remove(num)
                self.sudoku_table[i, j] = -1
            else:
                return True

        return False
