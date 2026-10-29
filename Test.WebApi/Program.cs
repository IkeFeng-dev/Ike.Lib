using Microsoft.ML.OnnxRuntime;
using OpenCvSharp;
using System.Text;
using Ike.Standard.Controller;
using System.Diagnostics;

namespace Test.WebApi
{
    public class Program
    {

        

        public static void Main(string[] args)
        {

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;



            #region 测试


            Console.ReadLine();

            /*
            string scrfdModel = @"F:\Desktop\MediaHub\models\scrfd_10g_kps.onnx";

            string arcFaceModel =  @"F:\Desktop\MediaHub\models\arcfaceresnet100-8.onnx";

            using var faceService = new FaceFeatureService(scrfdModel, arcFaceModel);

            // ── 检测 + 提取特征 ──────────────────────────────────────────────
            List<FaceResult> faces1 = faceService.GetFaceResults(@"F:\Desktop\storage\photos\D24071246.jpg");
            List<FaceResult> faces2 = faceService.GetFaceResults(@"F:\Desktop\storage\photos\D14060371.jpg");

            Console.WriteLine($"图1人脸数: {faces1.Count}");
            Console.WriteLine($"图2人脸数: {faces2.Count}");
            faces1.ForEach(f => Console.WriteLine(f));

            // ── 相似度比较 ───────────────────────────────────────────────────
            if (faces1.Count > 0 && faces2.Count > 0)
            {
                float sim = faceService.ComputeSimilarity(faces1[0], faces2[0]);
                bool same = faceService.IsSamePerson(faces1[0], faces2[0], threshold: 0.40f);
                Console.WriteLine($"相似度: {sim:F4}  →  {(same ? "同一人" : "不同人")}");
            }

            // ── 多人相似度矩阵 ───────────────────────────────────────────────
            float[,] matrix = faceService.ComputeSimilarityMatrix(faces1, faces2);

            // ── 调试图像 ─────────────────────────────────────────────────────
            using var debug1 = faceService.DrawDebug(
                @"F:\Desktop\storage\photos\D24071246.jpg", faces1,
                @"F:\Desktop\storage\debug\debug1.jpg");

            // ── 并排对比图（连线标注最优匹配） ───────────────────────────────
            using var img1 = Cv2.ImRead(@"F:\Desktop\storage\photos\D15110683.jpg");
            using var img2 = Cv2.ImRead(@"F:\Desktop\storage\photos\MVIMG_20261004_130922.jpg");
            using var compare = faceService.DrawCompare(img1, faces1, img2, faces2,
                threshold: 0.40f, savePath: @"F:\Desktop\storage\debug\compare.jpg");

            Cv2.ImShow("Compare", compare);
            Cv2.WaitKey();

            */

            #endregion





            //Console.ReadLine();
            //Environment.Exit(0);
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers()
            .AddCommonControllers(options =>
            {
                // 语义化快捷启用
                options.EnableFileRelease();
            });

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            app.Run();
        }
    }
}
