using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ike
{
	/// <summary>
	/// OpenCV操作类
	/// </summary>
	public class OpenCvSharp4
	{
		/// <summary>
		/// 检查矩形数值是否正确
		/// </summary>
		/// <param name="rect">矩形结构</param>
		/// <returns>返回检查结果</returns>
		public bool CheckRect(Rect rect)
		{
			if (rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 将十六进制颜色字符串转换为<see cref="Scalar"/>
		/// </summary>
		/// <param name="hexColor">十六进制颜色,例如<see langword="#FFFFFF"/>或<see  langword="FFFFFF"/></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">十六进制颜色格式错误</exception>
		public Scalar HexToBGR(string hexColor)
		{
			if (hexColor.StartsWith("#"))
			{
				hexColor = hexColor.Substring(1);
			}
			if (hexColor.Length != 6)
			{
				throw new ArgumentException("Invalid hexadecimal color format",nameof(hexColor));
			}
			int red = Convert.ToInt32(hexColor.Substring(0, 2), 16);
			int green = Convert.ToInt32(hexColor.Substring(2, 2), 16);
			int blue = Convert.ToInt32(hexColor.Substring(4, 2), 16);
			return new Scalar(blue, green, red);
		}

		/// <summary>
		/// 校验0-255范围的值,不在范围内取最大或最小值
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		private int Clamp(int value)
		{
			if (value < 0) return 0;
			if (value > 255) return 255;
			return value;
		}

		/// <summary>
		/// RGB转YUV
		/// </summary>
		/// <param name="r">红色</param>
		/// <param name="g">绿色</param>
		/// <param name="b">蓝色</param>
		/// <returns></returns>
		public (double Y, double U, double V) RgbToYuv(byte r, byte g, byte b)
		{
			double y = 0.299 * r + 0.587 * g + 0.114 * b;
			double u = -0.14713 * r - 0.28886 * g + 0.436 * b;
			double v = 0.615 * r - 0.51499 * g - 0.10001 * b;
			return (y, u, v);
		}


		/// <summary>
		/// YUV转RGB
		/// </summary>
		/// <param name="y">亮度</param>
		/// <param name="u">绿色色度</param>
		/// <param name="v">蓝色色度</param>
		/// <returns></returns>
		public (int R, int G, int B) YuvToRgb(double y, double u, double v)
		{
			int r = (int)(y + 1.13983 * v);
			int g = (int)(y - 0.39465 * u - 0.58060 * v);
			int b = (int)(y + 2.03211 * u);
			return (Clamp(r), Clamp(g), Clamp(b));
		}


		/// <summary>
		/// 在图像上框选指定矩形区域
		/// </summary>
		/// <param name="image">目标图像</param>
		/// <param name="isClone"><see langword="true"/>克隆新对象, <see langword="false"/>在 <paramref name="image"/>中直接操作 </param>
		/// <param name="rect">矩形区域</param>
		/// <param name="color">框选颜色（BGR格式）</param>
		/// <param name="thickness">矩形边框的线条厚度。如果值为负数（如 -1），则表示填充矩形</param>
		/// <param name="lineTypes">
		/// <remark>
		/// <see cref="LineTypes.Link4"/>：4 连通线<br/>
		/// <see cref="LineTypes.Link8"/>：8 连通线<br/>
		/// <see cref="LineTypes.AntiAlias"/>：抗锯齿线<br/>
		/// </remark>
		/// </param>
		/// <param name="shift">点坐标的小数位数。通常为 0，表示使用整数坐标</param>
		/// <exception cref="ArgumentNullException">图像为空</exception>
		/// <exception cref="ArgumentException">矩形区域无效</exception>
		public Mat DrawRectangle(Mat image, bool isClone, Rect rect, Scalar color, int thickness = 1, LineTypes lineTypes = LineTypes.Link8, int shift = 0)
		{
			// 检查图像是否为空
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}
			if (image.Empty())
			{
				throw new ArgumentException("Image is empty.", nameof(image));
			}

			// 检查矩形区域是否有效
			if (!CheckRect(rect))
			{
				throw new ArgumentException("Invalid rectangle region.", nameof(rect));
			}
			if (isClone)
			{
				Mat clone = image.Clone();
				Cv2.Rectangle(clone, rect, color, thickness);
				return clone;
			}
			else
			{
				Cv2.Rectangle(image, rect, color, thickness, lineTypes, shift);
				return image;
			}
		}


		/// <summary>
		/// 图像模板匹配
		/// </summary>
		/// <param name="largeImage">大图,用于在此图中找样图<paramref name="template"/></param>
		/// <param name="template">样图</param>
		/// <param name="matchRect">如果匹配成功,则返回匹配的矩形区域</param>
		/// <param name="matchThreshold">如果匹配成功,则返回匹配的阈值数值</param>
		/// <param name="threshold">设置匹配阈值,阈值范围根据<see cref="TemplateMatchModes"/>类型设定</param>
		/// <param name="matchModes">
		/// <remarks>
		/// 这是匹配模式，用于指定匹配的计算方法 <br/>
		/// 常用模式：<br/>
		/// <see cref="TemplateMatchModes.SqDiff"/>：平方差匹配,阈值范围[0, +∞],值越小匹配越好,对光照敏感<br/>
		/// <see cref="TemplateMatchModes.SqDiffNormed"/>：归一化平方差匹配,阈值范围[0, 1],推荐阈值[0.1 - 0.2],对光照变化有一定的鲁棒性<br/>
		/// <see cref="TemplateMatchModes.CCorr"/>：相关匹配,阈值范围[0, +∞],值越大匹配越好，对光照敏感<br/>
		/// <see cref="TemplateMatchModes.CCorrNormed"/>：归一化相关匹配,阈值范围[0, 1],推荐阈值[0.8 - 0.95],值越大匹配越好<br/>
		/// <see cref="TemplateMatchModes.CCoeff"/>：相关系数匹配,阈值范围[-1, 1],推荐阈值[0.8 - 0.95],值越大匹配越好<br/>
		/// <see cref="TemplateMatchModes.CCoeffNormed"/>：归一化相关系数匹配,阈值范围[-1, 1],推荐阈值[0.8 - 0.95],值越大匹配越好<br/>
		/// 推荐模式：<br/>
		/// <see cref="TemplateMatchModes.CCoeffNormed"/>：归一化相关系数匹配，对光照变化有一定的鲁棒性，适合大多数场景
		/// </remarks>
		/// </param>
		/// <returns>返回是否匹配结果</returns>
		/// <exception cref="ArgumentNullException">参数为<see langword="null"/> </exception>
		/// <exception cref="ArgumentException">参数错误</exception>
		public bool MatchTemplate(Mat largeImage, Mat template, out Rect matchRect, out double matchThreshold, double threshold = 0.8, TemplateMatchModes matchModes = TemplateMatchModes.CCoeffNormed)
		{
			matchRect = new Rect(0, 0, 0, 0);
			matchThreshold = 0;
			// 检查图像是否为空及是否加载成功
			if (largeImage == null)
			{
				throw new ArgumentNullException(nameof(largeImage));
			}
			if (largeImage.Empty())
			{
				throw new ArgumentException(nameof(largeImage));
			}
			if (template == null)
			{
				throw new ArgumentNullException(nameof(template));
			}
			if (template.Empty())
			{
				throw new ArgumentException(nameof(template));
			}
			// 创建匹配结果矩阵
			using (Mat result = new Mat())
			{
				Cv2.MatchTemplate(largeImage, template, result, matchModes);
				// 获取匹配结果中的最大值和最小值
				Cv2.MinMaxLoc(result, out _, out matchThreshold, out _, out Point maxLoc);
				// 设置匹配阈值
				if (matchThreshold >= threshold)
				{
					// 获取小图的尺寸
					Size templateSize = template.Size();
					// 创建匹配矩形
					matchRect = new Rect(maxLoc, templateSize);
					return true;
				}
				return false;
			}
		}


		/// <summary>
		/// 根据轮廓值计算出矩形  (单个轮廓可直接调用<see cref="Cv2.BoundingRect(InputArray)"/>计算)
		/// </summary>
		/// <param name="contours">轮廓数组</param>
		/// <param name="filterRect">过滤小于此矩形面积的数据</param>
		/// <returns>返回轮廓对应的矩形数组</returns>
		public Rect[] ContoursToRects(Point[][] contours,  Rect filterRect = default)
		{ 
			List<Rect> rects = new List<Rect>();
			bool isFilter = !(filterRect.Height == 0 && filterRect.Width == 0);
			double area = isFilter ? (filterRect.Width * filterRect.Height) : 0;
			foreach (var contour in contours)
			{
				Rect rect = Cv2.BoundingRect(contour);
				if (!isFilter || area > (rect.Width * rect.Height))
				{
					rects.Add(rect);
				}
			}
			return rects.ToArray();
		}



		/// <summary>
		/// 在图像中查找指定颜色区域，并返回所有轮廓
		/// </summary>
		/// <param name="image">目标图像</param>
		/// <param name="targetColor">目标颜色 (BGR 格式)</param>
		/// <param name="hierarchies">输出的轮廓层次结构，描述轮廓之间的关系</param>
		/// <param name="filterContour">过滤面积小于当前值的轮廓,0表示不过滤</param>
		/// <param name="threshold">颜色匹配阈值 (根据实际情况设定,值越小匹配度越高)</param>
		/// <param name="retrievalModes">
		/// <remark>
		/// 轮廓检索模式:<br/>
		/// <see cref="RetrievalModes.External"/>：只提取最外层的轮廓<br/>
		/// <see cref="RetrievalModes.List"/>：提取所有轮廓，不建立层次结构<br/>
		/// <see cref="RetrievalModes.CComp"/>：提取所有轮廓，并将它们组织为两级层次结构（外层和内层）<br/>
		/// <see cref="RetrievalModes.Tree"/>：提取所有轮廓，并建立完整的层次结构<br/>
		/// </remark>
		/// </param>
		/// <param name="contourApproximationModes">
		/// <remark>
		/// 轮廓近似方法:<br/>
		/// <see cref="ContourApproximationModes.ApproxNone"/>：存储所有轮廓点，不进行近似<br/>
		/// <see cref="ContourApproximationModes.ApproxSimple"/>：压缩水平、垂直和对角线段，仅保留端点<br/>
		/// <see cref="ContourApproximationModes.ApproxTC89L1"/>：使用 Teh-Chin 链近似算法（L1 版本）<br/>
		/// <see cref="ContourApproximationModes.ApproxTC89KCOS"/>：使用 Teh-Chin 链近似算法（KCOS 版本）<br/>
		/// </remark>
		/// </param>
		/// <returns>返回找到的轮廓数组</returns>
		public Point[][] FindColorContour(Mat image, Scalar targetColor, out HierarchyIndex[] hierarchies, int filterContour = 0, int threshold = 10, RetrievalModes retrievalModes = RetrievalModes.External, ContourApproximationModes contourApproximationModes = ContourApproximationModes.ApproxSimple)
		{
			if (image == null || image.Empty())
			{
				throw new ArgumentException("Image is null or empty.", nameof(image));
			}

			// 创建掩码矩阵
			using (Mat mask = new Mat())
			{
				// 设置目标颜色的阈值范围
				Scalar lowerBound = new Scalar(
					Math.Max(0, targetColor.Val0 - threshold), // B 通道
					Math.Max(0, targetColor.Val1 - threshold), // G 通道
					Math.Max(0, targetColor.Val2 - threshold)  // R 通道
				);
				Scalar upperBound = new Scalar(
					Math.Min(255, targetColor.Val0 + threshold), // B 通道
					Math.Min(255, targetColor.Val1 + threshold), // G 通道
					Math.Min(255, targetColor.Val2 + threshold)  // R 通道
				);
				// 生成掩码
				Cv2.InRange(image, lowerBound, upperBound, mask);
				// 查找轮廓
				Cv2.FindContours(mask, out Point[][] contours, out hierarchies, retrievalModes, contourApproximationModes);
				if (filterContour > 0)
				{
					List<Point[]> points = new List<Point[]>();
					foreach (var contour in contours)
					{
						if (Cv2.ContourArea(contour) > filterContour)
						{
							points.Add(contour);
						}
					}
					contours = points.ToArray();

				}
				return contours;
			}
		}






	}
}
