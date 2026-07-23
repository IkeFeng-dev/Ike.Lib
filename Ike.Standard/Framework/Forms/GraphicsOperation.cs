#if NETFRAMEWORK

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ike.Standard.Forms
{
    /// <summary>
    /// 图像绘制类相关操作
    /// </summary>
    public static partial class GraphicsOperation
    {

        /// <summary>
        /// 获取随机色
        /// </summary>
        /// <returns>返回一个随机颜色的<see cref="Color"/></returns>
        public static Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return Color.FromArgb(int_Red, int_Green, int_Blue);
        }



        /// <summary>
        /// 获取加深颜色
        /// </summary>
        /// <param name="color">要加深的颜色</param>
        /// <param name="max">调整最大加深值</param>
        /// <returns>返回加深颜色后的<see cref="Color"/></returns>
        public static Color GetDarkerColor(Color color, byte max)
        {
            //还可以根据需要调整此处的值
            int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255);
            int r = Math.Abs(Math.Min(color.R - increase, max));
            int g = Math.Abs(Math.Min(color.G - increase, max));
            int b = Math.Abs(Math.Min(color.B - increase, max));
            return Color.FromArgb(r, g, b);
        }



        /// <summary>
        /// 计算文本的像素宽度
        /// </summary>
        /// <param name="str">文本内容</param>
        /// <param name="font">文本字体</param>
        /// <returns></returns>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static int GetStringWidth(string str, Font font)
        {
            if (str.Length == 0) return 0;
            Size sif = TextRenderer.MeasureText(str, font, new Size(0, 0), TextFormatFlags.NoPadding);
            return sif.Width;
        }



        /// <summary>
        /// 在矩形中绘制文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="color">颜色</param>
        /// <param name="backColor">背景颜色</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="font">字体</param>
        /// <param name="graphics">对象</param>
        /// <param name="stringAlignment">文本在矩形中的对齐方式</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void DrawText(string text, Color color, Color backColor, float x, float y, Font font, Graphics graphics, StringAlignment stringAlignment)
        {
            SolidBrush drawBrush = new SolidBrush(color);
            RectangleF drawRect = new RectangleF(x, y, GetStringWidth(text, font), font.Height);
            Pen blackPen = new Pen(backColor);
            graphics.DrawRectangle(blackPen, x, y, GetStringWidth(text, font), font.Height);
            StringFormat drawFormat = new StringFormat
            {
                Alignment = stringAlignment
            };
            graphics.DrawString(text, font, drawBrush, drawRect, drawFormat);
        }



        /// <summary>
        /// 绘制两色渐变色
        /// </summary>
        /// <param name="graphics">控件对象</param>
        /// <param name="rectangle">控件矩形</param>
        /// <param name="startColor">开始颜色</param>
        /// <param name="endColor">结束颜色</param>
        /// <param name="linearGradientMode">渐变色绘制方向</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void DrawGradient(Graphics graphics, Rectangle rectangle, Color startColor, Color endColor, LinearGradientMode linearGradientMode)
        {
            Brush b = new LinearGradientBrush(rectangle, startColor, endColor, linearGradientMode);
            graphics.FillRectangle(b, rectangle);
        }




        /// <summary>
        /// 绘制指定圆角半径矩形边框
        /// </summary>
        /// <param name="rectangle">原始矩形</param>
        /// <param name="graphics">Graphics 对象</param>
        /// <param name="radius">圆角半径,如果小于等于0 则绘制正方形</param>
        /// <param name="backColor">颜色</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void DrawBorder(Rectangle rectangle, Color backColor, int radius, ref Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (radius <= 0)
            {
                radius = 1;
            }
            graphics.DrawPath(new Pen(backColor), DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - 2, rectangle.Height - 1, radius));
        }




        /// <summary>
        /// 绘制圆角边框【可设置四个角不同的圆角】
        /// </summary>
        /// <param name="rectangle">控件的范围</param>
        /// <param name="graphics">Graphics</param>
        /// <param name="top_left_radius">左上角的圆角</param>
        /// <param name="top_right_radius">右上角的圆角</param>
        /// <param name="bottom_left_radius">底部左边的圆角</param>
        /// <param name="bottom_right_radius">底部右边的圆角</param>
        /// <param name="backColor">要描述的边框的背景的颜色</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void Draworder(Rectangle rectangle, Color backColor, int top_left_radius, int top_right_radius, int bottom_left_radius, int bottom_right_radius, ref Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (top_left_radius <= 0)
            {
                top_left_radius = 1;
            }
            graphics.DrawPath(new Pen(backColor), DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1, top_left_radius, top_right_radius, bottom_left_radius, bottom_right_radius));
        }





        /// <summary>
        /// 圆角边框,边框可加粗【可设置四个角不同的圆角】
        /// </summary>
        /// <param name="rectangle">原始矩形</param>
        /// <param name="graphics">对象</param>
        /// <param name="backColor">边框颜色</param>
        /// <param name="radius">圆角半径</param>
        /// <param name="linewide">边框宽度</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void DrawSolidBorder(Rectangle rectangle, Color backColor, int radius, int linewide, ref Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (radius <= 0)
            {
                graphics.DrawRectangle(new Pen(backColor, linewide), rectangle);
            }
            else
            {
                graphics.DrawPath(new Pen(backColor, linewide), DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - 2, rectangle.Height - 1, radius));
            }
        }


        /// <summary>
        /// 绘制矩形【可设置四个角不同的圆角】
        /// </summary>
        /// <param name="rectangle">初始矩形</param>
        /// <param name="graphics">对象</param>
        /// <param name="backColor">背景色</param>
        /// <param name="radius_topleft">左上角</param>
        /// <param name="radius_topright">右上角</param>
        /// <param name="radius_bottom_left">左下角</param>
        /// <param name="radius_bottomright">右下角</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void DrawRectangle(Rectangle rectangle, Color backColor, int radius_topleft, int radius_topright, int radius_bottom_left, int radius_bottomright, ref Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (radius_topleft == 0)
            {
                graphics.FillRectangle(new SolidBrush(backColor), rectangle);
            }
            else
            {
                graphics.FillPath(new SolidBrush(backColor), DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - 2, rectangle.Height - 1, radius_topleft, radius_topright, radius_bottom_left, radius_bottomright));
            }
        }





        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="rectangle">原始矩形</param>
        /// <param name="graphics">Graphics 对象</param>
        /// <param name="radius">圆角半径</param>
        /// <param name="backColor">颜色</param>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static void DrawRectangle(Rectangle rectangle, Color backColor, int radius, ref Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (radius <= 0)
            {
                radius = 1;
            }
            graphics.FillPath(new SolidBrush(backColor), DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - 2, rectangle.Height - 1, radius));
        }





        /// <summary>
        /// 绘制圆角<see cref="GraphicsPath"/>
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <returns>绘制的<see cref="GraphicsPath"/>对象</returns>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static GraphicsPath DrawFilletGraphicsPath(float X, float Y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(X + radius, Y, (X + width) - (radius * 2f), Y);
            path.AddArc((X + width) - (radius * 2f), Y, radius * 2f, radius * 2f, 270f, 90f);
            path.AddLine((float)(X + width), (float)(Y + radius), (float)(X + width), (float)((Y + height) - (radius * 2f)));
            path.AddArc((float)((X + width) - (radius * 2f)), (float)((Y + height) - (radius * 2f)), (float)(radius * 2f), (float)(radius * 2f), 0f, 90f);
            path.AddLine((float)((X + width) - (radius * 2f)), (float)(Y + height), (float)(X + radius), (float)(Y + height));
            path.AddArc(X, (Y + height) - (radius * 2f), radius * 2f, radius * 2f, 90f, 90f);
            path.AddLine(X, (Y + height) - (radius * 2f), X, Y + radius);
            path.AddArc(X, Y, radius * 2f, radius * 2f, 180f, 90f);
            path.CloseFigure();
            return path;
        }





        /// <summary>
        /// 根据RectangleF绘制图形
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="radius">半径</param>
        /// <returns>绘制的<see cref="GraphicsPath"/>对象</returns>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static GraphicsPath GetRectangleFFillet(RectangleF rect, float radius)
        {
            return DrawFilletGraphicsPath(rect.X, rect.Y, rect.Width, rect.Height, radius);
        }




        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="radius_topleft"></param>
        /// <param name="radius_topright"></param>
        /// <param name="radius_bottomleft"></param>
        /// <param name="radius_bottomright"></param>
        /// <returns>绘制的<see cref="GraphicsPath"/>对象</returns>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius_topleft, int radius_topright, int radius_bottomleft, int radius_bottomright)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x, y, radius_topleft, radius_topleft, 180, 90);
            gp.AddArc(x + width - radius_topright, y, radius_topright, radius_topright, 270, 90);
            gp.AddArc(x + width - radius_bottomleft, y + height - radius_bottomleft, radius_bottomleft, radius_bottomleft, 0, 90);
            gp.AddArc(x, y + height - radius_bottomright, radius_bottomright, radius_bottomright, 90, 90);
            gp.CloseAllFigures();
            return gp;
        }





        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="radius">半径</param>
        /// <returns>绘制的<see cref="GraphicsPath"/>对象</returns>
        /// <remarks>
        /// [<see langword="2026年5月5日14:52:08" />]
        /// </remarks>
        public static GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x, y, radius, radius, 180, 90);
            gp.AddArc(x + width - radius, y, radius, radius, 270, 90);
            gp.AddArc(x + width - radius, y + height - radius, radius, radius, 0, 90);
            gp.AddArc(x, y + height - radius, radius, radius, 90, 90);
            gp.CloseAllFigures();
            return gp;
        }





    }
}


#endif