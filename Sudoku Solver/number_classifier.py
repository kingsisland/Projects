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
                print("The cell is %s" % ((row, col),))
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
                                print("Current best match is %s with %d pixels matched" % ((number, variant), matches))

                print("Finally, the classified number is %d with %d matches" % (classified_number, max_count))
                self.sudoku_table[row, col] = classified_number


number_extractor = NumberExtractor()
number_extractor.preprocess()
number_extractor.find_numbers()
number_classifier = NumberClassifier(number_extractor)
number_classifier.classify_numbers()

# number_extractor.center_scale_numbers()
# number_extractor.crop_scale_numbers()
