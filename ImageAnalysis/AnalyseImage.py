#!/usr/bin/python3
import sys
import os
import json
import imutils
import cv2
import getopt
import numpy as np

from ShapeDetector import ShapeDetector

def normalizeData(mapData, removeSpace):
    print("Normalising data")
    print("Finding the lowest values for position and highest values for scale.")

    minX = sys.maxsize
    minY = sys.maxsize
    maxScale = {}

    for data in mapData:
        if data["shape"] not in maxScale:
            maxScale[data["shape"]] = 0

        if data["scale"] > maxScale[data["shape"]]:
            maxScale[data["shape"]] = data["scale"]
        if data["x"] < minX:
            minX = data["x"]
        if data["y"] < minY:
            minY = data["y"]

        data['colorHex'] = '#%02x%02x%02x' % (data["color"][0], data["color"][1], data["color"][2])

    print('MinX value:{}\nMinY value:{}\nMaxScale:{}'.format(minX, minY, maxScale))
    print('Updating values')
    if removeSpace:
        print('Will remove empty spacing')
    else:
        print('Empty space will be preserved')

    for data in mapData:
        data["scale"] = float(data["scale"]) / float(maxScale[data["shape"]])
        if removeSpace:
            data["x"] = float(data["x"]) - float(minX)
            data["y"] = float(data["y"]) - float(minY)

def AnalyseImage(path, outputPath, removeSpace, aproximationFactor = 0.04):
    data = path
    print("Analysing image at location:" + str(data))
    print("Loading image....")
    # load the image and resize it to a smaller factor so that
    # the shapes can be approximated better
    image = cv2.imread(data)
    #create backup so we can read collor dirrectly from unmodified image
    backup = image.copy()
    print("Resizing image")
    resized = imutils.resize(image, width=2048)
    ratio = image.shape[0] / float(resized.shape[0])

    # convert the resized image to grayscale, blur it slightly,
    # and threshold it
    print("Changing colors to grayscale.")
    gray = cv2.cvtColor(resized, cv2.COLOR_BGR2GRAY)
    blurred = cv2.GaussianBlur(gray, (5, 5), 0)
    thresh = cv2.threshold(blurred, 60, 255, cv2.THRESH_BINARY)[1]

    # find contours in the thresholded image and initialize the
    # shape detector
    cnts = cv2.findContours(thresh.copy(), cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
    #cv2.findContours(gray.copy(), cv2.RETR_EXTERNAL,	cv2.CHAIN_APPROX_NONE)
    cnts = imutils.grab_contours(cnts)

    mapData = []
    sd = ShapeDetector()
    # loop over the contours
    for c in cnts:
        # compute the center of the contour, then detect the name of the
        # shape using only the contour
        M = cv2.moments(c)
        
        if M["m00"] == 0:
            continue
        #calculate center of object
        resizedX = int(M["m10"] / M["m00"])
        resizedY = int(M["m01"] / M["m00"])
        cX = int((M["m10"] / M["m00"]) * ratio)
        cY = int((M["m01"] / M["m00"]) * ratio)

        shape = sd.detect(c, aproximationFactor)
        rValue = int(backup[cY, cX, 2])
        gValue = int(backup[cY, cX, 1])
        bValue = int(backup[cY, cX, 0])
        
        # multiply the contour (x, y)-coordinates by the resize ratio,
        # then draw the contours and the name of the shape on the image
        c = c.astype("float")
        c *= ratio
        c = c.astype("int")
        
        (x,y),radius = cv2.minEnclosingCircle(c)
        center = (int(x),int(y))
        radius = int(radius)

        #used for rotation detection 
        # supposadly the last item in returned struct is angle in degrees
        rect = cv2.minAreaRect(c)
        rotation = 0.0
        if shape is not "circle":
            rotation = float(rect[-1])
            #lets draw rotation boxes
            box = cv2.boxPoints(rect)
            box = np.int0(box)
            im = cv2.drawContours(image,[box],0,(0,0,255),2)

        cv2.drawContours(image, [c], -1, (0, 255, 0), 2)
        cv2.putText(image, shape, center, cv2.FONT_HERSHEY_SIMPLEX,
            0.5, (255, 255, 255), 2)

        mapData.append(
            {
                "shape" : shape,
                "rotation" : rotation, # might be unprecise
                "color" : [rValue, gValue, bValue],
                "scale" : radius,
                "x" : cX,
                "y" : cY,
            }
        )
    
    normalizeData(mapData, removeSpace)
    with open(outputPath + '.json', 'w') as outfile:
        json.dump({ "Items" : mapData }, outfile)

    #show the image with marked objects
    cv2.imwrite(outputPath, image)

if __name__ == "__main__":
    outputPath = 'test-output.jpg'
    try:
        opts, args = getopt.getopt(sys.argv[1:], "i:o:a:r", ["input=", "output=", "aproximation=","remove"])
        inputFile = ""
        aproximationFactor = 4 #Might have issues with this value
        removeSpace = False
        for o, a in opts:
            if o in ("-i", "--input"):
                inputFile = os.path.abspath(a)
            elif o in ("-o", "--output"):
                outputPath = os.path.abspath(a)
            elif o in ("-r", "--remove"):
                removeSpace = True
            elif o in ("-a", "--aproximation"):
                aproximationFactor = float(a)

        AnalyseImage(inputFile, outputPath, removeSpace, aproximationFactor)
    except getopt.GetoptError as error:
        print(str(error))
        sys.exit(1)
