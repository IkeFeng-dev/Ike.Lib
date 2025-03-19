using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ike.Framework
{
    /// <summary>
    /// 一些公共方法
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 图像压缩读取
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <param name="compressRate">图像压缩率,相比较原图压缩的宽高倍数</param>
        /// <returns>读取并且压缩后的图像</returns>
        public static Image ImageCompression(string imagePath,int compressRate)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("file not found",imagePath);
            }
            // 加载原始图像
            using (Image originalImage = Image.FromFile(imagePath))
            {
                // 创建一个新的 Bitmap 对象，调整尺寸或颜色深度
                Bitmap compressedImage = new Bitmap(originalImage.Width / compressRate, originalImage.Height / compressRate);
                using (Graphics g = Graphics.FromImage(compressedImage))
                {
                    g.DrawImage(originalImage, 0, 0, compressedImage.Width, compressedImage.Height);
                }
                return compressedImage;
            }
        }
    }
}
