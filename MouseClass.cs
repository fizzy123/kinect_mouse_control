using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using MIO = System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Tutorial1
{
    /// <summary>
    /// Start Params (bool Lefty, Rectangle CoverageArea)
    /// </summary>
    public class MouseClass : SkeletonWorkerInterface
    {
        [DllImport("User32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        // http://msdn.microsoft.com/en-us/library/ms646260%28VS.85%29.aspx

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;

        /// <summary>
        /// Simultates the Left or Right Mouse Click
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Left"></param>
        public void Click(int x, int y, bool Left, bool Down)
        {
            if (Left)
            {
                if (Down)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
                }
            }
            else
            {
                if (Down)
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
                }
            }
        }

        internal TimeSpan _ClickDelay = new TimeSpan(0, 0, 1);
        internal DateTime _LastClick = DateTime.Now;

        Double[] pointVelocity = new Double[2] {0,0};

        Boolean extendedClick = false;
        Boolean currentlyGrabbing = false;
        public void DoWork(Microsoft.Kinect.Skeleton Data, Microsoft.Kinect.KinectSensor Kinect, List<object> Params, Boolean clicked, Boolean grabbed)
        {

            Joint _WorkingJoint, _ReferenceJoint;
            Point mouse_pos;
            Double dx, dy;
            GetCursorPos(out mouse_pos);
            _WorkingJoint = Data.Joints[JointType.HandRight];
            _ReferenceJoint = Data.Joints[JointType.ShoulderCenter];

            Double[] pointCoordinates = ExponentialWeightedAvg(_WorkingJoint); // Smoothing function

            //Calculate difference between where mouse is and where hand is
            // Scale up because we don't want to have to move our hand across the full range of the kinect in order to go from one side of the screen to the other
            dx = (1920 * 5 * pointCoordinates[0] - mouse_pos.X);
            dy = (1080 * 5 * (-pointCoordinates[1]) - mouse_pos.Y);
            
            //Set Velocity
            pointVelocity[0] = dx / 10;
            pointVelocity[1] = dy / 10;

            //Uncomment to view process of moving mouse
            //Console.WriteLine("{0}, {1}, {2}; {3}, {4}, {5}", pointCoordinates[0], mouse_pos.X, dx, pointCoordinates[1], mouse_pos.Y, dy);

            //Don't move if the cursor is clicking currently. Done to make sure the cursor doesn't move in the process of clicking.
            if (!extendedClick)
            {
                SetCursorPos(mouse_pos.X + (int)(pointVelocity[0]), mouse_pos.Y + (int)(pointVelocity[1]));
            }

            //Communicates the clicking event to the computer
            if ((clicked) && (mouse_pos.Y != 1079))
            {
                //Prevents rapidly re-clicking. 
                if (DateTime.Now - _LastClick > _ClickDelay)
                {

                    _LastClick = DateTime.Now;
                    if (extendedClick)
                    {
                        Console.WriteLine("Clicked, Right Click");
                        Click(mouse_pos.X, mouse_pos.Y, false, true);
                        Click(mouse_pos.X, mouse_pos.Y, false, false);
                        extendedClick = false;
                    }
                    else
                    {
                        Console.WriteLine("Clicked");
                        Click(mouse_pos.X, mouse_pos.Y, true, true);
                        Click(mouse_pos.X, mouse_pos.Y, true, false);
                        extendedClick = true;
                    }
                }
            }
            else 
            {
                extendedClick = false;
                if ((grabbed) && (mouse_pos.Y != 1079))
                {
                    Click(mouse_pos.X, mouse_pos.Y, true, true);
                    Console.WriteLine("Grabbed");
                    currentlyGrabbing = true;
                }
                if ((!grabbed) && (currentlyGrabbing))
                {
                    Console.WriteLine("UnGrabbed");
                    Click(mouse_pos.X, mouse_pos.Y, true, false);
                    currentlyGrabbing = false;
                }
            }
        }

        public double ExponentialMovingAverage(double[] data, double baseValue)
        {
            double numerator = 0;
            double denominator = 0;

            double average = data.Sum();
            average /= data.Length;

            for (int i = 0; i < data.Length; ++i)
            {
                numerator += data[i] * Math.Pow(baseValue, data.Length - i - 1);
                denominator += Math.Pow(baseValue, data.Length - i - 1);
            }

            numerator += average * Math.Pow(baseValue, data.Length);
            denominator += Math.Pow(baseValue, data.Length);

            return numerator / denominator;
        }

        private readonly Queue<double> _weightedX = new Queue<double>();
        private readonly Queue<double> _weightedY = new Queue<double>();

        private double[] ExponentialWeightedAvg(Joint joint)
        {
            _weightedX.Enqueue(joint.Position.X);
            _weightedY.Enqueue(joint.Position.Y);

            if (_weightedX.Count > 10)
            {
                _weightedX.Dequeue();
                _weightedY.Dequeue();
            }

            double x = ExponentialMovingAverage(_weightedX.ToArray(), 0.9);
            double y = ExponentialMovingAverage(_weightedY.ToArray(), 0.9);
            double[] coordinates = new double[2];
            coordinates[0] = x;
            coordinates[1] = y;
            return coordinates;
        }
    }
}
