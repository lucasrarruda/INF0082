using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImageProcessing
{
    internal class GrayScaleSIMD
    {
        public static void ProcessST(string[] imageFiles, string baseOutputPath)
        {
            Directory.CreateDirectory(baseOutputPath);
            foreach (string imagePath in imageFiles)
            {
                ProcessImage(imagePath, baseOutputPath);
            }
        }

        public static void ProcessMT(string[] imageFiles, string baseOutputPath)
        {
            Directory.CreateDirectory(baseOutputPath);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            };

            Parallel.ForEach(imageFiles, parallelOptions, imagePath => ProcessImage(imagePath, baseOutputPath));
        }

        public static async Task ProcessTasks(string[] imageFiles, string baseOutputPath)
        {
            Directory.CreateDirectory(baseOutputPath);

            var tasks = imageFiles.Select(imagePath => Task.Run(() => ProcessImage(imagePath, baseOutputPath)));

            await Task.WhenAll(tasks);
        }

        private static void ProcessImage(string imagePath, string baseOutputPath)
        {
            using Bitmap bitmap = new(imagePath);

            // Bloqueia os bits da imagem para acesso direto à memória
            Rectangle rect = new(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Pega o ponteiro para o primeiro byte
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] pixelData = new byte[bytes];
            Marshal.Copy(ptr, pixelData, 0, bytes);

            // Coeficientes para a conversão em grayscale
            Vector<float> rCoeff = new Vector<float>(0.3f);
            Vector<float> gCoeff = new Vector<float>(0.59f);
            Vector<float> bCoeff = new Vector<float>(0.11f);

            int simdLength = Vector<float>.Count;
            int i = 0;

            // Processamento SIMD para conversão dos pixels em grayscale
            while (i <= pixelData.Length - 3 * simdLength)
            {
                // Criar vetores float manualmente a partir dos dados byte
                float[] rValues = new float[simdLength];
                float[] gValues = new float[simdLength];
                float[] bValues = new float[simdLength];

                for (int j = 0; j < simdLength; j++)
                {
                    rValues[j] = pixelData[i + j * 3];
                    gValues[j] = pixelData[i + j * 3 + 1];
                    bValues[j] = pixelData[i + j * 3 + 2];
                }

                // DICA: 
                // Este é o código normal de conversão de RGB para Grayscale:
                //   for (int y = 0; y < bitmap.Height; y++)
                //   {
                //       for (int x = 0; x < bitmap.Width; x++)
                //       {
                //           Color pixel = bitmap.GetPixel(x, y);
                //           int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
                //           Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                //           bitmap.SetPixel(x, y, grayColor);
                //       }
                //   }
                // Você já tem os valores de Red, Green e Blue, certo?
                // Agora multiplique esses valores pelos coeficientes de cada canal usando Vetorização (SIMD) (R=0.3 , G=0.59, B=0.11) 
                // E no final você vai ter um Vector chamado grayFloat. Então seu trabalho a parte II desse trabalho estará pronta.

                Vector<float> grayFloat = new();/* Aqui é o resultado do SIMD. Seu trabalho de codificação terminaaqui*/

                // Converte o vetor de grayscale para bytes e atualiza os dados
                for (int j = 0; j < simdLength; j++)
                {
                    byte gray = (byte)Math.Clamp((int)grayFloat[j], 0, 255);
                    pixelData[i + j * 3] = gray;
                    pixelData[i + j * 3 + 1] = gray;
                    pixelData[i + j * 3 + 2] = gray;
                }

                i += 3 * simdLength;
            }

            // Processa os pixels restantes fora do loop SIMD
            for (; i < pixelData.Length; i += 3)
            {
                byte r = pixelData[i];
                byte g = pixelData[i + 1];
                byte b = pixelData[i + 2];

                byte gray = (byte)(0.3 * r + 0.59 * g + 0.11 * b);

                pixelData[i] = gray;
                pixelData[i + 1] = gray;
                pixelData[i + 2] = gray;
            }

            // Copia os dados de volta para o bitmap
            Marshal.Copy(pixelData, 0, ptr, bytes);
            bitmap.UnlockBits(bitmapData);

            string outputPath = Path.Combine(baseOutputPath, Path.GetFileName(imagePath));
            bitmap.Save(outputPath, ImageFormat.Jpeg);
        }
    }
}
