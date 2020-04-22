import os

import cv2


class TrainClassifier:
    # crops and scales the already centered amd cleaned numbers
    def crop_scale_training_images():
        for number in range(1, 10):
            variant = 0
            # Making sure to create dirs if not already created
            if not os.path.exists("training_images_cropped/"):
                os.mkdir("training_images_cropped/")
            if not os.path.exists("training_images_cropped_scaled_cells"):
                os.mkdir("training_images_cropped_scaled_cells")

            while True:
                variant += 1
                current_image_path = "training_images/" + str(number) + "/" + str(number) + str(variant) + ".png"
                if not os.path.exists(current_image_path):
                    break  # breaks when the variant gets out of range

                current_cell = cv2.imread(current_image_path, 0)
                height, width = current_cell.shape
                top_boundary = -1  # Corresponds to y values
                bottom_boundary = height  # Corresponds to y values
                left_boundary = -1  # Corresponds to x values
                right_boundary = width  # Corresponds to x values
                top_flag = False
                bottom_flag = False
                left_flag = False
                right_flag = False

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

                # Lets crop the images to their boundaries
                cropped_cell = current_cell[top_boundary:bottom_boundary + 1,
                               left_boundary: right_boundary + 1]
                # Lets scale the cropped pictures
                cropped_scaled_cell = cv2.resize(cropped_cell, (40, 40), cv2.INTER_AREA)

                # Making sure to create dirs if not already created
                if not os.path.exists("training_images_cropped/" + str(number)):
                    os.mkdir("training_images_cropped/" + str(number))
                if not os.path.exists("training_images_cropped_scaled_cells/" + str(number)):
                    os.mkdir("training_images_cropped_scaled_cells/" + str(number))

                cropped_cell_path = "training_images_cropped/" + str(number) + "/" + str(number) + str(variant) + ".png"
                cropped_scaled_cell_path = "training_images_cropped_scaled_cells/" + str(number) + "/" + str(
                    number) + str(variant) + ".png"

                try:
                    os.remove(cropped_cell_path)
                    os.remove(cropped_scaled_cell_path)
                except:
                    pass
                cv2.imwrite(cropped_cell_path, cropped_cell)
                cv2.imwrite(cropped_scaled_cell_path, cropped_scaled_cell)
