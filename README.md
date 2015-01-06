kinect_mouse_control
====================

https://www.youtube.com/watch?v=D_6ha00_lbM&list=UUB1620vx3tyS5H7RD-_nybg

The goal was to create an interface that I could use to control the mouse by waving my hand around with the Kinect. I used the Kinect SDK and a Kinect for Windows, which was necessary to get close enough to use it without stepping away from my computer. It works pretty well, except that the Kinect stream have a maximum resolution of 1280x960 while my screen is considerably more than that. I'm hoping that it'll work better with the new Kinect that's coming out soon.

#Motion

Getting the motion smooth was actually the most difficult part of the project. The position of the mouse is done by tracking the right hand of the closest skeleton in front of the Kinect. At first, I tried simply mapping the X and Y coordinate of that hand to the X and Y coordinate of the mouse, but that gives you jumpy data. I tried making it better by telling it to focus on differences in distance from one frame to another, which helped a little bit, but not enough. I finally ended up telling the mouse to accelerate towards the position it should be at, which completely removed the jumpiness and also made it pretty reliable.

#Controls

There are three main controls at the moment: right click, left click and drag and drop. For all of these controls, the program detects the point closest to the Kinect and looks at the area composed of all points that are within 100 millimeters of the depth of that closest point. It keeps a running average of the area, width and height. Events are triggered when these values stray too far from the average. These values do not contribute to their running averages during the time events are triggered. Left clicking is triggered when the area and height are significantly less than the average. Physically, this is represented by "tapping" the air. Right clicking is triggered when a left click is registered for some amount of time. This is taken from the design of touchscreens. Drag and drop, which is just extended left clicking, is triggered when the user closes his/her hand into a fist. This is detected through two methods. The first is by seeing that the width is only slightly larger than the height. The second is by counting the number of fingers detected. This is done by taking x values of the row of pixels in the area of closest proximity that is ten pixels down from the highest point of the area of closest proximity and checking those x values for gaps. Drag and drop also has a threshold that it must cross before setting the mouse to drag or drop. Namely, the program must detect five frames of dragging or of dropping before it actually drags or drops.

#Future Development

This is going to be on the back burner until I get a new Kinect. I might also add application specific gestures or tell the mouse to have a higher likelihood of approaching buttons that are on the screen.
