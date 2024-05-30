// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BoysheO.Extensions;
using HidSharp;

Console.WriteLine("Hello, World!");


// 查找所有HID设备
var list = DeviceList.Local;
var l = list.GetDevices(DeviceTypes.Hid).Select(v => v.DevicePath + "---FriName=" + v.GetFriendlyName())
    .JoinAsOneString("\n");
Console.WriteLine(l);
var joyConA = list.GetHidDevices(0x057E, 0x2007).First();
var joyConsB = list.GetHidDevices(0x057E, 0x2006).First(); // 任天堂Joy-Con的供应商和产品ID
// Console.WriteLine($"joyConA=[{joyConA.Count()}] names={joyConA.Select(v=>v.GetFriendlyName()).JoinAsOneString()} productName={joyConA.Select(v=>v.GetProductName()).JoinAsOneString()}");
// Console.WriteLine($"joyConB=[{joyConsB.Count()}] names={joyConsB.Select(v=>v.GetFriendlyName()).JoinAsOneString()} productName={joyConsB.Select(v=>v.GetProductName()).JoinAsOneString()}");
Console.WriteLine("joyCon print done");

using (var st = joyConA.Open())
{
    
}

//
// if (joyCon != null)
// {
//     using (var stream = joyCon.Open())
//     {
//         byte[] buffer = new byte[49];
//         buffer[0] = 0x01; // 报告ID
//         buffer[1] = 0x40; // 振动命令
//         buffer[2] = 0x40; // 振动命令
//         buffer[3] = 0xff; // 振动强度
//
//         // stream.Write(buffer);
//         stream.SetFeature(buffer); // 发送振动命令
//
//
//         Console.WriteLine("HD震动已启动。按任意键停止...");
//         Console.ReadKey();
//
//         buffer[3] = 0x00; // 停止振动
//         // stream.Write(buffer);
//         stream.SetFeature(buffer);
//
//         Console.WriteLine("HD震动已停止。");
//     }
// }
// else
// {
//     Console.WriteLine("未找到Joy-Con设备。");
// }