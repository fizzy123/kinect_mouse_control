using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using MIO = System.IO;
using System.Drawing;

namespace Tutorial1
{
    public interface SkeletonWorkerInterface
    {

        void Start(List<object> Params);

        void DoWork(Skeleton Data, KinectSensor Kinect, List<object> Params, Boolean clicked, double[] lowestDepthPoint);

    }
}
