#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Mar 29 00:28:17 2020

@author: kaytlyn
"""

import imutils
from skimage import exposure
import numpy as np
import argparse
import cv2
import matplotlib.pyplot as plt

# construct an argument parser to parse the arguments
#ap = argparse.ArgumentParser("-q", "--query", required = True, help = "Path to the query image")
#args = vars(ap.parse_args())

image = cv2.imread("sudokuImage.png")
ratio = image.shape[0]/300.0
orig = image.copy()

image = imutils.resize(image, height= 300)

# convert the image to grayscale, blur it, and find edges
# in the image
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
gray = cv2.bilateralFilter(gray, 11, 17, 17)

edged = cv2.Canny(gray, 50, 150)


cnts = cv2.findContours(edged.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE )

cnts = imutils.grab_contours(cnts)
cnts = sorted(cnts, key = cv2.contourArea,reverse =True)[:10]
sudokuCnt = None



# loop over the contours found
for c in cnts:
    # approximate the contour
    peri = cv2.arcLength(c, True)
    approx = cv2.approxPolyDP(c, 0.015* peri, True)
    img = image.copy()
    cv2.drawContours(img, [approx], -1 ,(0,255,0),3)
    cv2.imshow("Sudoku Contours", img)
    cv2.waitKey(0)
  

    # if the contour has four points then we
    # have found the screen
#    if len(approx) == 4:
#        sudokuCnt = approx
#        break
    

    
    
    
    
    