kinect_mouse_control
====================

This application allows you to control your mouse with your hand. So far, all it does is click and move. 

The movement is managed by taking the coordinate of the right hand joint, mapping that coordinate to a point on the 
monitor, and accelerating the mouse toward that position. Because the kinect streams will have lower resolutions than
most large screens, fast and precise movement is not possible, but maybe the next iteration of the kinect will resolve
that.

The clicking is managed by tracking the "area" of the hand. This area is defined as all points within 100 millimeters of
the lowest depth. We take the total area and the delta-y of the area and if both of these are significantly less than the
running average, a click is reported to the system. 

FEATURES:

Left-click
Right-click  
Grabbing (Drag and drop)  
