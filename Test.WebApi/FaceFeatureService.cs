using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test.WebApi
{

    // ─────────────────────────────────────────────────────────────────────────────
    //  数据结构
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 单张人脸的完整检测 + 识别结果
    /// </summary>
    public class FaceResult
    {
        /// <summary>原图坐标系中的人脸矩形</summary>
        public Rect BoundingBox { get; set; }

        /// <summary>SCRFD 检测置信度 [0, 1]</summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 5 个面部关键点 (左眼 / 右眼 / 鼻尖 / 左嘴角 / 右嘴角)，单位：原图像素
        /// </summary>
        public Point2f[] Landmarks { get; set; } = Array.Empty<Point2f>();

        /// <summary>ArcFace 512 维特征向量（已 L2 归一化，可直接做余弦相似度）</summary>
        public float[] FeatureVector { get; set; }

        /// <summary>仿射对齐后的 112×112 人脸图，用于调试或二次处理</summary>
        public Mat AlignedFace { get; set; }

        public override string ToString() =>
            $"Face[Conf={Confidence:F3}, Box={BoundingBox}, Feat={FeatureVector?.Length ?? 0}D]";
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  服务类
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 人脸检测 + 对齐 + 特征提取一体化服务<br/>
    /// 检测/关键点: SCRFD (scrfd_10g_kps.onnx)<br/>
    /// 特征提取:    ArcFace ResNet-100 (arcfaceresnet100-8.onnx)
    /// </summary>
    public sealed class FaceFeatureService : IDisposable
    {
        #region 字段 & 常量

        private readonly InferenceSession _detector;
        private readonly InferenceSession _recognizer;
        private bool _disposed;

        // ── SCRFD 检测参数 ───────────────────────────────────────────
        private const int DetSize = 640;
        private const float DetThreshold = 0.5f;
        private const float NmsThreshold = 0.4f;
        private static readonly int[] Strides = { 8, 16, 32 };
        private const int NumAnchors = 2;

        // ── ArcFace 对齐模板 (112×112 坐标系) ───────────────────────
        private const int RecSize = 112;
        private static readonly Point2f[] AlignTemplate =
        {
        new(38.2946f, 51.6963f),  // 左眼
        new(73.5318f, 51.5014f),  // 右眼
        new(56.0252f, 71.7366f),  // 鼻尖
        new(41.5493f, 92.3655f),  // 左嘴角
        new(70.7299f, 92.2041f),  // 右嘴角
    };

        #endregion

        #region 构造 / 释放

        public FaceFeatureService(string scrfdModelPath, string arcFaceModelPath)
        {
            var opt = new Microsoft.ML.OnnxRuntime.SessionOptions();
            opt.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;

            _detector = new InferenceSession(scrfdModelPath, opt);
            _recognizer = new InferenceSession(arcFaceModelPath, opt);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _detector?.Dispose();
            _recognizer?.Dispose();
            _disposed = true;
        }

        #endregion

        // ═════════════════════════════════════════════════════════════
        //  公共 API
        // ═════════════════════════════════════════════════════════════

        #region 检测 + 识别

        /// <summary>从图像路径检测人脸并提取特征</summary>
        public List<FaceResult> GetFaceResults(string imagePath)
        {
            using var img = Cv2.ImRead(imagePath, ImreadModes.Color);
            if (img.Empty())
                throw new FileNotFoundException($"无法读取图像: {imagePath}");
            return GetFaceResults(img);
        }

        /// <summary>从 Mat 检测人脸并提取特征</summary>
        public List<FaceResult> GetFaceResults(Mat image)
        {
            var faces = DetectFaces(image);
            foreach (var face in faces)
            {
                face.AlignedFace = AlignFace(image, face.Landmarks);
                face.FeatureVector = ExtractFeature(face.AlignedFace);
            }
            return faces;
        }

        #endregion

        #region 相似度比较

        /// <summary>
        /// 计算两张人脸余弦相似度 [-1, 1]<br/>
        /// 经验阈值: &gt;0.28 可能同一人，&gt;0.40 较确定同一人
        /// </summary>
        public float ComputeSimilarity(FaceResult a, FaceResult b)
        {
            if (a.FeatureVector == null || b.FeatureVector == null)
                throw new InvalidOperationException("FeatureVector 为空，请先调用 GetFaceResults");
            return CosineSimilarity(a.FeatureVector, b.FeatureVector);
        }

        /// <summary>
        /// 计算两组人脸的相似度矩阵 [i, j] = facesA[i] 与 facesB[j] 的相似度
        /// </summary>
        public float[,] ComputeSimilarityMatrix(List<FaceResult> facesA, List<FaceResult> facesB)
        {
            var matrix = new float[facesA.Count, facesB.Count];
            for (int i = 0; i < facesA.Count; i++)
                for (int j = 0; j < facesB.Count; j++)
                    matrix[i, j] = CosineSimilarity(facesA[i].FeatureVector, facesB[j].FeatureVector);
            return matrix;
        }

        /// <summary>判断两张人脸是否为同一人</summary>
        public bool IsSamePerson(FaceResult a, FaceResult b, float threshold = 0.40f)
            => ComputeSimilarity(a, b) >= threshold;

        #endregion

        #region 调试绘图

        /// <summary>
        /// 在图像上绘制边界框、关键点、置信度标签，返回新 Mat（原图不修改）
        /// </summary>
        /// <param name="image">原始图像</param>
        /// <param name="faces">GetFaceResults 返回的结果</param>
        /// <param name="savePath">若不为 null，则保存到此路径</param>
        public Mat DrawDebug(Mat image, List<FaceResult> faces, string savePath = null)
        {
            var canvas = image.Clone();

            // 关键点颜色：左眼/右眼/鼻尖/左嘴角/右嘴角
            Scalar[] kpColors =
            {
            new Scalar(0,   0,   220),  // 红 - 左眼
            new Scalar(220, 0,   0  ),  // 蓝 - 右眼
            new Scalar(0,   220, 220),  // 黄 - 鼻尖
            new Scalar(220, 0,   220),  // 紫 - 左嘴角
            new Scalar(0,   220, 0  ),  // 绿 - 右嘴角
        };

            for (int fi = 0; fi < faces.Count; fi++)
            {
                var f = faces[fi];

                // 1. 边界框
                Cv2.Rectangle(canvas, f.BoundingBox, new Scalar(0, 230, 0), 2);

                // 2. 置信度 + 编号
                string label = $"#{fi}  {f.Confidence:F2}";
                int labelY = Math.Max(f.BoundingBox.Y - 8, 18);
                Cv2.PutText(canvas, label,
                    new Point(f.BoundingBox.X, labelY),
                    HersheyFonts.HersheySimplex, 0.55,
                    new Scalar(0, 230, 0), 2);

                // 3. 5 个关键点
                for (int k = 0; k < f.Landmarks.Length; k++)
                {
                    var pt = new Point((int)f.Landmarks[k].X, (int)f.Landmarks[k].Y);
                    Cv2.Circle(canvas, pt, 3, kpColors[k % kpColors.Length], -1);
                }
            }

            // 总人脸数
            Cv2.PutText(canvas, $"Faces: {faces.Count}",
                new Point(8, 30), HersheyFonts.HersheySimplex, 1.0,
                new Scalar(0, 165, 255), 2);

            if (savePath != null) Cv2.ImWrite(savePath, canvas);
            return canvas;
        }

        /// <summary>从路径读取图像后绘制调试图</summary>
        public Mat DrawDebug(string imagePath, List<FaceResult> faces, string savePath = null)
        {
            using var img = Cv2.ImRead(imagePath, ImreadModes.Color);
            return DrawDebug(img, faces, savePath);
        }

        /// <summary>
        /// 并排对比两张图，用连线和相似度分数标注最优匹配人脸对
        /// </summary>
        /// <param name="threshold">相似度阈值，高于此值连线显示绿色，否则红色</param>
        public Mat DrawCompare(
            Mat img1, List<FaceResult> faces1,
            Mat img2, List<FaceResult> faces2,
            float threshold = 0.40f,
            string savePath = null)
        {
            var d1 = DrawDebug(img1, faces1);
            var d2 = DrawDebug(img2, faces2);

            // 统一高度（letterbox 补黑边）
            int h = Math.Max(d1.Rows, d2.Rows);
            var p1 = new Mat(); var p2 = new Mat();
            Cv2.CopyMakeBorder(d1, p1, 0, h - d1.Rows, 0, 0, BorderTypes.Constant, Scalar.Black);
            Cv2.CopyMakeBorder(d2, p2, 0, h - d2.Rows, 0, 0, BorderTypes.Constant, Scalar.Black);
            d1.Dispose(); d2.Dispose();

            var canvas = new Mat();
            Cv2.HConcat(new[] { p1, p2 }, canvas);
            p1.Dispose(); p2.Dispose();

            int offsetX = img1.Cols; // d1 原始宽

            // 为 faces1 中每张人脸找 faces2 中最相似的人脸并连线
            for (int i = 0; i < faces1.Count; i++)
            {
                if (faces1[i].FeatureVector == null) continue;

                float bestSim = float.MinValue;
                int bestJ = -1;
                for (int j = 0; j < faces2.Count; j++)
                {
                    if (faces2[j].FeatureVector == null) continue;
                    float sim = CosineSimilarity(faces1[i].FeatureVector, faces2[j].FeatureVector);
                    if (sim > bestSim) { bestSim = sim; bestJ = j; }
                }
                if (bestJ < 0) continue;

                var c1 = BoxCenter(faces1[i].BoundingBox);
                var c2 = new Point(offsetX + BoxCenter(faces2[bestJ].BoundingBox).X,
                                             BoxCenter(faces2[bestJ].BoundingBox).Y);

                var lineColor = bestSim >= threshold
                    ? new Scalar(0, 230, 0)   // 绿：匹配
                    : new Scalar(0, 0, 230);  // 红：不匹配

                Cv2.Line(canvas, c1, c2, lineColor, 2);
                Cv2.PutText(canvas,
                    $"{bestSim:F3}",
                    new Point((c1.X + c2.X) / 2, (c1.Y + c2.Y) / 2 - 8),
                    HersheyFonts.HersheySimplex, 0.55, lineColor, 1);
            }

            if (savePath != null) Cv2.ImWrite(savePath, canvas);
            return canvas;
        }

        #endregion

        // ═════════════════════════════════════════════════════════════
        //  SCRFD 检测（私有）
        // ═════════════════════════════════════════════════════════════

        #region SCRFD

        private List<FaceResult> DetectFaces(Mat image)
        {
            int srcH = image.Rows, srcW = image.Cols;

            // Letterbox 等比缩放
            float scale = Math.Min((float)DetSize / srcW, (float)DetSize / srcH);
            int newW = (int)(srcW * scale);
            int newH = (int)(srcH * scale);

            using var resized = new Mat();
            Cv2.Resize(image, resized, new Size(newW, newH));

            using var letterbox = new Mat(DetSize, DetSize, MatType.CV_8UC3, Scalar.Black);
            resized.CopyTo(letterbox[new Rect(0, 0, newW, newH)]);

            // 转 NCHW float32 张量并推理
            var tensor = BuildNchwTensor(letterbox, DetSize, DetSize, 127.5f, 128.0f);
            var inName = _detector.InputMetadata.Keys.First();
            using var results = _detector.Run(
                new[] { NamedOnnxValue.CreateFromTensor(inName, tensor) });

            return ParseScrfdOutput(results.ToList(), scale, srcW, srcH);
        }

        private List<FaceResult> ParseScrfdOutput(
            List<DisposableNamedOnnxValue> outputs,
            float scale, int origW, int origH)
        {
            var rawBoxes = new List<float[]>();    // [x1, y1, x2, y2] (letterbox 坐标)
            var rawScores = new List<float>();
            var rawKps = new List<Point2f[]>();

            // InsightFace 官方导出顺序: score_8/16/32, bbox_8/16/32, kps_8/16/32
            var dict = outputs.ToDictionary(o => o.Name);

            for (int si = 0; si < Strides.Length; si++)
            {
                int stride = Strides[si];
                int fH = DetSize / stride;
                int fW = DetSize / stride;
                int N = fH * fW * NumAnchors;

                // 按名称优先，回退到分组索引 (scores: 0-2, bboxes: 3-5, kps: 6-8)
                float[] sc = FetchOutput(dict, outputs, $"score_{stride}", si);
                float[] bx = FetchOutput(dict, outputs, $"bbox_{stride}", si + 3);
                float[] kps = FetchOutput(dict, outputs, $"kps_{stride}", si + 6);

                if (sc == null || bx == null) continue;

                for (int idx = 0; idx < N; idx++)
                {
                    float score = sc[idx];
                    if (score < DetThreshold) continue;

                    // Anchor 中心坐标
                    int gy = (idx / NumAnchors) / fW;
                    int gx = (idx / NumAnchors) % fW;
                    float cx = (gx + 0.5f) * stride;
                    float cy = (gy + 0.5f) * stride;

                    // BBox 解码 (left / top / right / bottom 距离，乘以 stride)
                    float x1 = cx - bx[idx * 4] * stride;
                    float y1 = cy - bx[idx * 4 + 1] * stride;
                    float x2 = cx + bx[idx * 4 + 2] * stride;
                    float y2 = cy + bx[idx * 4 + 3] * stride;

                    rawBoxes.Add(new[] { x1, y1, x2, y2 });
                    rawScores.Add(score);

                    // Keypoints 解码
                    var pts = new Point2f[5];
                    if (kps != null)
                        for (int k = 0; k < 5; k++)
                            pts[k] = new Point2f(
                                cx + kps[idx * 10 + k * 2] * stride,
                                cy + kps[idx * 10 + k * 2 + 1] * stride);
                    rawKps.Add(pts);
                }
            }

            // NMS 过滤重叠框
            var keep = ApplyNms(rawBoxes, rawScores, NmsThreshold);

            float inv = 1f / scale;
            var faces = new List<FaceResult>(keep.Count);
            foreach (int i in keep)
            {
                float x1 = Clamp(rawBoxes[i][0] * inv, 0, origW);
                float y1 = Clamp(rawBoxes[i][1] * inv, 0, origH);
                float x2 = Clamp(rawBoxes[i][2] * inv, 0, origW);
                float y2 = Clamp(rawBoxes[i][3] * inv, 0, origH);

                var kpsScaled = rawKps[i]
                    .Select(p => new Point2f(p.X * inv, p.Y * inv))
                    .ToArray();

                faces.Add(new FaceResult
                {
                    BoundingBox = new Rect((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1)),
                    Confidence = rawScores[i],
                    Landmarks = kpsScaled,
                });
            }
            return faces;
        }

        /// <summary>优先按名字查找 ONNX 输出，失败则按索引回退</summary>
        private static float[] FetchOutput(
            Dictionary<string, DisposableNamedOnnxValue> dict,
            List<DisposableNamedOnnxValue> list,
            string name, int fallbackIdx)
        {
            if (dict.TryGetValue(name, out var val))
                return val.AsEnumerable<float>().ToArray();
            if (fallbackIdx < list.Count)
                return list[fallbackIdx].AsEnumerable<float>().ToArray();
            return null;
        }

        #endregion

        // ═════════════════════════════════════════════════════════════
        //  人脸对齐（私有）
        // ═════════════════════════════════════════════════════════════

        #region 对齐

        private static Mat AlignFace(Mat image, Point2f[] landmarks)
        {
            if (landmarks == null || landmarks.Length < 5)
                return new Mat(); // 无关键点，返回空 Mat

            // 用 5 点仿射相似变换对齐到 ArcFace 标准模板
            using var M = Cv2.EstimateAffinePartial2D(
                InputArray.Create(landmarks),
                InputArray.Create(AlignTemplate));

            var aligned = new Mat();
            Cv2.WarpAffine(image, aligned, M,
                new Size(RecSize, RecSize),
                InterpolationFlags.Cubic,
                BorderTypes.Reflect101);
            return aligned;
        }

        #endregion

        // ═════════════════════════════════════════════════════════════
        //  ArcFace 特征提取（私有）
        // ═════════════════════════════════════════════════════════════

        #region 特征提取

        private float[] ExtractFeature(Mat alignedFace)
        {
            if (alignedFace == null || alignedFace.Empty()) return null;

            var tensor = BuildNchwTensor(alignedFace, RecSize, RecSize, 127.5f, 128.0f);
            var inName = _recognizer.InputMetadata.Keys.First();

            using var results = _recognizer.Run(
                new[] { NamedOnnxValue.CreateFromTensor(inName, tensor) });

            var embedding = results.First().AsEnumerable<float>().ToArray();
            return L2Normalize(embedding); // 归一化后余弦相似度 = 点积
        }

        #endregion

        // ═════════════════════════════════════════════════════════════
        //  通用工具方法（私有 / 静态）
        // ═════════════════════════════════════════════════════════════

        #region 工具

        /// <summary>
        /// 将 OpenCV BGR Mat 转换为 NCHW float32 张量<br/>
        /// 归一化公式: (pixel - mean) / std
        /// </summary>
        private static DenseTensor<float> BuildNchwTensor(Mat img, int h, int w, float mean, float std)
        {
            var tensor = new DenseTensor<float>(new[] { 1, 3, h, w });
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    var p = img.At<Vec3b>(y, x); // BGR
                    tensor[0, 0, y, x] = (p.Item2 - mean) / std; // R
                    tensor[0, 1, y, x] = (p.Item1 - mean) / std; // G
                    tensor[0, 2, y, x] = (p.Item0 - mean) / std; // B
                }
            return tensor;
        }

        /// <summary>L2 归一化向量</summary>
        private static float[] L2Normalize(float[] v)
        {
            double norm = Math.Sqrt(v.Sum(x => (double)x * x));
            if (norm < 1e-10) return v;
            return v.Select(x => (float)(x / norm)).ToArray();
        }

        /// <summary>余弦相似度（要求向量已 L2 归一化）</summary>
        public static float CosineSimilarity(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                throw new ArgumentException("特征向量维度不一致");
            float dot = 0f;
            for (int i = 0; i < a.Length; i++) dot += a[i] * b[i];
            return dot;
        }

        /// <summary>单类别 NMS</summary>
        private static List<int> ApplyNms(List<float[]> boxes, List<float> scores, float iouThresh)
        {
            var order = Enumerable.Range(0, scores.Count)
                                  .OrderByDescending(i => scores[i])
                                  .ToList();
            var keep = new List<int>();
            while (order.Count > 0)
            {
                int cur = order[0]; keep.Add(cur);
                order.RemoveAt(0);
                order = order.Where(i => BoxIoU(boxes[cur], boxes[i]) <= iouThresh).ToList();
            }
            return keep;
        }

        private static float BoxIoU(float[] a, float[] b)
        {
            float ix = Math.Max(0f, Math.Min(a[2], b[2]) - Math.Max(a[0], b[0]));
            float iy = Math.Max(0f, Math.Min(a[3], b[3]) - Math.Max(a[1], b[1]));
            float inter = ix * iy;
            float areaA = (a[2] - a[0]) * (a[3] - a[1]);
            float areaB = (b[2] - b[0]) * (b[3] - b[1]);
            float uni = areaA + areaB - inter;
            return uni <= 0f ? 0f : inter / uni;
        }

        private static float Clamp(float v, float lo, float hi)
            => v < lo ? lo : v > hi ? hi : v;

        private static Point BoxCenter(Rect r)
            => new Point(r.X + r.Width / 2, r.Y + r.Height / 2);

        #endregion
    }

}
