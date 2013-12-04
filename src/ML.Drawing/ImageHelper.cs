using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ML.Drawing
{
    public class ImageHelper
    {
        /// <summary>   
        /// 生成缩略图   
        /// </summary>   
        /// <param name="originalImagePath">源图路径（物理路径）</param>   
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>   
        /// <param name="width">缩略图宽度</param>   
        /// <param name="height">缩略图高度</param>   
        /// <param name="mode">生成缩略图的方式</param>   
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(originalImagePath));
            Image originalImage = Image.FromStream(stream);
            stream.Dispose();

            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case "HW":    //指定高宽缩放（可能变形）   
                    break;
                case "W":    //指定宽，高按比例   
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H":    //指定高，宽按比例   
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut":    //指定高宽裁减（不变形）   
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //制作缩略图
            Bitmap bitmap = new Bitmap(towidth, toheight);
            bitmap.SetResolution((float)originalImage.HorizontalResolution, (float)originalImage.VerticalResolution);

            System.Drawing.Image TargetImage = System.Drawing.Image.FromHbitmap(bitmap.GetHbitmap());
            Graphics aGh = Graphics.FromImage(TargetImage);
            //设置高质量插值法
            aGh.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            aGh.SmoothingMode = SmoothingMode.HighQuality;
            //aGh.Clear(Color.Transparent);
            aGh.Clear(Color.White);

            aGh.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
            new Rectangle(x, y, ow, oh),
            GraphicsUnit.Pixel);

            //aGh.DrawImage(originalImage , 0, 0, towidth, toheight);
            aGh.Dispose();

            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters myEncoderParameters = new EncoderParameters(2);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            System.Drawing.Imaging.Encoder myEncoder2 = System.Drawing.Imaging.Encoder.ColorDepth;
            System.Drawing.Imaging.Encoder myEncoder3 = System.Drawing.Imaging.Encoder.ChrominanceTable;
            EncoderParameter myEncoderParameter1 = new EncoderParameter(myEncoder, 100L);
            EncoderParameter myEncoderParameter2 = new EncoderParameter(myEncoder2, 16L);
            myEncoderParameters.Param[0] = myEncoderParameter1;
            myEncoderParameters.Param[1] = myEncoderParameter2;

            try
            {
                TargetImage.Save(thumbnailPath, myImageCodecInfo, myEncoderParameters);
            }
            catch (System.Exception e)
            {

                throw e;

            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                TargetImage.Dispose();
            }

        }

        public static bool chkFileExt(string fileExt)
        {
            bool canUpload = false;
            string[] allowFileExt = { ".gif", ".jpg", ".png" };
            foreach (string s in allowFileExt)
            {
                if (s == fileExt) canUpload = true;
            }
            return canUpload;
        }

        /// <summary>
        /// 获得图像高宽信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ImageInformation GetImageInfo(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                ImageInformation imageInfo = new ImageInformation();
                imageInfo.Width = image.Width;
                imageInfo.Height = image.Height;
                return imageInfo;
            }
        }
        public bool ThumbnailCallback()
        {
            return false;
        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }


        /**/
        /// <summary>
        /// 获取图片缩略图
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pWidth">缩略图宽度</param>
        /// <param name="pHeight">缩略图高度</param>
        /// <param name="pFormat">保存格式，通常可以是jpeg</param>
        public void GetSmaller(string pPath, string pSavedPath, int pWidth, int pHeight)
        {
            string fileSaveUrl = pSavedPath;//+ "\\smaller.jpg";

            using (FileStream fs = new FileStream(pPath, FileMode.Open))
            {

                MakeSmallImg(fs, fileSaveUrl, pWidth, pHeight);
            }

        }


        //按模版比例生成缩略图（以流的方式获取源文件）  
        //生成缩略图函数  
        //顺序参数：源图文件流、缩略图存放地址、模版宽、模版高  
        //注：缩略图大小控制在模版区域内  
        public static void MakeSmallImg(System.IO.Stream fromFileStream, string fileSaveUrl, System.Double templateWidth, System.Double templateHeight)
        {
            //从文件取得图片对象，并使用流中嵌入的颜色管理信息  
            System.Drawing.Image myImage = System.Drawing.Image.FromStream(fromFileStream, true);

            //缩略图宽、高  
            System.Double newWidth = myImage.Width, newHeight = myImage.Height;
            //宽大于模版的横图  
            if (myImage.Width > myImage.Height || myImage.Width == myImage.Height)
            {
                if (myImage.Width > templateWidth)
                {
                    //宽按模版，高按比例缩放  
                    newWidth = templateWidth;
                    newHeight = myImage.Height * (newWidth / myImage.Width);
                }
            }
            //高大于模版的竖图  
            else
            {
                if (myImage.Height > templateHeight)
                {
                    //高按模版，宽按比例缩放  
                    newHeight = templateHeight;
                    newWidth = myImage.Width * (newHeight / myImage.Height);
                }
            }

            //取得图片大小  
            System.Drawing.Size mySize = new Size((int)newWidth, (int)newHeight);
            //新建一个bmp图片  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(myImage, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
            System.Drawing.GraphicsUnit.Pixel);

            ///文字水印  
            /*System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(bitmap);
            System.Drawing.Font f = new Font("Lucida Grande", 6);
            System.Drawing.Brush b = new SolidBrush(Color.Gray);
            G.DrawString("Ftodo.com", f, b, 0, 0);
            G.Dispose();*/

            ///图片水印  
            //System.Drawing.Image   copyImage   =   System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("pic/1.gif"));  
            //Graphics   a   =   Graphics.FromImage(bitmap);  
            //a.DrawImage(copyImage,   new   Rectangle(bitmap.Width-copyImage.Width,bitmap.Height-copyImage.Height,copyImage.Width,   copyImage.Height),0,0,   copyImage.Width,   copyImage.Height,   GraphicsUnit.Pixel);  

            //copyImage.Dispose();  
            //a.Dispose();  
            //copyImage.Dispose();  

            //保存缩略图  
            if (File.Exists(fileSaveUrl))
            {
                File.SetAttributes(fileSaveUrl, FileAttributes.Normal);
                File.Delete(fileSaveUrl);
            }

            //bitmap.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);

            if (fileSaveUrl.IndexOf(".gif") != -1)
            {
                bitmap.Save(fileSaveUrl, ImageFormat.Gif);
            }
            else
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;

                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 99L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bitmap.Save(fileSaveUrl, myImageCodecInfo, myEncoderParameters);
            }


            g.Dispose();
            myImage.Dispose();
            bitmap.Dispose();
        }

        /**/
        /// <summary>
        /// 获取图片缩略图
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pWidth">缩略图宽度</param>
        /// <param name="pHeight">缩略图高度</param>
        /// <param name="pFormat">保存格式，通常可以是jpeg</param>
        public void GetMiddle(string pPath, string pSavedPath, int pWidth, int pHeight)
        {
            string fileSaveUrl = pSavedPath;//+ "\\smaller.jpg";

            using (FileStream fs = new FileStream(pPath, FileMode.Open))
            {

                MakeMiddleImg(fs, fileSaveUrl, pWidth, pHeight);
            }

        }

        public static void CreateMiddle(System.Drawing.Image aImage, string fileExt, string ThumbUrl, int ThumbWidth, int ThumbHeight)
        {
            int oWidth = aImage.Width;
            int oHeight = aImage.Height;

            if (oWidth > oHeight)
                ThumbHeight = ThumbHeight * oHeight / oWidth;
            else
                ThumbWidth = ThumbWidth * oWidth / oHeight;

            //制作缩略图
            Bitmap myBitmap2 = new Bitmap(ThumbWidth, ThumbHeight);
            myBitmap2.SetResolution((float)aImage.HorizontalResolution, (float)aImage.VerticalResolution);

            System.Drawing.Image TargetImage = System.Drawing.Image.FromHbitmap(myBitmap2.GetHbitmap());
            Graphics aGh = Graphics.FromImage(TargetImage);
            aGh.Clear(Color.White);
            //设置高质量插值法
            aGh.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            aGh.SmoothingMode = SmoothingMode.HighQuality;
            aGh.DrawImage(aImage, 0, 0, ThumbWidth, ThumbHeight);
            aGh.Dispose();

            if (fileExt == "gif")
            {
                TargetImage.Save(ThumbUrl, ImageFormat.Gif);
                myBitmap2.Dispose();
            }
            else
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                EncoderParameters myEncoderParameters = new EncoderParameters(2);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                System.Drawing.Imaging.Encoder myEncoder2 = System.Drawing.Imaging.Encoder.ColorDepth;
                System.Drawing.Imaging.Encoder myEncoder3 = System.Drawing.Imaging.Encoder.ChrominanceTable;
                EncoderParameter myEncoderParameter1 = new EncoderParameter(myEncoder, 100L);
                EncoderParameter myEncoderParameter2 = new EncoderParameter(myEncoder2, 16L);
                myEncoderParameters.Param[0] = myEncoderParameter1;
                myEncoderParameters.Param[1] = myEncoderParameter2;
                TargetImage.Save(ThumbUrl, myImageCodecInfo, myEncoderParameters);
                myBitmap2.Dispose();
            }
        }

        //按固定宽度生成缩略图（以流的方式获取源文件）  
        //生成缩略图函数  120 宽 高度在90-150之间
        //顺序参数：源图文件流、缩略图存放地址、模版宽、模版高  
        //注：缩略图大小控制在模版区域内  
        public static void MakeMiddleImg(System.IO.Stream fromFileStream, string fileSaveUrl, System.Double templateWidth, System.Double templateHeight)
        {
            //从文件取得图片对象，并使用流中嵌入的颜色管理信息  
            System.Drawing.Image myImage = System.Drawing.Image.FromStream(fromFileStream, true);

            //缩略图宽、高  
            System.Double newWidth = myImage.Width, newHeight = myImage.Height;


            /*if(myImage.Width > 120){
                newWidth = 120;
                newHeight = myImage.Height * (newWidth / myImage.Width);

            }	else{

                newWidth = myImage.Width;
                newHeight = myImage.Height; 
            }*/
            //宽大于模版的横图  
            if (myImage.Width > myImage.Height || myImage.Width == myImage.Height)
            {
                if (myImage.Width > templateWidth)
                {
                    //宽按模版，高按比例缩放  
                    newWidth = templateWidth;
                    newHeight = myImage.Height * (newWidth / myImage.Width);
                }
            }
            //高大于模版的竖图  
            else
            {
                if (myImage.Height > templateHeight)
                {
                    //高按模版，宽按比例缩放  
                    newWidth = templateWidth;
                    newHeight = myImage.Height * (newWidth / myImage.Width);
                    //if(newHeight>180) newHeight = 180;
                    //newHeight = templateHeight;
                    //newHeight = templateHeight;
                    //newWidth = myImage.Width * (newHeight / myImage.Height);
                }
            }

            //取得图片大小  
            System.Drawing.Size mySize = new Size((int)newWidth, (int)newHeight);
            //新建一个bmp图片  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(myImage, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
            System.Drawing.GraphicsUnit.Pixel);

            //保存缩略图  
            if (File.Exists(fileSaveUrl))
            {
                File.SetAttributes(fileSaveUrl, FileAttributes.Normal);
                File.Delete(fileSaveUrl);
            }

            //bitmap.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);

            if (fileSaveUrl.IndexOf(".gif") != -1)
            {
                bitmap.Save(fileSaveUrl, ImageFormat.Gif);
            }
            else
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;

                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 99L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bitmap.Save(fileSaveUrl, myImageCodecInfo, myEncoderParameters);
            }

            g.Dispose();
            myImage.Dispose();
            bitmap.Dispose();
        }


        /**/
        /// <summary>
        /// 获取图片指定部分
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pPartStartPointX">目标图片开始绘制处的坐标X值(通常为)</param>
        /// <param name="pPartStartPointY">目标图片开始绘制处的坐标Y值(通常为)</param>
        /// <param name="pPartWidth">目标图片的宽度</param>
        /// <param name="pPartHeight">目标图片的高度</param>
        /// <param name="pOrigStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="pOrigStartPointY">原始图片开始截取处的坐标Y值</param>
        /// <param name="pFormat">保存格式，通常可以是jpeg</param>
        public void GetPart(string pPath, string pSavedPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
        {
            string normalJpgPath = pSavedPath;//+ "\\normal.jpg";

            using (Image originalImg = Image.FromFile(pPath))
            {
                Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);
                Graphics graphics = Graphics.FromImage(partImg);
                Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new Size(pPartWidth, pPartHeight));//目标位置
                Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

                ///文字水印  
                System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(partImg);
                //System.Drawing.Font f = new Font("Lucida Grande", 6);
                //System.Drawing.Brush b = new SolidBrush(Color.Gray);
                G.Clear(Color.White);
                graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
                //G.DrawString("Ftodo.com", f, b, 0, 0);
                G.Dispose();

                originalImg.Dispose();
                if (File.Exists(normalJpgPath))
                {
                    File.SetAttributes(normalJpgPath, FileAttributes.Normal);
                    File.Delete(normalJpgPath);
                }

                //修改保存方式，提高图片画质
                //partImg.Save(normalJpgPath, ImageFormat.Jpeg);
                if (pSavedPath.IndexOf(".gif") != -1)
                {
                    partImg.Save(normalJpgPath, ImageFormat.Gif);
                }
                else
                {
                    ImageCodecInfo myImageCodecInfo;
                    System.Drawing.Imaging.Encoder myEncoder;
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;

                    myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameter = new EncoderParameter(myEncoder, 99L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    partImg.Save(normalJpgPath, myImageCodecInfo, myEncoderParameters);
                }

            }
        }


        /**/
        /// <summary>
        /// 获取按比例缩放的图片指定部分
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pPartStartPointX">目标图片开始绘制处的坐标X值(通常为)</param>
        /// <param name="pPartStartPointY">目标图片开始绘制处的坐标Y值(通常为)</param>
        /// <param name="pPartWidth">目标图片的宽度</param>
        /// <param name="pPartHeight">目标图片的高度</param>
        /// <param name="pOrigStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="pOrigStartPointY">原始图片开始截取处的坐标Y值</param>
        /// <param name="imageWidth">缩放后的宽度</param>
        /// <param name="imageHeight">缩放后的高度</param>
        public void GetPart(string pPath, string pSavedPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY, int imageWidth, int imageHeight)
        {
            string normalJpgPath = pSavedPath;//+ "\\normal.jpg";
            using (Image originalImg = Image.FromFile(pPath))
            {
                originalImg.Save(pSavedPath + ".gif", ImageFormat.Gif);
                //宽度小于48的图片直接复制原图
                if (originalImg.Width <= 48)
                {
                    originalImg.Save(pSavedPath, ImageFormat.Jpeg);
                    return;
                }
                if (originalImg.Width == imageWidth && originalImg.Height == imageHeight)
                {
                    GetPart(pPath, pSavedPath, pPartStartPointX, pPartStartPointY, pPartWidth, pPartHeight, pOrigStartPointX, pOrigStartPointY);
                    return;
                }

                Image.GetThumbnailImageAbort callback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                Image zoomImg = originalImg.GetThumbnailImage(imageWidth, imageHeight, callback, IntPtr.Zero);//缩放

                zoomImg.Save(pSavedPath + ".zoomImg.gif", ImageFormat.Gif);
                Bitmap partImg = new Bitmap(imageWidth, imageHeight);

                Graphics graphics = Graphics.FromImage(partImg);
                Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new Size(pPartWidth, pPartHeight));//目标位置
                Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

                ///文字水印  
                System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(partImg);
                //System.Drawing.Font f = new Font("Lucida Grande", 6);
                //System.Drawing.Brush b = new SolidBrush(Color.Gray);
                G.Clear(Color.White);

                graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
                //G.DrawString("Ftodo.com", f, b, 0, 0);
                G.Dispose();

                originalImg.Dispose();
                if (File.Exists(normalJpgPath))
                {
                    File.SetAttributes(normalJpgPath, FileAttributes.Normal);
                    File.Delete(normalJpgPath);
                }

                //partImg.Save(normalJpgPath, ImageFormat.Jpeg);
                if (pSavedPath.IndexOf(".gif") != -1)
                {
                    partImg.Save(normalJpgPath, ImageFormat.Gif);
                }
                else
                {
                    ImageCodecInfo myImageCodecInfo;
                    System.Drawing.Imaging.Encoder myEncoder;
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;

                    myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameter = new EncoderParameter(myEncoder, 99L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    partImg.Save(normalJpgPath, myImageCodecInfo, myEncoderParameters);
                }
            }
        }


        /// <summary>
        /// 截图某图片的指定区域，并以设定的大小另存为新图
        /// </summary>
        /// <param name="pPath"></param>
        /// <param name="pSavedPath"></param>
        /// <param name="pPartLocX"></param>
        /// <param name="pPartLocY"></param>
        /// <param name="pPartWidth"></param>
        /// <param name="pPartHeight"></param>
        /// <param name="psaveWidth"></param>
        /// <param name="psaveHeight"></param>
        public void GetPart2(string pPath, string pSavedPath, int pPartLocX, int pPartLocY, int pPartWidth, int pPartHeight, int psaveWidth, int psaveHeight)
        {
            // 加载原始图片，被截的那张
            using (Image origImg = Image.FromFile(pPath))
            {
                // 创建目标图像的大小
                RectangleF destRect = new Rectangle(0, 0, psaveWidth, psaveHeight);

                // 截图区域的大小
                RectangleF srcRect = new RectangleF(0, 0, pPartWidth, pPartHeight);
                // 按截取位置偏移
                srcRect.Offset(pPartLocX, pPartLocY);
                //
                GraphicsUnit units = GraphicsUnit.Pixel;
                // Draw image to screen.
                Bitmap saveImg = new Bitmap(psaveWidth, psaveHeight);
                Graphics graphics = Graphics.FromImage(saveImg);
                graphics.DrawImage(origImg, destRect, srcRect, units);

                origImg.Dispose();
                if (File.Exists(pSavedPath))
                {
                    File.SetAttributes(pSavedPath, FileAttributes.Normal);
                    File.Delete(pSavedPath);
                }

                //修改保存方式，提高图片画质
                //partImg.Save(normalJpgPath, ImageFormat.Jpeg);
                if (pSavedPath.IndexOf(".gif") != -1)
                {
                    saveImg.Save(pSavedPath, ImageFormat.Gif);
                }
                else
                {
                    ImageCodecInfo myImageCodecInfo;
                    System.Drawing.Imaging.Encoder myEncoder;
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;

                    myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameter = new EncoderParameter(myEncoder, 99L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    saveImg.Save(pSavedPath, myImageCodecInfo, myEncoderParameters);
                }
            }
        }


        public static bool MakeThumbnail(string imageUrl, string savePath, int toWidth, int toHeight)
        {
            Image image = Image.FromStream(new MemoryStream(File.ReadAllBytes(imageUrl)));
            return MakeThumbnail(image, savePath, toWidth, toHeight);
        }

        /// <summary>
        /// 制作缩缩图等比环切(luck2fly-2013/03/01)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="savePath"></param>
        /// <param name="toWidth"></param>
        /// <param name="toHeight"></param>
        /// <returns></returns>
        public static bool MakeThumbnail(Image image, string savePath, int toWidth, int toHeight)
        {
            if (File.Exists(savePath))
            {
                File.SetAttributes(savePath, FileAttributes.Normal);
                File.Delete(savePath);
            }

            if (image == null) return false;

            int x = 0;
            int y = 0;
            int ow = image.Width;
            int oh = image.Height;

            if ((double)image.Width / image.Height > (double)toWidth / toHeight)
            {
                oh = image.Height;
                ow = image.Height * toWidth / toHeight;
                x = (image.Width - ow) / 2;
            }
            else
            {
                ow = image.Width;
                oh = image.Width * toHeight / toWidth;
                y = (image.Height - oh) / 2;
            }

            Bitmap bmPhoto = new Bitmap(toWidth, toHeight, PixelFormat.Format24bppRgb);
            Graphics gbmPhoto = Graphics.FromImage(bmPhoto);
            gbmPhoto.InterpolationMode = InterpolationMode.High;
            gbmPhoto.SmoothingMode = SmoothingMode.HighQuality;
            gbmPhoto.Clear(Color.White);
            gbmPhoto.DrawImage(image, new Rectangle(0, 0, toWidth, toHeight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 99L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmPhoto.Save(savePath, myImageCodecInfo, myEncoderParameters);

            gbmPhoto.Dispose();
            bmPhoto.Dispose();
            return true;
        }


        #region VerificationNumber




        #endregion


    }

    public struct ImageInformation
    {
        private int width;
        private int height;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
    }
}
