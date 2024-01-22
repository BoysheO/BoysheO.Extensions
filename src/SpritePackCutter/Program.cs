// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using BoysheO.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Console.WriteLine("Hello, World!");

const string path = @"***";
const int CutSizeW = 32;
const int CutSizeH = 32;

var files = Directory.GetFiles(path).Where(v => v.AsPath().GetExtension() == ".png");
foreach (var file in files)
{
    var img = Image.Load<Rgba32>(file);
    int count = 0;
    //var maxCount = img.Width / CutSizeW * img.Height / CutSizeH;
    if (img.Width % CutSizeW != 0 || img.Height % CutSizeH != 0)
    {
        Console.WriteLine($"裁剪尺寸不凑整，大图尺寸{img.Width}w{img.Height}h,裁剪尺寸{CutSizeW}w{CutSizeH}h,文件{file}");
        continue;
    }

    var saveDir = file.AsSpan().SkipTailCount(".png").ToNewString();
    if (Directory.Exists(saveDir)) Directory.Delete(saveDir, true);
    Directory.CreateDirectory(saveDir);
    for (int row = 0; row < img.Height / CutSizeH; row++)
    {
        for (int col = 0; col < img.Width / CutSizeW; col++)
        {
            var px_x = col * CutSizeW;
            var px_y = row * CutSizeH;
            var copy = new Image<Rgba32>(CutSizeW, CutSizeH);
            for (int y = 0; y < CutSizeH; y++)
            {
                for (int x = 0; x < CutSizeW; x++)
                {
                    // Console.WriteLine($"{x},{y},{px_x + x},{px_y + y}");
                    copy[x, y] = img[px_x + x, px_y + y];
                }
            }

            var saveFile = saveDir + $"\\{count}.png";
            if (!IsEmpty(copy))
            {
                count++;
                copy.SaveAsPng(saveFile);
                Console.WriteLine($"save to {saveFile}");
            }
        }
    }
}

Console.WriteLine("done");

bool IsEmpty(Image<Rgba32> img)
{
    for (int i = 0; i < img.Width; i++)
    {
        for (int j = 0; j < img.Height; j++)
        {
            var px = img[i, j];
            if (px.A != 0) return false;
        }
    }

    return true;
}