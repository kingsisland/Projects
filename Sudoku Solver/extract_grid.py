import os

import cv2
import numpy as np


# DRAWS LINES
def draw_lines(image, hough_lines, color=None, thickness=2):
    if color is None:
        color = [255, 255, 255]
    height, width = image.shape
    for line in hough_lines:
        rho, theta = line[0]
        if rho == 0 and theta == -100:
            continue
        if theta != 0:
            m = -1 / np.tan(theta)
            c = rho / np.sin(theta)
            (x1, y1) = (0, int(c))
            (x2, y2) = (width, int(m * width + c))

        else:
            (x1, y1) = (rho, 0)
            (x2, y2) = (rho, height)

        cv2.line(image, (x1, y1), (x2, y2), color, thickness)


# Merging lines
def merge_related_lines(lines, image):
    height, width = image.shape
    for current in lines:
        if current[0, 0] == 0 and current[0, 1] == -100:
            continue
        rho_current, theta_current = current[0]

        if np.pi * 45 / 180 <= theta_current <= np.pi * 135 / 180:
            # Almost horizontal
            x1_curr, y1_curr = (0, rho_current / np.sin(theta_current))
            x2_curr, y2_curr = (width, rho_current / np.sin(theta_current) - width / np.tan(theta_current))
        else:
            # Almost Vertical
            x1_curr, y1_curr = (rho_current / np.cos(theta_current), 0)
            x2_curr, y2_curr = (rho_current / np.cos(theta_current) - height * np.tan(theta_current), height)

        for pos in lines:
            if (pos[0] == current[0]).all():
                continue
            rho_pos, theta_pos = pos[0]
            if rho_pos == 0 and theta_pos == -100:
                continue
            if abs(rho_pos - rho_current) < 20 and abs(theta_pos - theta_current) < np.pi * 10 / 180:
                if np.pi * 45 / 180 <= theta_pos <= np.pi * 135 / 180:
                    x1_pos, y1_pos = (0, rho_pos / np.sin(theta_pos))
                    x2_pos, y2_pos = (width, rho_pos / np.sin(theta_pos) - width / np.tan(theta_pos))
                else:
                    # Almost vertical
                    x1_pos, y1_pos = (rho_pos / np.cos(theta_pos), 0)
                    x2_pos, y2_pos = (rho_pos / np.cos(theta_pos) - height * np.tan(theta_pos), height)
                if (x1_curr - x1_pos) ** 2 + (y1_curr - y1_pos) ** 2 <= 64 * 64 and \
                        (x2_curr - x2_pos) ** 2 + (y2_curr - y2_pos) ** 2 <= 64 * 64:
                    # The two lines are close enough. Lets merge i.e, average them and estimate new points
                    rho_current = (rho_current + rho_pos) / 2
                    theta_current = (theta_current + theta_pos) / 2

                    pos[0, 0] = 0
                    pos[0, 1] = -100


# Figures out the borders of the outer_box
def find_borders(lines, image):
    height, width = image.shape
    top_edge = None
    bottom_edge = None
    left_egde = None
    right_egde = None

    top_dist = (height ** 2) * 3
    bottom_dist = 0
    left_dist = (width ** 2) * 3
    right_dist = 0

    for line in lines:
        if line[0, 0] == 0 and line[0, 1] == -100:
            continue
        if np.pi * 45 / 180 <= line[0, 1] <= np.pi * 135 / 180:
            # Horizontal line
            x1, y1 = 0, line[0, 0] / np.sin(line[0, 1])
            if y1 >= height:
                y1 = height - 1
            elif y1 < 0:
                y1 = 0

            x2, y2 = width, line[0, 0] / np.sin(line[0, 1]) - width / np.tan(line[0, 1])
            if y2 >= height:
                y2 = height - 1
            elif y2 < 0:
                y2 = 0

            dist = y1 ** 2 + y2 ** 2
            if dist < top_dist:
                top_dist = dist
                top_edge = line
            if dist > bottom_dist:
                bottom_dist = dist
                bottom_edge = line
        else:
            # Vertical line
            x1, y1 = line[0, 0] / np.cos(line[0, 1]), 0
            if x1 < 0:
                x1 = 0
            elif x1 > width:
                x1 = width

            x2, y2 = line[0, 0] / np.cos(line[0, 1]) - height * np.tan(line[0, 1]), height
            if x2 < 0:
                x2 = 0
            elif x2 > width:
                x2 = width

            dist = x1 ** 2 + x2 ** 2
            if dist < left_dist:
                left_dist = dist
                left_egde = line
            if dist > right_dist:
                right_dist = dist
                right_egde = line
    return [top_edge, bottom_edge, left_egde, right_egde]


def find_intersection_point(line_a, line_b):
    rho_1, theta_1 = line_a[0]
    rho_2, theta_2 = line_b[0]

    x = (rho_2 * np.sin(theta_1) - rho_1 * np.sin(theta_2)) / np.sin(theta_1 - theta_2)
    y = (rho_1 - x * np.cos(theta_1)) / np.sin(theta_1)
    return int(x), int(y)


