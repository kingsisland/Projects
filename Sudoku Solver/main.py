import os.path

import classify_numbers
import extract_grid
import extract_numbers
import sudoku_solver

image_path = str(input("Enter the image file path"))
# if os.path.exists("Output_Images"):
#     os.rmdir("Output_Images")
if os.path.exists(image_path):
    sudoku_image = extract_grid.SudokuImage(image_path)
else:
    sudoku_image = extract_grid.SudokuImage()

sudoku_image.preprocess_image()
sudoku_image.detect_borders()
sudoku_image.correct_perspective()
extracted_grid = sudoku_image.get_extracted_grid()

number_extractor = extract_numbers.NumberExtractor(extracted_grid)
number_extractor.preprocess()
number_extractor.find_numbers()
number_extractor.center_scale_numbers()
number_extractor.crop_scale_numbers()
unclassified_sudoku_table = number_extractor.get_sudoku_table()
# train.crop_scale_training_images()

number_classifier = classify_numbers.NumberClassifier(unclassified_sudoku_table)
number_classifier.classify_numbers()
sudoku_table = number_classifier.get_sudoku_table()
sudoku_solver = sudoku_solver.SudokuSolver(sudoku_table)
sudoku_solver.solve()
