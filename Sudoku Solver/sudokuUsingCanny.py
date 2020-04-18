#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Mon Mar 30 23:10:33 2020

@author: kaytlyn
"""
import cv2
import numpy as np
import imutils
from  matplotlib import pyplot as plt

# Read the target image
image = cv2.imread("SudokuImage.png",0)

# RESIZING THE IMAGE
#scale_percentage = 40 
#height = int(image.shape[0]*scale_percentage / 100 )
#width = int(image.shape[1]*scale_percentage / 100)
#dim = (height, width)
#
#image = cv2.resize(image,  dim)

image = cv2.bilateralFilter(image, 11, 17, 17)



#ret,th1 = cv2.threshold(image, 150,255, cv2.THRESH_BINARY)

th2 = cv2.adaptiveThreshold(image, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, \
                                cv2.THRESH_BINARY, 11,5 )
cv2.imshow('th2', th2)
cv2.waitKey(0)
cv2.destroyAllWindows()

