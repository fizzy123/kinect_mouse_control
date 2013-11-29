using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Drawing;

namespace Tutorial1
{
    public static class MKinectDrawing
    {
/// <summary>
/// Just Draw The Skeleton To A Bitmap
/// </summary>
/// <param name="width"></param>
/// <param name="height"></param>
/// <param name="FrameData"></param>
/// <returns></returns>
public static Bitmap DrawSkeleton(int width, int height, Microsoft.Kinect.SkeletonFrame FrameData, Color BackColor)
{
    Bitmap _TmpReturn = new Bitmap(width, height);
    Graphics _G = Graphics.FromImage(_TmpReturn);

    _G.Clear(BackColor);
    int SkeletonCount = FrameData.SkeletonArrayLength;
    Skeleton[] skeletonData = new Skeleton[SkeletonCount];
    FrameData.CopySkeletonDataTo(skeletonData);
    foreach (var iSkeleton in skeletonData)
    {
        if (iSkeleton.TrackingState == Microsoft.Kinect.SkeletonTrackingState.Tracked)
        {
            var HeadJoint = iSkeleton.Joints[JointType.Head].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, HeadJoint.Position.X, HeadJoint.Position.Y, 20, 20);

            var AnkleLeftJoint = iSkeleton.Joints[JointType.AnkleLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, AnkleLeftJoint.Position.X, AnkleLeftJoint.Position.Y, 10, 10);

            var AnkleRightJoint = iSkeleton.Joints[JointType.AnkleRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, AnkleRightJoint.Position.X, AnkleRightJoint.Position.Y, 10, 10);

            var ElbowLeftJoint = iSkeleton.Joints[JointType.ElbowLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, ElbowLeftJoint.Position.X, ElbowLeftJoint.Position.Y, 10, 10);

            var ElbowRightJoint = iSkeleton.Joints[JointType.ElbowRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, ElbowRightJoint.Position.X, ElbowRightJoint.Position.Y, 10, 10);

            var FootLeftJoint = iSkeleton.Joints[JointType.FootLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, FootLeftJoint.Position.X, FootLeftJoint.Position.Y, 10, 10);

            var FootRightJoint = iSkeleton.Joints[JointType.FootRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, FootRightJoint.Position.X, FootRightJoint.Position.Y, 10, 10);

            var HandLeftJoint = iSkeleton.Joints[JointType.HandLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, HandLeftJoint.Position.X, HandLeftJoint.Position.Y, 10, 10);

            var HandRightJoint = iSkeleton.Joints[JointType.HandRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, HandRightJoint.Position.X, HandRightJoint.Position.Y, 10, 10);

            var HipCenterJoint = iSkeleton.Joints[JointType.HipCenter].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, HipCenterJoint.Position.X, HipCenterJoint.Position.Y, 10, 10);

            var HipLeftJoint = iSkeleton.Joints[JointType.HipLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, HipLeftJoint.Position.X, HipLeftJoint.Position.Y, 10, 10);

            var HipRightJoint = iSkeleton.Joints[JointType.HipRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, HipRightJoint.Position.X, HipRightJoint.Position.Y, 10, 10);

            var KneeLeftJoint = iSkeleton.Joints[JointType.KneeLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, KneeLeftJoint.Position.X, KneeLeftJoint.Position.Y, 10, 10);

            var KneeRightJoint = iSkeleton.Joints[JointType.KneeRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, KneeRightJoint.Position.X, KneeRightJoint.Position.Y, 10, 10);

            var ShoulderCenterJoint = iSkeleton.Joints[JointType.ShoulderCenter].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, ShoulderCenterJoint.Position.X, ShoulderCenterJoint.Position.Y, 10, 10);

            var ShoulderLeftJoint = iSkeleton.Joints[JointType.ShoulderLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, ShoulderLeftJoint.Position.X, ShoulderLeftJoint.Position.Y, 10, 10);

            var ShoulderRightJoint = iSkeleton.Joints[JointType.ShoulderRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, ShoulderRightJoint.Position.X, ShoulderRightJoint.Position.Y, 10, 10);

            var WristLeftJoint = iSkeleton.Joints[JointType.WristLeft].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, WristLeftJoint.Position.X, WristLeftJoint.Position.Y, 10, 10);

            var WristRightJoint = iSkeleton.Joints[JointType.WristRight].ScaleTo(width, height);
            _G.FillEllipse(Brushes.Black, WristRightJoint.Position.X, WristRightJoint.Position.Y, 10, 10);
        }
    }

    _G.Dispose();
    return _TmpReturn;
}

    }
}
