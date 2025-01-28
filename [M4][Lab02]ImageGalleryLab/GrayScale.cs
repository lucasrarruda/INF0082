using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing
{
    internal class GrayScale
    {
        public static void ProcessST(string[] imageFiles, string baseOutputPath)
        {
            Directory.CreateDirectory(baseOutputPath);

            foreach (string imagePath in imageFiles)
            {
                ProcessImage(imagePath, baseOutputPath);
            }
        }

        public static void ProcessMT(string[] imageFiles, string baseOutputPath, bool staticPartitioner = true)
        {
            Directory.CreateDirectory(baseOutputPath);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4 // Limitar a 4 threads
            };

            if (staticPartitioner)
            {

            }
            else
            {
                // Dynamic
            }
        }

        public static async Task ProcessTasks(string[] imageFiles, string baseOutputPath, bool staticPartitioner = true)
        {
            Directory.CreateDirectory(baseOutputPath);

            Directory.CreateDirectory(baseOutputPath);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4 // Limitar a 4 threads
            };

            if (staticPartitioner)
            {

            }
            else
            {
                // Dynamic
            }

            /*var tasks = imageFiles.Select(imagePath =>
            {
                return Task.Run(() => ProcessImage(imagePath, baseOutputPath));
            });

            await Task.WhenAll(tasks);*/
        }

        private static void ProcessImage(string imagePath, string baseOutputPath)
        {
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color pixel = bitmap.GetPixel(x, y);
                        int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
                        Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                        bitmap.SetPixel(x, y, grayColor);
                    }
                }
                string outputPath = Path.Combine(baseOutputPath, Path.GetFileName(imagePath));

                bitmap.Save(outputPath, ImageFormat.Jpeg);
            }
        }
    }
}