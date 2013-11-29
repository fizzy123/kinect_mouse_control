using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using MIO = System.IO;

namespace Tutorial1
{
    public class SimpleDetection
    {
        static DateTime _LastMovementTry = DateTime.Now;
        static int _TryCount = 0;
        static int _MaxTries = 20;

        /// <summary>
        /// Try and Center the Device based on Head Position.  Will try to move a max of 20 times.
        /// </summary>
        /// <param name="MinHeadOffsetFromTop">Offset From Top Of Camera Field (i.e  .2)</param>
        /// <param name="MaxHeadOffsetFromTop">Max Offset From Top (i.e .4)</param>
        /// <param name="Kinect">Runtime Object</param>
        /// <param name="HeadPosition">Current Head Position</param>
        /// <returns>True if within bounds.  False if needs testing.</returns>
        public static bool CenterDevice(float MinHeadOffsetFromTop, float MaxHeadOffsetFromTop, KinectSensor Kinect, SkeletonPoint HeadPosition)
        {
            if (_TryCount > _MaxTries) { return false; }

            if (DateTime.Now - _LastMovementTry > new TimeSpan(0, 0, 1))
            {
                _TryCount++;

                _LastMovementTry = DateTime.Now;

                if (1 - MinHeadOffsetFromTop < HeadPosition.Y)
                {
                    Kinect.ElevationAngle += 1;
                    return false;

                }
                if (1 - MaxHeadOffsetFromTop > HeadPosition.Y)
                {
                    //Kinect.ElevationAngle -= 1;
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;            
        }

        //public SimpleDetection(

    }
}
