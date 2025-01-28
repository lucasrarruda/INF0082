using ImageProcessing;
using System.Diagnostics;

// During build time it copies the "images" dir to .\bin\blablabla\images.
string inputDir = @".\images";
string outputDir = Path.Combine(Path.GetTempPath(), "ImageGalleryOutput");

Console.WriteLine("----> Reading  from " + inputDir);
Console.WriteLine("----> Writing into " + outputDir);

try
{
    string[] imageFiles = Directory.GetFiles("images");

    Console.WriteLine("This is the Image Processing Example");
    Console.WriteLine("This program converts a bunch of images to grayscale");
  

    Console.WriteLine("(1) Running in a Single Thread ...");
    var sw = Stopwatch.StartNew(); // Start the stopwatch
    GrayScale.ProcessST(imageFiles, Path.Combine(outputDir, "SingleThread"));
    sw.Stop(); // Stop the stopwatch
    Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("(2) Running in Multi Threads -- static partitioner ... ");
    sw.Restart(); // Restart the stopwatch
    GrayScale.ProcessMT(imageFiles, Path.Combine(outputDir, "MultiThread_StaticPart"), true);
    sw.Stop(); // Stop the stopwatch
    Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("(3) Running in Multi Threads -- dynamic partitioner ... ");
    sw.Restart(); // Restart the stopwatch
    GrayScale.ProcessMT(imageFiles, Path.Combine(outputDir, "MultiThread_DynPart"), false);
    sw.Stop(); // Stop the stopwatch
    Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("(4) Running in Multi Tasks -- static partitioner ...");
    sw.Restart(); // Restart the stopwatch
    await GrayScale.ProcessTasks(imageFiles, Path.Combine(outputDir, "MultiTask_StaticPart"), true);
    sw.Stop(); // Stop the stopwatch
    Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("(5) Running in Multi Tasks -- dynamic partitioner ...");
    sw.Restart(); // Restart the stopwatch
    await GrayScale.ProcessTasks(imageFiles, Path.Combine(outputDir, "MultiTask_DynPart"), false);
    sw.Stop(); // Stop the stopwatch
    Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("(6) Running with SIMD Tasks...");
    sw.Restart(); // Restart the stopwatch
    await GrayScaleSIMD.ProcessTasks(imageFiles, Path.Combine(outputDir, "SIMD"));
    sw.Stop(); // Stop the stopwatch
    Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}
