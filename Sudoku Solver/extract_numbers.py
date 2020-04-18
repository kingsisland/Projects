import cv2
import numpy as np
import os
import matplotlib.pyplot as plt


class NumberExtractor:
    def __init__(self):
        self.image = cv2.imread("Output_Images/extracted_grid_1.png", 0)
        self.thresholded = None
        self.sudoku_numbers = np.ones((9, 9)).astype(np.int)
        if not os.path.exists("Output_Images/cells"):
            os.mkdir("Output_Images/cells")
        if not os.path.exists("Output_Images/cleaned_cells"):
            os.mkdir("Output_Images/cleaned_cells")
        if not os.path.exists("Output_Images/centered_cells"):
            os.mkdir("Output_Images/centered_cells")

    def preprocess(self):
        denoised = cv2.GaussianBlur(self.image, (5, 5), 0)
        thresholded = cv2.adaptiveThreshold(denoised, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, 5, 5)
        self.thresholded = cv2.bitwise_not(thresholded)
        # kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3))
        # dilated = cv2.dilate(thresholded, kernel, iterations=1)

    def find_numbers(self):
        # empty_image = np.ones_like(self.thresholded)
        # Divide the board into 81 squares
        cell_size = int(self.image.shape[0] / 9) + 1
        for y in range(9):
            for x in range(9):
                cropped_cell = self.thresholded[y * cell_size:(y + 1) * cell_size - 1,
                               x * cell_size: (x + 1) * cell_size - 1]
                cell_path = "Output_Images/cells/" + str(y) + str(x) + ".png"
                try:
                    os.remove(cell_path)
                except:
                    pass

                # Write each cell to memory
                cv2.imwrite(cell_path, cropped_cell)

        # Find numbered cells and empty cells using flood fill
        for j in range(9):
            for i in range(9):
                cell_path = "Output_Images/cells/" + str(j) + str(i) + ".png"
                current_cell = cv2.imread(cell_path, 0)
                current_cell = cv2.resize(current_cell, (40, 40), cv2.INTER_AREA)
                y = int(current_cell.shape[0] / 2)
                number_found = False
                seed_point = None
                for x in range(int(current_cell.shape[1] / 3), int(2 * current_cell.shape[1] / 3)):
                    if current_cell[y, x] == 255:
                        number_found = True
                        seed_point = (x, y)
                        break
                if number_found:
                    # print("Seed Point is: %s and y,x is %d,%d" %(seed_point, j, i))
                    cv2.floodFill(current_cell, None, seed_point, 150)

                    # Fill rest of the image with black leaving only the number
                    for y in range(current_cell.shape[0]):
                        for x in range(current_cell.shape[1]):
                            if current_cell[y, x] != 150:
                                cv2.floodFill(current_cell, None, (x, y), 0)

                    cv2.floodFill(current_cell, None, seed_point, 255)
                    cleaned_cell_path = "Output_Images/cleaned_cells/" + str(j) + str(i) + ".png"
                    try:
                        os.remove(cleaned_cell_path)
                    except:
                        pass
                    # write cleaned cell to memory

                    cv2.imwrite(cleaned_cell_path, current_cell)
                else:
                    # print("Not found for %d,%d" %(j, i))
                    self.sudoku_numbers[j, i] = -1

    def center_scale_numbers(self):
        # Cell centering and scaling to get uniform scaled and center digited cells
        for i in range(9):
            for j in range(9):
                #  RENAME TO self.sudoku_numbers -------------------________________________-----------------------
                if self.sudoku_numbers[i, j] == -1:
                    continue
                current_cell_path = "Output_Images/cleaned_cells/" + str(i) + str(j) + ".png"
                current_cell = cv2.imread(current_cell_path, 0)

                top_space = 0
                bottom_space = 0
                left_space = 0
                right_space = 0
                top_flag = False
                bottom_flag = False
                left_flag = False
                right_flag = False

                # VERTICAL CENTERING OF THE DIGIT
                for y in range(current_cell.shape[0]):
                    if top_flag:
                        break
                    for x in range(current_cell.shape[1]):
                        if current_cell[y, x] == 255:
                            top_space = y
                            top_flag = True
                            break

                for y in range(current_cell.shape[0] - 1, -1, -1):
                    if bottom_flag:
                        break
                    for x in range(current_cell.shape[1]):
                        if current_cell[y, x] == 255:
                            bottom_space = current_cell.shape[0] - 1 - y
                            bottom_flag = True
                            break

                if top_space > bottom_space:
                    # MOVE THE PICTURE UP
                    shift = int((top_space - bottom_space) / 2)
                    if not shift == 0:
                        for y in range(top_space, current_cell.shape[0]):
                            for x in range(current_cell.shape[1]):
                                if current_cell[y, x] == 255:
                                    current_cell[y - shift, x] = current_cell[y, x]
                                    current_cell[y, x] = 0
                elif bottom_space > top_space:
                    # Move the picture down
                    shift = int((bottom_space - top_space) / 2)
                    if shift == 0:
                        shift = 1
                    for y in range(current_cell.shape[0] - 1 - bottom_space, -1, -1):
                        for x in range(current_cell.shape[1]):
                            if current_cell[y, x] == 255:
                                current_cell[y + shift, x] = current_cell[y, x]
                                current_cell[y, x] = 0

                # HORIZONTAL CENTERING OF THE DIGIT
                for x in range(current_cell.shape[1]):
                    if left_flag:
                        break
                    for y in range(current_cell.shape[0]):
                        if current_cell[y, x] == 255:
                            left_space = x
                            left_flag = True
                            break

                for x in range(current_cell.shape[1] - 1, -1, -1):
                    if right_flag:
                        break
                    for y in range(current_cell.shape[0]):
                        if current_cell[y, x] == 255:
                            right_space = current_cell.shape[1] - 1 - x
                            right_flag = True
                            break

                if left_space > right_space:
                    # MOVE THE IMAGE LEFT WARDS
                    shift = int((left_space - right_space) / 2)
                    if not shift == 0:
                        for x in range(left_space, current_cell.shape[1]):
                            for y in range(current_cell.shape[0]):
                                if current_cell[y, x] == 255:
                                    current_cell[y, x - shift] = current_cell[y, x]
                                    current_cell[y, x] = 0
                elif right_space > left_space:
                    # MOVE THE IMAGE TOWARDS RIGHT
                    shift = int((right_space - left_space) / 2)
                    if shift == 0:
                        shift = 1
                    for x in range(current_cell.shape[1] - 1 - right_space, -1, -1):
                        for y in range(current_cell.shape[0]):
                            if current_cell[y, x] == 255:
                                current_cell[y, x + shift] = current_cell[y, x]
                                current_cell[y, x] = 0

                cleaned_cell_path = "Output_Images/centered_cells/" + str(i) + str(j) + ".png"
                try:
                    os.remove(cleaned_cell_path)
                finally:
                    pass
                cv2.imwrite(cleaned_cell_path, current_cell)

    def find_number_boundaries_and_scale_(self):
        # Tightens on the number present in cleaned cell and scales all the found numbers to a uniform value
        for i in range(9):
            for j in range(9):
                if self.sudoku_numbers[i, j] == -1:
                    continue

                height, width = current_cell.shape
                current_cell_path = "Output_Images/cleaned_cells/" + str(i) + str(j) + ".png"
                current_cell = cv2.imread(current_cell_path, 0)

                top_boundary = -1             # Corresponds to y values
                bottom_boundary = height      # Corresponds to y values
                left_boundary = -1            # Corresponds to x values
                right_boundary = width        # Corresponds to x values

                # Finds the top most pixel
                for y in range(height):
                    if top_flag:
                        break
                    for x in range(width):
                        if current_cell[y, x] == 255:
                            top_boundary = y
                            top_flag = True
                            break
                # Finds the bottom post pixel that is lit up
                for y in range(height - 1, -1, -1):
                    if bottom_flag:
                        break
                    for x in range(width):
                        if current_cell[y, x] == 255:
                            bottom_boundary = y
                            bottom_flag = True
                            break
                # Finds the left most pixel
                for x in range(width):
                    if left_flag:
                        break
                    for y in range(height):
                        if current_cell[y, x] == 255:
                            left_boundary = x
                            left_flag = True
                            break
                # Finds the right most boundary
                for x in range(width - 1, -1, -1):
                    if right_flag:
                        break
                    for y in range(height):
                        if current_cell[y, x] == 255:
                            right_boundary = x
                            right_flag = True
                            break

                # Making sure to create dirs if not already created
                if not os.path.exists("Output_Images/cropped_cells"):
                    os.mkdir("Output_Images/cropped_cells")
                if not os.path.exists("Output_Images/cropped_scaled_images"):
                    os.mkdir("Output_Images/cropped_scaled_images")

                # Lets crop the images to their boundaries
                cropped_cell = current_cell[top_boundary:bottom_boundary + 1, left_boundary: right_boundary + 1]
                # Lets scale the cropped pictures
                cropped_scaled_cell = cv2.resize(cropped_cell, (40, 40), cv2.INTER_AREA)

                cropped_cell_path = "Output_Images/cropped_cells/" + str(i) + str(j) + ".png"
                cropped_scaled_cell_path = "Output_Images/cropped_scaled_cells/" + str(i) + str(j) + ".png"
                try:
                    os.remove(cropped_cell_path)
                    os.remove(cropped_scaled_cell_path)
                finally:
                    pass
                cv2.imwrite(cropped_cell_path,cropped_cell)
                cv2.imwrite(cropped_scaled_cell_path, cropped_scaled_cell)





number_extractor = NumberExtractor()
number_extractor.preprocess()
number_extractor.find_numbers()
#number_extractor.center_scale_numbers()