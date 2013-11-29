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
        public void Click(int x, int y, bool Left)
        {
            if (Left)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            }
            else
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
            }
        }

        internal Rectangle _Bounds;
        internal TimeSpan _ClickDelay = new TimeSpan(0, 0, 1);
        internal DateTime _LastClick = DateTime.Now;

        public void Start(List<object> Params)
        {
            if (Params.Count > 0)
            {
                if (Params[1] is Rectangle)
                {
                    _Bounds = (Rectangle)Params[1];
                }
            }
        }

        Double[] old_point_coordinates;

        public void DoWork(Microsoft.Kinect.Skeleton Data, Microsoft.Kinect.KinectSensor Kinect, List<object> Params, Boolean clicked)
        {
            Joint _WorkingJoint;
            Point mouse_pos;
            Double dx, dy;
            GetCursorPos(out mouse_pos);
            _WorkingJoint = Data.Joints[JointType.HandRight];
            //Console.WriteLine(depthPixels[_WorkingJoint.Position.X,_WorkingJoint.Position.Y);

            Double[] point_coordinates = ExponentialWeightedAvg(_WorkingJoint);
            if (old_point_coordinates != null)
            {
                dx = point_coordinates[0] - old_point_coordinates[0];
                dy = point_coordinates[1] - old_point_coordinates[1];
                //Console.WriteLine("{0},{1}", dx, dy);
                
                if (Math.Abs(dx) > 0.01)
                {
                    dx = dx * 10000;
                } 
                else if (Math.Abs(dx) > 0.00001) 
                {
                    dx = dx * 5000;
                }
                if (Math.Abs(dy) > 0.01)
                {
                    dy = dy * 10000;
                }
                else if (Math.Abs(dy) > 0.00001)
                {
                    dy = dy * 5000;
                }
                if (!clicked)
                {
                    SetCursorPos(mouse_pos.X + (int)(dx), mouse_pos.Y - (int)(dy));
                }
                Console.WriteLine("{0}, {1}", dx, dy);
            }
            old_point_coordinates = point_coordinates;

            /// VERY simple click check.

            if (clicked)
            {
                if (DateTime.Now - _LastClick > _ClickDelay)
                {
                    Click(mouse_pos.X, mouse_pos.Y, true);
                    _LastClick = DateTime.Now;
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
