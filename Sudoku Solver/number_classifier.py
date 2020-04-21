import math
import os

import cv2

from extract_numbers import NumberExtractor


class NumberClassifier:
    def __init__(self, number_extractor=NumberExtractor()):
        self.sudoku_table = number_extractor.get_sudoku_table()

    def classify_numbers(self):
        # Classifies numbers based on the how close a number is to a particular number's training images in training set
        for row in range(9):
            for col in range(9):
                if self.sudoku_table[row, col] == -1:
                    continue
                current_cell_path = "Output_Images/cropped_scaled_cells/" + str(row) + str(col) + ".png"
                current_cell = cv2.imread(current_cell_path, 0)
                # print("The cell is %s" % ((row, col),))
                max_count = -1
                classified_number = None
                # Compare this current cell to all the existing training images
                for number in range(1, 10):
                    variant = 0

                    # Have to make sure that training images are created before performing classification ______________
                    # # Making sure to create dirs if not already created
                    # if not os.path.exists("training_images_cropped/"):
                    #     os.mkdir("training_images_cropped/")
                    # if not os.path.exists("training_images_cropped_scaled_cells"):
                    #     os.mkdir("training_images_cropped_scaled_cells")

                    while True:
                        variant += 1
                        current_image_path = "training_images_cropped_scaled_cells/" + str(number) + "/" + str(
                            number) + str(
                            variant) + ".png"
                        if not os.path.exists(current_image_path):
                            break  # breaks when the variant gets out of range

                        current_training_image = cv2.imread(current_image_path, 0)
                        height, width = current_training_image.shape

                        if current_cell.shape == current_training_image.shape:
                            matches = 0
                            for y in range(height):
                                for x in range(width):
                                    if current_cell[y, x] == current_training_image[y, x]:
                                        matches += 1

                            # Whenever a close match of a number's variant occurs
                            if matches > max_count:
                                max_count = matches
                                classified_number = number
                                # print("Current best match is %s with %d pixels matched" % ((number, variant), matches))

                # print("Finally, the classified number is %d with %d matches" % (classified_number, max_count))
                self.sudoku_table[row, col] = classified_number


class SudokuSolver:
    def __init__(self, number_classifier=NumberClassifier()):
        self.sudoku_table = number_classifier.sudoku_table
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


number_extractor = NumberExtractor()
number_extractor.preprocess()
number_extractor.find_numbers()
number_classifier = NumberClassifier(number_extractor)
number_classifier.classify_numbers()
sudoku_solver = SudokuSolver(number_classifier)
sudoku_solver.solve()
# number_extractor.center_scale_numbers()
# number_extractor.crop_scale_numbers()
