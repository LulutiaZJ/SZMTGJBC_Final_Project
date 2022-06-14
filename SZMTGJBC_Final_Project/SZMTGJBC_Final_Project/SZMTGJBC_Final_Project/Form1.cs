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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string srcName, dstName;
        Mat img, result;
        Mat img2, change, img3;
        Mat resultX, resultY;
        Mat hist;
        Bitmap myImage, bitmap;
        int PBwidth, PBheight;
        bool beginPaint = false, beginMove = false;
        int currentXpos, currentYpos;
        static Mat src_gray = new Mat();
        static int thresh = 100;
        static RNG rng = new RNG(12345);



        #region 文件操作

        /// <summary>
        /// 打开图片
        /// </summary>
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Title = "请选择文件";
            openFileDialog2.InitialDirectory = @"D:\";                //默认路径是D:\
            openFileDialog2.Filter = "图片(*.jpg,*.gif,*.bmp,*.png)|*.jpg;*.gif;*.bmp;*.png";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog2.FileName != "")
                {
                    srcName = openFileDialog2.FileName;
                    img = new Mat(srcName);
                    pictureBox1.Image = BitmapConverter.ToBitmap(img);
                    result = img;
                    button1.Enabled = true;
                }
            }
        }

        /// <summary>
        /// 另存为文件
        /// </summary>
        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveData = new SaveFileDialog();    //以保存文件的方式打开
            SaveData.Title = "请选择路径";                      //标题
            SaveData.InitialDirectory = @"D:\";                //默认路径是D:\
            SaveData.Filter = "图片(*.jpg,*.gif,*.bmp,*.png)|*.jpg;*.gif;*.bmp;*.png";
            SaveData.FileName = "img";                     //默认文件名是img，后缀名会自动补齐
            if (SaveData.ShowDialog() == DialogResult.OK)
            {    //如果选定路径按下保存按钮
                dstName = SaveData.FileName;                   //script赋值为选择保存的路径
                bool isSuc = Cv2.ImWrite(dstName, result);
                if (isSuc) MessageBox.Show("保存成功！");
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        #endregion

        #region 基本图像处理

        /// <summary>
        /// 亮度调节
        /// </summary>
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            float a = Convert.ToSingle(trackBar1.Value / 100.0);
            if (img2 == null && result != null)
            {
                change = result;
                img2 = change;
            }
            if (img2 != null)
            {
                img2 = change * a;
                Bitmap bitmap1 = BitmapConverter.ToBitmap(img2);
                pictureBox1.Image = bitmap1;
                result = img2;
            }
        }

        /// <summary>
        /// 鼠标轨迹绘制
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "开始绘制轨迹")
            {
                beginPaint = true;
                button2.Text = "结束绘制轨迹";
                bitmap = new Bitmap(pictureBox1.Image);
                myImage = bitmap;
            }
            else
            {
                result = BitmapConverter.ToMat(myImage);
                beginPaint = false;
                button2.Text = "开始绘制轨迹";
            }
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (beginPaint == true)
            {
                if (e.Button == MouseButtons.Left)
                {
                    beginMove = true;
                    currentXpos = e.X;
                    currentYpos = e.Y;
                }
            }
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (beginMove)
            {
                Graphics g = Graphics.FromImage(myImage);
                Pen myPen = new Pen(Color.Black, 2);
                g.DrawLine(myPen, currentXpos, currentYpos, e.X, e.Y);
                pictureBox1.Image = myImage;
                g.Dispose();
                currentYpos = e.Y;
                currentXpos = e.X;
            }
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beginMove = false;
                currentXpos = 0;
                currentYpos = 0;
            }
        }

        /// <summary>
        /// 图片缩放
        /// </summary>
        private void trackBar6_ValueChanged(object sender, EventArgs e)
        {
            double E = (double)trackBar6.Value / 50 + 0.1;
            Cv2.Resize(img, result, new OpenCvSharp.Size(), E, E);
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 灰度图
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            Cv2.CvtColor(img, result, ColorConversionCodes.BGR2GRAY);
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 二值化
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            Cv2.Threshold(result, result, 125, 255, ThresholdTypes.Binary);
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        #endregion

        #region 图像滤波


        /// <summary>
        /// 均值滤波
        /// </summary>
        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            int b = 2 * (int)(trackBar2.Value / 10) + 1;
            if (img2 == null && result != null)
            {
                change = result;
                img2 = change;
            }
            if (img2 != null)
            {
                Cv2.Blur(change, img2, new OpenCvSharp.Size(b, b));
                Bitmap bitmap1 = BitmapConverter.ToBitmap(img2);
                pictureBox1.Image = bitmap1;
                result = img2;
            }
        }

        /// <summary>
        /// 中值滤波
        /// </summary>
        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            int c = 2 * (int)(trackBar3.Value / 10) + 1;
            if (img2 == null && result != null)
            {
                change = result;
                img2 = change;
            }
            if (img2 != null)
            {
                Cv2.MedianBlur(change, img2, c);
                Bitmap bitmap1 = BitmapConverter.ToBitmap(img2);
                pictureBox1.Image = bitmap1;
                result = img2;
            }
        }

        /// <summary>
        /// 腐蚀
        /// </summary>
        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            int d = (int)(trackBar4.Value / 10) + 1;
            Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(d, d),
                    new OpenCvSharp.Point(-1, -1));
            if (img2 == null && result != null)
            {
                change = result;
                img2 = change;
            }
            if (img2 != null)
            {
                Cv2.Erode(change, img2, element);
                Bitmap bitmap1 = BitmapConverter.ToBitmap(img2);
                pictureBox1.Image = bitmap1;
                result = img2;
            }
        }

        /// <summary>
        /// 膨胀
        /// </summary>
        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            int d = (int)(trackBar5.Value / 10) + 1;
            Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(d, d),
                    new OpenCvSharp.Point(-1, -1));
            if (img2 == null && result != null)
            {
                change = result;
                img2 = change;
            }
            if (img2 != null)
            {
                Cv2.Dilate(change, img2, element);
                Bitmap bitmap1 = BitmapConverter.ToBitmap(img2);
                pictureBox1.Image = bitmap1;
                result = img2;
            }
        }

        #endregion

        #region 图像变换

        /// <summary>
        /// Sobel算子检测边缘
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            Mat cn = Cv2.ImRead(srcName, ImreadModes.Color);
            resultX = resultY = result;
            Cv2.Sobel(cn, resultX, -1, 1, 0);
            Cv2.Sobel(cn, resultY, -1, 0, 1);
            result = resultX + resultY;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// Canny算子检测边缘
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            Mat cn = Cv2.ImRead(srcName, ImreadModes.Color);
            Cv2.Canny(cn, result, 100, 200);
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 霍夫变换
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            Mat cn = Cv2.ImRead(srcName, ImreadModes.Color);
            Cv2.Canny(cn, result, 80, 180);
            Cv2.Threshold(result, result, 170, 255, ThresholdTypes.Binary);
            LineSegmentPoint[] lineSegmentPoint;
            lineSegmentPoint = Cv2.HoughLinesP(result, 1, Cv2.PI / 180, 150, 30, 10);
            for (int i = 0; i < lineSegmentPoint.Length; i++)
            {
                Cv2.Line(cn, lineSegmentPoint[i].P1, lineSegmentPoint[i].P2, Scalar.RandomColor(), 3);
            }
            result = cn;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 图像翻转-重映射
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            Mat imgY = img;
            Cv2.Flip(img, imgY, FlipMode.Y);
            result = imgY;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 仿射变换
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            double angle = 30;
            Point2f center = new Point2f(img.Rows / 2, img.Cols / 2);
            Mat fs = Cv2.GetRotationMatrix2D(center, angle, 1);
            Cv2.WarpAffine(result, result, fs, result.Size());
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 直方图均衡化
        /// </summary>
        private void button11_Click(object sender, EventArgs e)
        {
            Mat gray = img;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            Mat equalImg = img;
            Cv2.EqualizeHist(gray, equalImg);
            hist = img;
            int[] channels = { 0 };
            Rangef[] inRanges = new Rangef[] { new Rangef(0, 256) };
            int[] histSize = { 256 };
            Mat[] equals = new Mat[] { equalImg };
            Cv2.CalcHist(equals, channels, new Mat(), hist, 1, histSize, inRanges);
            Mat histImage = Mat.Zeros(400, 512, MatType.CV_8UC3);
            for (int i = 1; i <= hist.Rows; i++)
            {
                Point p1 = new Point(2 * (i - 1), 400 - 1);
                Point p2 = new Point(2 * i - 1, 400 - hist.At<float>(i - 1) / 15);
                Scalar scalar = new Scalar(255, 255, 255);
                Cv2.Rectangle(histImage, p1, p2, scalar, -1);
            }
            result = histImage;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        #endregion

        #region 图像修复与匹配

        /// <summary>
        /// 直方图
        /// </summary>
        private void button10_Click(object sender, EventArgs e)
        {
            Mat gray = img;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            hist = img;
            int[] channels = { 0 };
            Rangef[] inRanges = new Rangef[] { new Rangef(0, 256) };
            int[] histSize = { 256 };
            Mat[] grays = new Mat[] { gray };
            Cv2.CalcHist(grays, channels, new Mat(), hist, 1, histSize, inRanges);
            Mat histImage = Mat.Zeros(400, 512, MatType.CV_8UC3);
            for (int i = 1; i <= hist.Rows; i++)
            {
                Point p1 = new Point(2 * (i - 1), 400 - 1);
                Point p2 = new Point(2 * i - 1, 400 - hist.At<float>(i - 1) / 15);
                Scalar scalar = new Scalar(255, 255, 255);
                Cv2.Rectangle(histImage, p1, p2, scalar, -1);
            }
            result = histImage;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 绘制轮廓
        /// </summary>
        private void button12_Click(object sender, EventArgs e)
        {
            Mat gray = img, binary = gray;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(13, 13), 4, 4);//平滑滤波
            Cv2.Threshold(gray, binary, 170, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);//自适应二值化
            HierarchyIndex[] hierarchy;
            OpenCvSharp.Point[][] coutours;
            Cv2.FindContours(binary, out coutours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);
            for (int i = 0; i < coutours.Length; i++)
            {
                Cv2.DrawContours(img, coutours, i, Scalar.RandomColor(), 1);
            }
            result = img;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 凸包
        /// </summary>
        private void button13_Click(object sender, EventArgs e)
        {
            Mat gray = img, binary = gray;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, 105, 255, ThresholdTypes.Binary);
            HierarchyIndex[] hierarchy;
            OpenCvSharp.Point[][] coutours;
            Cv2.FindContours(binary, out coutours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);
            Point[][] hull = new Point[coutours.Length][];
            for (int i = 0; i < coutours.Length; i++)
            {
                hull[i] = Cv2.ConvexHull(coutours[i], false);
            }
            for (int i = 0; i < hull.Length; i++) //contours.Length
            {
                Scalar color = new Scalar(rng.Uniform(0, 256), rng.Uniform(0, 256), rng.Uniform(0, 256));
                Cv2.DrawContours(result, coutours, (int)i, color);
                Cv2.DrawContours(result, hull, (int)i, color);
            }
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 图像修复
        /// </summary>
        private void button14_Click(object sender, EventArgs e)
        {
            Mat gray = img;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            Mat mask = gray;
            Cv2.Threshold(gray, mask, 245, 255, ThresholdTypes.Binary);
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            Cv2.Dilate(mask, mask, kernel);
            //Cv2.Inpaint(img, mask, result, 5, InpaintMethod.NS);
            Bitmap bitmap1 = BitmapConverter.ToBitmap(mask);
            //Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 直方图对比
        /// </summary>
        private void button15_Click(object sender, EventArgs e)
        {
            Mat gray = img, gray2 = gray;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            Mat gray3 = Cv2.ImRead("D:\\lena.png", ImreadModes.Grayscale);
            hist = img;
            Mat hist3 = hist;
            int[] channels = { 0 };
            Rangef[] inRanges = new Rangef[] { new Rangef(0, 256) };
            int[] histSize = { 256 };
            Mat[] grays = new Mat[] { gray };
            Mat[] grays3 = new Mat[] { gray3 };
            Cv2.CalcHist(grays, channels, new Mat(), hist, 1, histSize, inRanges);
            Cv2.CalcHist(grays3, channels, new Mat(), hist3, 1, histSize, inRanges);
            Mat histImage = Mat.Zeros(400, 512, MatType.CV_8UC3);
            for (int i = 1; i <= hist.Rows; i++)
            {
                Point p1 = new Point(2 * (i - 1), 400 - 1);
                Point p2 = new Point(2 * i - 1, 400 - hist.At<float>(i - 1) / 15);
                Scalar scalar = new Scalar(255, 255, 255);
                Cv2.Rectangle(histImage, p1, p2, scalar, -1);
            }
            Mat histImage3 = Mat.Zeros(400, 512, MatType.CV_8UC3);
            for (int i = 1; i <= hist.Rows; i++)
            {
                Point p1 = new Point(2 * (i - 1), 400 - 1);
                Point p2 = new Point(2 * i - 1, 400 - hist.At<float>(i - 1) / 15);
                Scalar scalar = new Scalar(255, 255, 255);
                Cv2.Rectangle(histImage3, p1, p2, scalar, -1);
            }
            double xxx = Cv2.CompareHist(hist, hist3, HistCompMethods.Correl);
            MessageBox.Show(xxx.ToString());
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        #endregion

        #region 特征点检测与匹配

        /// <summary>
        /// 亚像素角点检测
        /// </summary>
        private void button16_Click(object sender, EventArgs e)
        {
            Mat gray = img, gray2 = gray;
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            OpenCvSharp.Size winSize = new OpenCvSharp.Size(21, 21);   // 搜索矩形大小的一半，类似于渲染的小方格
            OpenCvSharp.Size zeroZone = new OpenCvSharp.Size(-1, -1);    //这里是死区的一半尺寸，（-1，-1表示没有死区）
            List<Point2f> inputCorners = new List<Point2f>();  //这里是输入角的初始坐标和提供的细化坐标（类似扫描）
            for (int i = 50; i < img.Rows - 60; i += 40)   //给inputCorners 矩阵赋值
            {
                for (int j = 50; j < img.Cols - 50; j += 40)
                {
                    inputCorners.Add(new Point(j, i));
                }
            }
            TermCriteria criteria = new TermCriteria(CriteriaTypes.Eps, 100, 0.01);   //这个是检测数据
            //这里开始检测角点
            Point2f[] y_cornersPoint = Cv2.CornerSubPix(gray, inputCorners, winSize, zeroZone, criteria);
            //遍历画出角点
            foreach (var item in y_cornersPoint)
            {
                Cv2.Circle(gray, Convert.ToInt16(item.X), Convert.ToInt16(item.Y), 10, Scalar.Black, 2);
            }
            result = gray;
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        /// <summary>
        /// 特征点Surf匹配
        /// </summary>
        /// <param name="imgSrc">输入图1</param>
        /// <param name="imgSub">输入图2</param>
        /// <param name="threshold">Surf的阈值</param>
        /// <returns></returns>
        public static Bitmap MatchPicBySurf(Bitmap imgSrc, Bitmap imgSub, double threshold = 400)
        {
            Mat matSrc = BitmapConverter.ToMat(imgSrc);
            Mat matTo = BitmapConverter.ToMat(imgSub);
            Mat matSrcRet = new Mat();
            Mat matToRet = new Mat();

            KeyPoint[] keyPointsSrc, keyPointsTo;
            var Surf = OpenCvSharp.XFeatures2D.SURF.Create(threshold, 4, 3, true, true);

            Surf.DetectAndCompute(matSrc, null, out keyPointsSrc, matSrcRet);
            Surf.DetectAndCompute(matTo, null, out keyPointsTo, matToRet);

            using (var flnMatcher = new OpenCvSharp.FlannBasedMatcher())
            {
                var matches = flnMatcher.Match(matSrcRet, matToRet);
                //求最小最大距离
                double minDistance = 1000;//反向逼近
                double maxDistance = 0;
                for (int i = 0; i < matSrcRet.Rows; i++)
                {
                    double distance = matches[i].Distance;
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }

                var pointsSrc = new List<Point2f>();
                var pointsDst = new List<Point2f>();
                //筛选较好的匹配点
                var goodMatches = new List<DMatch>();
                for (int i = 0; i < matSrcRet.Rows; i++)
                {
                    double distance = matches[i].Distance;
                    if (distance < Math.Max(minDistance * 2, 0.02))
                    {
                        pointsSrc.Add(keyPointsSrc[matches[i].QueryIdx].Pt);
                        pointsDst.Add(keyPointsTo[matches[i].TrainIdx].Pt);
                        //距离小于范围的压入新的DMatch
                        goodMatches.Add(matches[i]);
                    }
                }
                var outMat = new Mat();
                var pSrc = new List<Point2d>();
                var pDst = new List<Point2d>();
                //单精度点转为双精度点
                foreach (Point2f point in pointsSrc) { pSrc.Add(new Point2d(point.X, point.Y)); }
                foreach (Point2f point in pointsDst) { pDst.Add(new Point2d(point.X, point.Y)); }
                var outMask = new Mat();
                // 如果原始的匹配结果为空, 则跳过过滤步骤
                if (pSrc.Count > 0 && pDst.Count > 0)
                    // 算法RANSAC对匹配的结果做过滤
                    Cv2.FindHomography(pSrc, pDst, HomographyMethods.Ransac, mask: outMask);
                // 如果通过RANSAC处理后的匹配点大于10个,才应用过滤. 否则使用原始的匹配点结果(匹配点过少的时候通过RANSAC处理后,可能会得到0个匹配点的结果).
                if (outMask.Rows > 10)
                {
                    byte[] maskBytes = new byte[outMask.Rows * outMask.Cols];
                    outMask.GetArray(out maskBytes);
                    Cv2.DrawMatches(matSrc, keyPointsSrc, matTo, keyPointsTo, goodMatches, outMat, matchesMask: maskBytes, flags: DrawMatchesFlags.NotDrawSinglePoints);
                }
                else
                    Cv2.DrawMatches(matSrc, keyPointsSrc, matTo, keyPointsTo, goodMatches, outMat, flags: DrawMatchesFlags.NotDrawSinglePoints);
                return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outMat);
            }

        }

        /// <summary>
        /// SURF匹配
        /// </summary>
        private void button17_Click(object sender, EventArgs e)
        {
            Bitmap bbb = BitmapConverter.ToBitmap(img);
            Bitmap eee = BitmapConverter.ToBitmap(img);
            Bitmap ccc = MatchPicBySurf(bbb, eee);
            result = BitmapConverter.ToMat(ccc);
            Bitmap bitmap1 = BitmapConverter.ToBitmap(result);
            pictureBox1.Image = bitmap1;
        }

        #endregion

        /// <summary>
        /// 跳转
        /// </summary>
        private void 视频ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }


        #region 菜单显示与隐藏

        /// <summary>
        /// 窗体加载
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            PBheight = pictureBox1.Height;
            PBwidth = pictureBox1.Width;
            label1.Visible = false;
            button2.Visible = false;
            this.pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            label2.Visible = false;//均值滤波标签
            trackBar2.Visible = false;//均值滤波
            trackBar1.Visible = false;
            trackBar3.Visible = false;//中值滤波
            label3.Visible = false;//中值滤波标签
            trackBar4.Visible = false;//腐蚀
            label4.Visible = false;//腐蚀标签
            trackBar5.Visible = false;//膨胀
            label5.Visible = false;//膨胀标签
            label6.Visible = false;//图片缩放标签
            trackBar6.Visible = false;//图片缩放
            button3.Visible = false;//二值化
            button4.Visible = false;//灰度图
            button5.Visible = false;//Sobel边缘
            button6.Visible = false;//Canny边缘
            button7.Visible = false;//霍夫变换检测直线
            button8.Visible = false;//重映射
            button9.Visible = false;//仿射变换
            button10.Visible = false;//直方图
            button11.Visible = false;//直方图均衡化
            button12.Visible = false;//绘制轮廓
            button13.Visible = false;//凸包
            button14.Visible = false;//图像修复
            button15.Visible = false;//直方图对比
            button16.Visible = false;//角点检测
            button17.Visible = false;//SURF
        }

        /// <summary>
        /// 基本图像处理
        /// </summary>
        private void 基本图像处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackBar1.Visible = true;//亮度
            label1.Visible = true;//亮度标签
            button2.Visible = true;//绘制轨迹
            label2.Visible = false;//均值滤波标签
            trackBar2.Visible = false;//均值滤波
            trackBar3.Visible = false;//双边滤波
            label3.Visible = false;//双边滤波标签
            trackBar4.Visible = false;//腐蚀
            label4.Visible = false;//腐蚀标签
            trackBar5.Visible = false;//膨胀
            label5.Visible = false;//膨胀标签
            trackBar6.Visible = true;//图片缩放
            label6.Visible = true;//图片缩放标签
            button3.Visible = true;//二值化
            button4.Visible = true;//灰度图
            button5.Visible = false;//Sobel边缘
            button6.Visible = false;//Canny边缘
            button7.Visible = false;//霍夫变换检测直线
            button8.Visible = false;//重映射
            button9.Visible = false;//仿射变换
            button10.Visible = false;//直方图
            button11.Visible = false;//直方图均衡化
            button12.Visible = false;//绘制轮廓
            button13.Visible = false;//凸包
            button14.Visible = false;//图像修复
            button15.Visible = false;//直方图对比
            button16.Visible = false;//角点检测
            button17.Visible = false;//SURF
            //亮度设置
            trackBar1.SetRange(50, 150);
            trackBar1.TickFrequency = 10;
            trackBar1.Value = 100;
            //缩放设置
            trackBar6.SetRange(10, 200);
            trackBar6.TickFrequency = 10;
            trackBar6.Value = 45;
        }

        /// <summary>
        /// 图像滤波
        /// </summary>
        private void 图像滤波ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Visible = true;//均值滤波标签
            trackBar2.Visible = true;//均值滤波
            label1.Visible = false;//亮度标签
            trackBar1.Visible = false;//亮度
            button2.Visible = false;//绘制轨迹
            label3.Visible = true;//中值滤波标签
            trackBar3.Visible = true;//中值滤波
            trackBar4.Visible = true;//腐蚀
            label4.Visible = true;//腐蚀标签
            trackBar5.Visible = true;//膨胀
            label5.Visible = true;//膨胀标签
            trackBar6.Visible = false;//图片缩放
            label6.Visible = false;//图片缩放标签
            button3.Visible = false;//二值化
            button4.Visible = false;//灰度图
            button5.Visible = false;//Sobel边缘
            button6.Visible = false;//Canny边缘
            button7.Visible = false;//霍夫变换检测直线
            button8.Visible = false;//重映射
            button9.Visible = false;//仿射变换
            button10.Visible = false;//直方图
            button11.Visible = false;//直方图均衡化
            button12.Visible = false;//绘制轮廓
            button13.Visible = false;//凸包
            button14.Visible = false;//图像修复
            button15.Visible = false;//直方图对比
            button16.Visible = false;//角点检测
            button17.Visible = false;//SURF
            //均值滤波设置
            trackBar2.SetRange(0, 100);
            trackBar2.TickFrequency = 10;
            trackBar2.Value = 0;
            //中值滤波设置
            trackBar3.SetRange(0, 100);
            trackBar3.TickFrequency = 10;
            trackBar3.Value = 0;
            //腐蚀设置
            trackBar4.SetRange(0, 100);
            trackBar4.TickFrequency = 10;
            trackBar4.Value = 0;
            //膨胀设置
            trackBar5.SetRange(0, 100);
            trackBar5.TickFrequency = 10;
            trackBar5.Value = 0;
        }

        /// <summary>
        /// 图像变换
        /// </summary>
        private void 图像变换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackBar1.Visible = false;//亮度
            label1.Visible = false;//亮度标签
            button2.Visible = false;//绘制轨迹
            label2.Visible = false;//均值滤波标签
            trackBar2.Visible = false;//均值滤波
            trackBar3.Visible = false;//双边滤波
            label3.Visible = false;//双边滤波标签
            trackBar4.Visible = false;//腐蚀
            label4.Visible = false;//腐蚀标签
            trackBar5.Visible = false;//膨胀
            label5.Visible = false;//膨胀标签
            trackBar6.Visible = false;//图片缩放
            label6.Visible = false;//图片缩放标签
            button3.Visible = false;//二值化
            button4.Visible = false;//灰度图
            button5.Visible = true;//Sobel边缘
            button6.Visible = true;//Canny边缘
            button7.Visible = true;//霍夫变换检测直线
            button8.Visible = true;//重映射
            button9.Visible = true;//仿射变换
            button10.Visible = false;//直方图
            button11.Visible = true;//直方图均衡化
            button12.Visible = false;//绘制轮廓
            button13.Visible = false;//凸包
            button14.Visible = false;//图像修复
            button15.Visible = false;//直方图对比
            button16.Visible = false;//角点检测
            button17.Visible = false;//SURF
        }

        /// <summary>
        /// 图像修复与匹配
        /// </summary>
        private void 图像修复与匹配ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackBar1.Visible = false;//亮度
            label1.Visible = false;//亮度标签
            button2.Visible = false;//绘制轨迹
            label2.Visible = false;//均值滤波标签
            trackBar2.Visible = false;//均值滤波
            trackBar3.Visible = false;//双边滤波
            label3.Visible = false;//双边滤波标签
            trackBar4.Visible = false;//腐蚀
            label4.Visible = false;//腐蚀标签
            trackBar5.Visible = false;//膨胀
            label5.Visible = false;//膨胀标签
            trackBar6.Visible = false;//图片缩放
            label6.Visible = false;//图片缩放标签
            button3.Visible = false;//二值化
            button4.Visible = false;//灰度图
            button5.Visible = false;//Sobel边缘
            button6.Visible = false;//Canny边缘
            button7.Visible = false;//霍夫变换检测直线
            button8.Visible = false;//重映射
            button9.Visible = false;//仿射变换
            button11.Visible = false;//直方图均衡化
            button10.Visible = true;//直方图
            button12.Visible = true;//绘制轮廓
            button13.Visible = true;//凸包
            button14.Visible = true;//图像修复
            button15.Visible = true;//直方图对比
            button16.Visible = false;//角点检测
            button17.Visible = false;//SURF
        }

        /// <summary>
        /// 特征点检测与匹配
        /// </summary>
        private void 特征点检测与匹配ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackBar1.Visible = false;//亮度
            label1.Visible = false;//亮度标签
            button2.Visible = false;//绘制轨迹
            label2.Visible = false;//均值滤波标签
            trackBar2.Visible = false;//均值滤波
            trackBar3.Visible = false;//双边滤波
            label3.Visible = false;//双边滤波标签
            trackBar4.Visible = false;//腐蚀
            label4.Visible = false;//腐蚀标签
            trackBar5.Visible = false;//膨胀
            label5.Visible = false;//膨胀标签
            trackBar6.Visible = false;//图片缩放
            label6.Visible = false;//图片缩放标签
            button3.Visible = false;//二值化
            button4.Visible = false;//灰度图
            button5.Visible = false;//Sobel边缘
            button6.Visible = false;//Canny边缘
            button7.Visible = false;//霍夫变换检测直线
            button8.Visible = false;//重映射
            button9.Visible = false;//仿射变换
            button11.Visible = false;//直方图均衡化
            button10.Visible = false;//直方图
            button12.Visible = false;//绘制轮廓
            button13.Visible = false;//凸包
            button14.Visible = false;//图像修复
            button15.Visible = false;//直方图对比
            button16.Visible = true;//角点检测
            button17.Visible = true;//SURF
        }

        #endregion

        /// <summary>
        /// 重置为原图
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(srcName);
            img = new Mat(srcName);
            result = img2 = img3 = change = img;
            if (trackBar1.Visible) trackBar1.Value = 100;//亮度回调
            if (button2.Visible)
            {
                beginPaint = false;
                button2.Text = "开始绘制轨迹";
            }
            if (trackBar2.Visible) trackBar2.Value = 0;//均值滤波回调
            if (trackBar3.Visible) trackBar3.Value = 0;//中值滤波回调
            if (trackBar4.Visible) trackBar4.Value = 0;//腐蚀回调
            if (trackBar5.Visible) trackBar5.Value = 0;//膨胀回调
            if (trackBar6.Visible) trackBar5.Value = 0;//图片缩放回调
        }
    }
}
