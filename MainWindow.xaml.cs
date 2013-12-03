using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using MIO = System.IO;
//extern Declare Auto Function SetCursorPos Lib "user32.dll" (ByVal x As Integer, ByVal y As Integer) As Integer

namespace Tutorial1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        List<SkeletonWorkerInterface> _Workers = new List<SkeletonWorkerInterface>();
        System.Drawing.Color _CurrentColor = System.Drawing.Color.White;
        KinectSensor _Runtime;
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private List<short> depthSquare;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] colorPixels;

        private Skeleton FirstSkeleton;

        private Boolean clicked = false;
        private Boolean grabbed = false;
        public MainWindow()
        {
            InitializeComponent();         
        }

        bool _IsFirstSkeleton = true;

        void _Runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    int SkeletonCount = skeletonFrame.SkeletonArrayLength;
                    Skeleton[] skeletonData = new Skeleton[SkeletonCount];
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    foreach (var SData in skeletonData)
                    {
                        if (SData.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            FirstSkeleton = SData;
                            break;
                        }
                    }

                    if (FirstSkeleton != null)
                    {
                        if (_IsFirstSkeleton)
                        {
                            if (SimpleDetection.CenterDevice(.10f, .3f, _Runtime, FirstSkeleton.Joints[JointType.Head].Position))
                            {
                                _IsFirstSkeleton = false;
                            }
                        }

                        /// This occasionally throws a Generic GDI Error.. 
                        //SkeletonImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(MKinectDrawing.DrawSkeleton(320, 240, skeletonFrame, _CurrentColor).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                        foreach (SkeletonWorkerInterface _Worker in _Workers)
                        {
                            _Worker.DoWork(FirstSkeleton, _Runtime, null, clicked, grabbed, lowestDepthPoint);
                        }

                    }
                }
            }
        }

        double oldAreaAvg = 0;
        List<int> areaList = new List<int>();

        double oldHeightAvg = 0;
        List<int> heightList = new List<int>();

        int clickCount = 0;
        int grabCount = 0;

        double[] lowestDepthPoint = new double[2];

        void _Runtime_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;
                    int x = 0, y = 0;
                    int lowDepth = -1, area = 0;
                    int lowestDepthX = 0, lowestDepthY = 0;
                    Boolean fingers;

                    // Detect point of lowest depth. It will be assume that this is the hand doing the clicking.
                    for (int i = 0; i < this.depthPixels.Length; ++i)
                    {
                        if ((lowDepth == -1) && (this.depthPixels[i].Depth != 0))
                        {
                            lowDepth = this.depthPixels[i].Depth;
                        }
                        else if ((lowDepth > this.depthPixels[i].Depth) && (this.depthPixels[i].Depth != 0))
                        {
                            lowDepth = this.depthPixels[i].Depth;
                            lowestDepthX = i % 640;
                            lowestDepthY = (i - x) / 640;
                        }
                    }

                    // Loop through the pixels again to determine the area of the hand, as well as fill in the depth bitmap.
                    int colorPixelIndex = 0;
                    HashSet<int> heightSet = new HashSet<int>();
                    HashSet<int> widthSet = new HashSet<int>();

                    int fingerCount = 0;
                    int minY = 0;
                    int lastX = 0;
                    for (int i = 0; i < this.depthPixels.Length; ++i)
                    {
                        x = i % 640;
                        y = (i - x) / 640;
                        if ((lowestDepthY < (y + 50)) && (lowestDepthY >= (y - 150)) && (lowestDepthX < (x + 100)) && (lowestDepthX >= (x - 100)))
                        {
                            
                            // Get the depth for this pixel
                            short depth = depthPixels[i].Depth;

                            // To convert to a byte, we're discarding the most-significant
                            // rather than least-significant bits.
                            // We're preserving detail, although the intensity will "wrap."
                            // Values outside the reliable depth range are mapped to 0 (black).

                            // Note: Using conditionals in this loop could degrade performance.
                            // Consider using a lookup table instead when writing production code.
                            // See the KinectDepthViewer class used by the KinectExplorer sample
                            // for a lookup table example.
                            byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                            if ((lowDepth + 100 > depthPixels[i].Depth) && (depthPixels[i].Depth != 0))
                            {
                                //increment hand area
                                area++;

                                //increment hand height.
                                heightSet.Add(y);
                                widthSet.Add(x);

                                // Write out blue byte
                                this.colorPixels[colorPixelIndex++] = 0;
                                
                                //Finger Count
                                if (minY == 0)
                                {
                                    minY = y;
                                }

                                if (y == minY + 10)
                                {
                                    if (lastX != 0)
                                    {
                                        if (lastX != x-1)
                                        {
                                            fingerCount++;
                                        }
                                    }
                                    lastX = x;
                                }
                            }
                            else
                            {
                                this.colorPixels[colorPixelIndex++] = intensity;
                            }

                            // Write out green byte
                            this.colorPixels[colorPixelIndex++] = intensity;

                            // Write out red byte                        
                            this.colorPixels[colorPixelIndex++] = intensity;

                            // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                            // If we were outputting BGRA, we would write alpha here.
                            ++colorPixelIndex;
                        }
                    }

                    //Code to require a delay before registering a click.
                    if (clickCount == 2)
                    {
                        clicked = true;
                        clickCount = 0;
                    }
                    else if ((!grabbed) && (oldAreaAvg - 1000 > area) && (oldHeightAvg + 50 < heightSet.Count()) || (oldHeightAvg - 50 > heightSet.Count())) 
                    {
                        clickCount++;
                    }
                    else
                    {
                        clickCount = 0;
                        clicked = false;

                        //Average area and height
                        double areaAvg;
                        if (areaList.Count == 10)
                        {
                            areaAvg = areaList.Average();
                            areaList.RemoveAt(0);
                            areaList.Add(area);
                            //Console.WriteLine("{0}", areaAvg);
                            oldAreaAvg = areaAvg;
                        }
                        else
                        {
                            areaList.Add(area);
                        }

                        double heightAvg;
                        if (heightList.Count == 10)
                        {
                            heightAvg = heightList.Average();
                            heightList.RemoveAt(0);
                            heightList.Add(heightSet.Count());
                            //Console.WriteLine("{0}", areaAvg);
                            oldHeightAvg = heightAvg;
                        }
                        else
                        {
                            heightList.Add(heightSet.Count());
                        }
                    }

                    if ((!clicked) && (Math.Abs(heightSet.Count() - widthSet.Count() + 5) < 15) && (fingerCount < 1) && (grabCount != 5))
                    {
                        grabCount++;
                    }
                    else if (grabCount != 0)
                    {
                        grabCount--;
                    }

                    if ((grabbed) && (grabCount == 0))
                    {
                        grabbed = false;
                    }
                    if ((!grabbed) && (grabCount == 5))
                    {
                        grabbed = true;
                    }

                    //Console.WriteLine("{0}, {1} : {2} - {3}", heightSet.Count(), widthSet.Count(), grabbed, clicked);

                    //if (grabbed)
                    //{
                    //    Console.WriteLine("Positive");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Negative");
                    //}
                    Int32Rect rectangle = new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight);
                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        rectangle,
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                    lowestDepthPoint[0] = lowestDepthX;
                    lowestDepthPoint[1] = lowestDepthY;
                 }
            }
        }

        void _Runtime_VideoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
         //   VideoStreamImage.Source = e.ImageFrame.ToBitmapSource();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _Runtime.Stop();
        }

        private void StartKinect_Click(object sender, RoutedEventArgs e)
        {
            /// HARD CODING TO 1 KINECT
            /// 
            if (KinectSensor.KinectSensors.Count() == 0)
            {
                throw new Exception("No Kinects Connected!");
            }

            if (KinectSensor.KinectSensors[0].Status != KinectStatus.Connected)
            {
                throw new Exception("No Kinects Ready!");

            }
                        
            /// Grab the First Kinect
            _Runtime = KinectSensor.KinectSensors[0];

            // Allocate space to put the color pixels we'll create
            this.colorPixels = new byte[40000 * sizeof(int)];

            // This is the bitmap we'll display on-screen
            this.colorBitmap = new WriteableBitmap(200, 200, 96.0, 96.0, PixelFormats.Bgr32, null);

            // Set the image we display to point to the bitmap where we'll put the image data
            this.DepthImage.Source = this.colorBitmap;

            // Set Kinect to work in near mode
            _Runtime.DepthStream.Range = DepthRange.Near;
            _Runtime.SkeletonStream.EnableTrackingInNearRange = true;

            this.depthPixels = new DepthImagePixel[this._Runtime.DepthStream.FramePixelDataLength];

            /// Initialize with Color and Depth and SkeletalTracking
            _Runtime.ColorStream.Enable();
            _Runtime.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _Runtime.SkeletonStream.Enable();
            _Runtime.Start();
            /// Setup Smoothing Parameters

            TransformSmoothParameters _TransformSmooth = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.5f,
                MaxDeviationRadius = 0.04f
            };

            _Runtime.SkeletonStream.Enable(_TransformSmooth);

            /// Add Event Handlers For Video and Depth
            _Runtime.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(_Runtime_VideoFrameReady);
            _Runtime.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(_Runtime_DepthFrameReady);

            /// Add Event Handler For Skeleton Data
            _Runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(_Runtime_SkeletonFrameReady);

            #region Add Mouse Support

            MouseClass _MClass = new MouseClass();

            List<object> _MouseParams = new List<object>();
            
            // Assume Righty
            _MouseParams.Add(true);
            System.Drawing.Rectangle _R = new System.Drawing.Rectangle();
           
            _R.Width = 1920;
            _R.Height = 1080;
            _R.X = 0;
            _R.Y = 0;
            
            _MouseParams.Add(_R);

            _MClass.Start(_MouseParams);

            _Workers.Add(_MClass);

            #endregion

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Works");
        }

        private void SetRedButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentColor = System.Drawing.Color.Red;
        }

        private void SetGreenButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentColor = System.Drawing.Color.Green;
        }

        private void SetBlueButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentColor = System.Drawing.Color.Blue;
        }

    }
}
