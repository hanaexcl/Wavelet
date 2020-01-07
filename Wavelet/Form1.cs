using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Wavelet
{
    struct RGB
    {
        public int R;
        public int G;
        public int B;
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            if (filePath.Length == 0) return;


            int _haarTimes = Int32.Parse(toolStripComboBox1.Text);

            Bitmap tmpBitmap = new Bitmap(filePath);
            Bitmap ntmpBitmap = new Bitmap(tmpBitmap.Width, tmpBitmap.Height);
            Bitmap ttmpBitmap = new Bitmap(tmpBitmap.Width, tmpBitmap.Height);

            pictureBox1.Image = tmpBitmap;

            int tmpSize_W = (int)Math.Pow(2, Math.Ceiling(Math.Log(tmpBitmap.Width, 2)));
            int tmpSize_H = (int)Math.Pow(2, Math.Ceiling(Math.Log(tmpBitmap.Height, 2)));
            RGB[,] img = new RGB[tmpSize_W, tmpSize_H];


            int r, g, b;
            for (int h = 0; h < tmpSize_H; h++)
            {
                for (int w = 0; w < tmpSize_W; w++)
                {
                    r = 0; g = 0; b = 0;
                    if (w < tmpBitmap.Width && h < tmpBitmap.Height)
                    {
                        r = tmpBitmap.GetPixel(w, h).R;
                        g = tmpBitmap.GetPixel(w, h).G;
                        b = tmpBitmap.GetPixel(w, h).B;
                    }

                    RGB tmpRGB = new RGB();
                    tmpRGB.R = r;
                    tmpRGB.G = g;
                    tmpRGB.B = b;

                    img[w, h] = tmpRGB;
                }
            }

            //計算
            int harrtmpSize_H = tmpSize_H;
            int harrtmpSize_W = tmpSize_W;
            for (int harrTimes = 1; harrTimes <= _haarTimes; harrTimes++)
            {
                //橫
                for (int h = 0; h < harrtmpSize_H; h++)
                {
                    int runTimes = 0;
                    for (int t = tmpSize_W; t != 1; t = t / 2)
                    {
                        runTimes++;
                        if (runTimes < harrTimes) continue;

                        List<RGB> tmp = new List<RGB>();
                        for (int i = 0; i < t; i = i + 2)
                        {
                            img[i / 2, h].R = (img[i, h].R + img[i + 1, h].R) / 2;
                            img[i / 2, h].G = (img[i, h].G + img[i + 1, h].G) / 2;
                            img[i / 2, h].B = (img[i, h].B + img[i + 1, h].B) / 2;

                            RGB tmpRGB = new RGB();
                            tmpRGB.R = img[i / 2, h].R - img[i + 1, h].R;
                            tmpRGB.G = img[i / 2, h].G - img[i + 1, h].G;
                            tmpRGB.B = img[i / 2, h].B - img[i + 1, h].B;
                            tmp.Add(tmpRGB);
                        }
                        tmp.Reverse();

                        for (int i = 0; i < tmp.Count; i++)
                        {
                            img[t - i - 1, h] = tmp[i];
                            //img[t - i - 1, h].R = ((img[t - i - 1, h].R * 8) + 128);
                            //img[t - i - 1, h].G = ((img[t - i - 1, h].G * 8) + 128);
                            //img[t - i - 1, h].B = ((img[t - i - 1, h].B * 8) + 128);
                        }

                        break;
                    }
                }

                //列
                for (int w = 0; w < harrtmpSize_W; w++)
                {
                    int runTimes = 0;
                    for (int t = tmpSize_H; t != 1; t = t / 2)
                    {
                        runTimes++;
                        if (runTimes < harrTimes) continue;

                        List<RGB> tmp = new List<RGB>();
                        for (int i = 0; i < t; i = i + 2)
                        {
                            img[w, i / 2].R = (img[w, i].R + img[w, i + 1].R) / 2;
                            img[w, i / 2].G = (img[w, i].G + img[w, i + 1].G) / 2;
                            img[w, i / 2].B = (img[w, i].B + img[w, i + 1].B) / 2;

                            RGB tmpRGB = new RGB();
                            tmpRGB.R = img[w, i / 2].R - img[w, i + 1].R;
                            tmpRGB.G = img[w, i / 2].G - img[w, i + 1].G;
                            tmpRGB.B = img[w, i / 2].B - img[w, i + 1].B;

                            tmp.Add(tmpRGB);
                        }
                        tmp.Reverse();

                        for (int i = 0; i < tmp.Count; i++)
                        {
                            img[w, t - i - 1] = tmp[i];
                            //img[w, t - i - 1].R = ((img[w, t - i - 1].R * 8) + 128);
                            //img[w, t - i - 1].G = ((img[w, t - i - 1].G * 8) + 128);
                            //img[w, t - i - 1].B = ((img[w, t - i - 1].B * 8) + 128);
                        }

                        break;
                    }
                }

                harrtmpSize_H = harrtmpSize_H / 2;
                harrtmpSize_W = harrtmpSize_W / 2;
            }

            //去除最小值
            for (int i = 0; i < 1; i++)
            {
                int minR, minG, minB;
                minR = img[0, 0].R;
                minG = img[0, 0].G;
                minB = img[0, 0].B;

                for (int h = 0; h < ntmpBitmap.Height; h++)
                {
                    for (int w = 0; w < ntmpBitmap.Width; w++)
                    {
                        if (Math.Abs(minR) > Math.Abs(img[w, h].R) && img[w, h].R > 0) minR = img[w, h].R;
                        if (Math.Abs(minG) > Math.Abs(img[w, h].G) && img[w, h].G > 0) minG = img[w, h].G;
                        if (Math.Abs(minB) > Math.Abs(img[w, h].B) && img[w, h].B > 0) minB = img[w, h].B;
                    }
                }

                for (int h = 0; h < ntmpBitmap.Height; h++)
                {
                    for (int w = 0; w < ntmpBitmap.Width; w++)
                    {
                        if (img[w, h].R == minR) img[w, h].R = 0;
                        if (img[w, h].G == minG) img[w, h].G = 0;
                        if (img[w, h].B == minB) img[w, h].B = 0;
                    }
                }
            }

            //顯示
            for (int h = 0; h < ntmpBitmap.Height; h++)
            {
                for (int w = 0; w < ntmpBitmap.Width; w++)
                {
                    r = img[w, h].R; g = img[w, h].G; b = img[w, h].B;

                    //if (r < 0) r = r + 256;
                    //if (g < 0) g = g + 256;
                    //if (b < 0) b = b + 256;

                    //if (r < 0) r = Math.Abs(r);
                    //if (g < 0) g = Math.Abs(g);
                    //if (b < 0) b = Math.Abs(b);
                    //int avg = (r + g + b) / 3;

                    //r = r * 8 + 128;
                    //g = g * 8 + 128;
                    //b = b * 8 + 128;

                    if (h >= harrtmpSize_H || w >= harrtmpSize_W)
                    {
                        r = ((r * 8) + 128);
                        g = ((g * 8) + 128);
                        b = ((b * 8) + 128);
                    }

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;
                    if (g < 0) g = 0;
                    if (g > 255) g = 255;
                    if (b < 0) b = 0;
                    if (b > 255) b = 255;
                    //ntmpBitmap.SetPixel(w, h, Color.FromArgb(avg, avg, avg));

                    ntmpBitmap.SetPixel(w, h, Color.FromArgb(r, g, b));
                }
            }

            pictureBox2.Image = ntmpBitmap;


            //回推
            for (int harrTimes = _haarTimes; harrTimes >= 1; harrTimes--)
            {
                //列
                for (int w = 0; w < tmpSize_W; w++)
                {
                    int middle = (int)tmpSize_H / (int)Math.Pow(2, harrTimes);

                    RGB[] tmpList = new RGB[tmpSize_H];

                    for (int h = 0; h < middle; h++)
                    {
                        tmpList[h * 2].R = img[w, h].R + img[w, middle + h].R;
                        tmpList[h * 2].G = img[w, h].G + img[w, middle + h].G;
                        tmpList[h * 2].B = img[w, h].B + img[w, middle + h].B;

                        tmpList[h * 2 + 1].R = img[w, h].R - img[w, middle + h].R;
                        tmpList[h * 2 + 1].G = img[w, h].G - img[w, middle + h].G;
                        tmpList[h * 2 + 1].B = img[w, h].B - img[w, middle + h].B;
                    }

                    for (int h = 0; h < tmpList.Length; h++)
                    {
                        img[w, h] = tmpList[h];
                    }
                }

                //橫
                for (int h = 0; h < tmpSize_H; h++)
                {
                    int middle = (int)tmpSize_W / (int)Math.Pow(2, harrTimes);

                    RGB[] tmpList = new RGB[tmpSize_W];

                    for (int w = 0; w < middle; w++)
                    {
                        tmpList[w * 2].R = img[w, h].R + img[middle + w, h].R;
                        tmpList[w * 2].G = img[w, h].G + img[middle + w, h].G;
                        tmpList[w * 2].B = img[w, h].B + img[middle + w, h].B;

                        tmpList[w * 2 + 1].R = img[w, h].R - img[middle + w, h].R;
                        tmpList[w * 2 + 1].G = img[w, h].G - img[middle + w, h].G;
                        tmpList[w * 2 + 1].B = img[w, h].B - img[middle + w, h].B;
                    }

                    for (int w = 0; w < tmpList.Length; w++)
                    {
                        img[w, h] = tmpList[w];
                    }
                }


                //harrtmpSize_H = harrtmpSize_H * 2;
                //harrtmpSize_W = harrtmpSize_W * 2;
            }

            //顯示
            for (int h = 0; h < ttmpBitmap.Height; h++)
            {
                for (int w = 0; w < ttmpBitmap.Width; w++)
                {
                    r = img[w, h].R; g = img[w, h].G; b = img[w, h].B;

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;
                    if (g < 0) g = 0;
                    if (g > 255) g = 255;
                    if (b < 0) b = 0;
                    if (b > 255) b = 255;

                    ttmpBitmap.SetPixel(w, h, Color.FromArgb(r, g, b));
                }
            }

            pictureBox3.Image = ttmpBitmap;
        }
    }
}
