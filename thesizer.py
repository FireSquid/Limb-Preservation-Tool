from scipy.spatial import distance as dist
from imutils import perspective
from imutils import contours
import numpy as np
import argparse
import imutils
import cv2
import time
import os

def cv(filename, width,in_dest,out_dest, isSmaller ):
    if(os.path.exists(filename)):
        #img = cv2.imread(filename)
        img = cv2.imread(filename)
        #image = cv2.fastNlMeansDenoisingColored(img,None,10,10,7,21)
        #cv2.imshow('img', image)
        #cv2.waitKey(0)
        #cv2.destroyAllWindows()
        #blur = cv2.bilateralFilter(image,9,75,75)
        if(img is not None):
            #gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        else:
            print("There is no image")
        #cv2.imshow('img', gray)
        #cv2.waitKey(0)
        #cv2.destroyAllWindows()
        #gray = cv2.GaussianBlur(gray, (11, 11), 0)

        # perform edge detection + dilation + erosion to close gaps_bt_edges
        edge_detect = cv2.Canny(gray, 100, 150) #change min and max to mess with edges
        edge_detect = cv2.dilate(edge_detect, None, iterations=1)
        edge_detect = cv2.erode(edge_detect, None, iterations=1)
        #cv2.imshow('img', edge_detect)
        #cv2.waitKey(0)
        #cv2.destroyAllWindows()

        # find contours in the edge map
        cntours = cv2.findContours(edge_detect.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        cntours = imutils.grab_contours(cntours)
        # sort contours left-to-right
        #(cntours, _) = contours.sort_contours(cntours)
        #sort contours by area
        cntours = sorted(cntours, key=cv2.contourArea, reverse=True)
        pixel_to_size = None
        # function for finding the midpoint
        def mdpt(A, B):
            return ((A[0] + B[0]) * 0.5, (A[1] + B[1]) * 0.5)

        # loop over the contours individually
        id = 1
        if(isSmaller == True):
            cntours[0], cntours[1] = cntours[1], cntours[0]
        for c in cntours: 
            #print(c)
            if cv2.contourArea(c) < 100: #ignore/fly through contours that are not big enough
                continue   # compute the rotated bounding box of the contour; should_handle cv2 or cv3..
            if(id == 1):
                orig = img.copy()
            if(id == 3):
                break
            #else:
            #    orig = tempimg.copy()
            bbox = cv2.minAreaRect(c)
            bbox = cv2.cv.boxPoints(bbox) if imutils.is_cv2() else cv2.boxPoints(bbox)
            bbox = np.array(bbox, dtype="int")
            # order the contours and draw bounding box
            bbox = perspective.order_points(bbox)
            cv2.drawContours(orig, [bbox.astype("int")], -1, (0, 255, 0), 2)
            # loop over the original points in bbox and draw them; 5px red dots
            for (x, y) in bbox:
                cv2.circle(orig, (int(x), int(y)), 5, (0, 0, 255), -1)
            # unpack the ordered bounding bbox; find midpoints
            (tl, tr, br, bl) = bbox
            (tltrX, tltrY) = mdpt(tl, tr)
            (blbrX, blbrY) = mdpt(bl, br)
            (tlblX, tlblY) = mdpt(tl, bl)
            (trbrX, trbrY) = mdpt(tr, br)

            # draw the mdpts on the image (blue);lines between the mdpts (yellow)
            cv2.circle(orig, (int(tltrX), int(tltrY)), 5, (255, 0, 0), -1)
            cv2.circle(orig, (int(blbrX), int(blbrY)), 5, (255, 0, 0), -1)
            cv2.circle(orig, (int(tlblX), int(tlblY)), 5, (255, 0, 0), -1)
            cv2.circle(orig, (int(trbrX), int(trbrY)), 5, (255, 0, 0), -1)

            cv2.line(orig, (int(tltrX), int(tltrY)), (int(blbrX), int(blbrY)),(0, 255, 255), 2)
            cv2.line(orig, (int(tlblX), int(tlblY)), (int(trbrX), int(trbrY)),(0, 255, 255), 2)
            # compute the Euclidean distances between the mdpts
            dA = dist.euclidean((tltrX, tltrY), (blbrX, blbrY))
            dB = dist.euclidean((tlblX, tlblY), (trbrX, trbrY))
            #print("dA is " + dA + "dB is " + dB)
            # use pixel_to_size ratio to compute object size
            if pixel_to_size is None:
                pixel_to_size = dB / width
            
            distA = dA / pixel_to_size
            distB = dB / pixel_to_size
            print("distA is " + str(distA) + " distB is " + str(distB)) 
            # draw the object sizes on the image
            cv2.putText(orig, "{:.1f}in".format(distA), (int(tltrX - 10), int(tltrY - 10)), cv2.FONT_HERSHEY_DUPLEX,0.55, (255, 255, 255), 2)
            cv2.putText(orig, "{:.1f}in".format(distB), (int(trbrX + 10), int(trbrY)), cv2.FONT_HERSHEY_DUPLEX,0.55, (255, 255, 255), 2)
            cv2.imwrite("./output.png", orig)
            id = id + 1

 
        os.rename(filename, in_dest)
        os.rename("output.png", out_dest) 

    else:
        print("Error: Filename specified does not exist within current path")        
