using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;

namespace SZMTGJBC_Final_Project
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        string srcName, dstName;

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "D://cup.mp4";

            VideoCapture capture = new VideoCapture(filename);
            capture.Open(filename);

            if (!capture.IsOpened())
            {
                //error in opening the video input
                MessageBox.Show("Unable to open file!");
                return;
            }

            // Create some random colors
            Scalar[] colors = new Scalar[100];
            RNG rng = new RNG(0x1fffff);
            for (int i = 0; i < 100; i++)
            {
                int r = rng.Uniform(0, 256);
                int g = rng.Uniform(0, 256);
                int b = rng.Uniform(0, 256);
                colors[i] = new Scalar(r, g, b);
            }
            Mat old_frame = new Mat(), old_gray = new Mat();


            // Take first frame and find corners in it
            capture.Read(old_frame);
            Cv2.CvtColor(old_frame, old_gray, ColorConversionCodes.BGR2GRAY);
            Point2f[] p0 = Cv2.GoodFeaturesToTrack(old_gray, 100, 0.3, 7, null, 7, false, 0.04);
            Point2f[] p1 = new Point2f[p0.Length];
            // Create a mask image for drawing purposes
            Mat mask = Mat.Zeros(old_frame.Size(), old_frame.Type());
            while (true)
            {
                Mat frame = new Mat(), frame_gray = new Mat();
                capture.Read(frame);
                if (frame.Empty())
                    break;
                Cv2.CvtColor(frame, frame_gray, ColorConversionCodes.BGR2GRAY);
                // calculate optical flow
                byte[] status;
                float[] err;

                TermCriteria criteria = new TermCriteria((CriteriaTypes.Count) | (CriteriaTypes.Eps), 10, 0.03);
                Cv2.CalcOpticalFlowPyrLK(old_gray, frame_gray, p0, ref p1, out status, out err, new OpenCvSharp.Size(15, 15), 2, criteria);
                Point2f[] good_new = new Point2f[p0.Length];
                for (uint i = 0; i < p0.Length; i++)
                {
                    // Select good points
                    if (status[i] == 1)
                    {
                        good_new[i] = p1[i];
                        // draw the tracks
                        Cv2.Line(mask, (OpenCvSharp.Point)p1[i], (OpenCvSharp.Point)p0[i], colors[i], 2);
                        Cv2.Circle(frame, (OpenCvSharp.Point)p1[i], 5, colors[i], -1);
                    }
                }
                Mat img = new Mat();
                Cv2.Add(frame, mask, img);
                Cv2.ImShow("Frame", img);
                int keyboard = Cv2.WaitKey(30);
                if (keyboard == 'q' || keyboard == 27)
                    break;
                // Now update the previous frame and previous points
                old_gray = frame_gray.Clone();
                p0 = good_new;
            }
            }
    }
}