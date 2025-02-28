import imutils
import cv2

class ShapeDetector:
    def __init__(self):
        pass
 
    def detect(self, c, aproximationFactor = 0.04):
        # initialize the shape name and approximate the contour
        shape = "unidentified"
        peri = cv2.arcLength(c, True)
        approx = cv2.approxPolyDP(c, aproximationFactor * peri, True)

                # if the shape is a triangle, it will have 3 vertices
        if len(approx) == 3:
            shape = "triangle"
 
        # if the shape has 4 vertices, it is either a square or
        # a rectangle
        elif len(approx) == 4:
            # compute the bounding box of the contour and use the
            # bounding box to compute the aspect ratio
            (x, y, w, h) = cv2.boundingRect(approx)
            ar = w / float(h)
 
            # a square will have an aspect ratio that is approximately
            # equal to one, otherwise, the shape is a rectangle
            shape = "square" if ar >= 0.95 and ar <= 1.05 else "rectangle"
 
        # if the shape is a pentagon, it will have 5 vertices
        elif len(approx) == 5:
            shape = "pentagon"

        elif len(approx) == 6:
            shape = "hexagon"

        elif len(approx) == 8:
            shape = "octagon"
 
        # otherwise, we assume the shape is a circle
        else:
            shape = "circle"
 
        # return the name of the shape
        return shape