# Find the longest edge
def length_of_longest_edge(top_left_corner, top_right_corner, bottom_left_corner, bottom_right_corner):
    maxLen = (bottom_left_corner[0] - bottom_right_corner[0]) ** 2 + (
            bottom_left_corner[1] - bottom_right_corner[1]) ** 2
    temp = (top_left_corner[0] - top_right_corner[0]) ** 2 + (top_left_corner[1] - top_right_corner[1]) ** 2
    if temp > maxLen:
        maxLen = temp
    temp = (top_left_corner[0] - bottom_left_corner[0]) ** 2 + (top_left_corner[1] - bottom_left_corner[1]) ** 2
    if temp > maxLen:
        maxLen = temp
    temp = (top_right_corner[0] - bottom_right_corner[0]) ** 2 + (top_right_corner[1] - bottom_right_corner[1]) ** 2
    if temp > maxLen:
        maxLen = temp
    return int(maxLen ** 0.5)


class SudokuImage:
    def __init__(self, image_path="sudokuFull.jpg"):
        # Check if output folder is already created
        if not os.path.exists("Output_Images"):
            os.mkdir("Output_Images")
        if not os.path.exists("Output_Images/preprocessing_phase"):
            os.mkdir("Output_Images/preprocessing_phase")

        self.image = cv2.imread(image_path, 0)
        self.outer_box = None
        self.extracted_grid = None
        self.hough_lines = None
        self.edges = None
        self.top_left_corner = None
        self.top_right_corner = None
        self.bottom_left_corner = None
        self.bottom_right_corner = None
        cv2.imwrite("Output_Images/preprocessing_phase/original.png", self.image)

    def preprocess_image(self):
        blurred = cv2.GaussianBlur(self.image, (5, 5), 0)
        thresholded = cv2.adaptiveThreshold(blurred, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, 5, 5)
        thresholded = cv2.bitwise_not(thresholded)
        cv2.imwrite("Output_Images/preprocessing_phase/thresholded.png", thresholded)
        kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3))
        dilated_image = cv2.dilate(thresholded, kernel, iterations=1)
        self.outer_box = dilated_image
        cv2.imwrite("Output_Images/preprocessing_phase/dilated_image.png", dilated_image)
        # Flood Fill to find the biggest blob

        height, width = np.shape(self.outer_box)
        max1 = -1
        max_pt = None
        for y in range(height):
            row = self.outer_box[y]
            for x in range(width):
                if row[x] >= 200:
                    area = cv2.floodFill(self.outer_box, None, (x, y), 64)[0]
                    if area > max1:
                        max1 = area
                        max_pt = (x, y)

        cv2.floodFill(self.outer_box, None, max_pt, (255, 255, 255))

        for y in range(height):
            row = self.outer_box[y]
            for x in range(width):
                if row[x] < 200 and x != max_pt[0] and y != max_pt[1]:
                    cv2.floodFill(self.outer_box, None, (x, y), (0, 0, 0))

        cv2.imwrite("Output_Images/preprocessing_phase/floodfill.png", self.outer_box)

        # outer_box = cv2.erode(outer_box,kernel)

    def detect_borders(self):

        # Parameters that define the desired accuracy that is 1 degree theta and 1 unit rho
        rho_resolution = 1
        theta_resolution = np.pi / 180
        threshold = 155
        self.hough_lines = cv2.HoughLines(self.outer_box, rho_resolution, theta_resolution, threshold)

        hough_lines_image = np.zeros_like(self.outer_box)
        draw_lines(hough_lines_image, self.hough_lines)
        cv2.imwrite("Output_Images/preprocessing_phase/hough_lines.png", hough_lines_image)

        merge_related_lines(self.hough_lines, self.image)

        hough_lines_image = np.zeros_like(self.image)
        draw_lines(hough_lines_image, self.hough_lines)
        hough_lines_image_bitwise_not = cv2.bitwise_not(hough_lines_image)
        cv2.imwrite("Output_Images/preprocessing_phase/merged_hough_lines.png", hough_lines_image_bitwise_not)

        # Find the borders of the Sudoku board
        self.edges = find_borders(self.hough_lines, self.image)
        edges_image = np.zeros_like(self.outer_box)
        draw_lines(edges_image, self.edges)
        edges_image = cv2.bitwise_not(edges_image)
        cv2.imwrite("Output_Images/preprocessing_phase/recognized_edges.png", edges_image)
        self.top_left_corner = find_intersection_point(self.edges[0], self.edges[2])
        self.top_right_corner = find_intersection_point(self.edges[0], self.edges[3])
        self.bottom_left_corner = find_intersection_point(self.edges[1], self.edges[2])
        self.bottom_right_corner = find_intersection_point(self.edges[1], self.edges[3])

    def correct_perspective(self):
        # Getting to the part of warping the image to correct the orientation and center it
        maxLen = length_of_longest_edge(self.top_left_corner, self.top_right_corner,
                                        self.bottom_left_corner, self.bottom_right_corner)
        src = np.array([self.top_left_corner, self.top_right_corner, self.bottom_right_corner,
                        self.bottom_left_corner]).astype(np.float32)
        dst = np.array([[0, 0], [maxLen - 1, 0], [maxLen - 1, maxLen - 1], [0, maxLen - 1]]).astype(np.float32)
        M = cv2.getPerspectiveTransform(src, dst)
        self.extracted_grid = cv2.warpPerspective(self.image, M, (maxLen, maxLen))
        extracted_grid_path = "Output_Images/extracted_grid.png"
        try:
            os.remove(extracted_grid_path)
        except:
            pass
        cv2.imwrite(extracted_grid_path, self.extracted_grid)
        return extracted_grid_path

    def get_extracted_grid(self):
        return self.extracted_grid
