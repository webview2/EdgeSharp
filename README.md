<p align="center"><img src="https://github.com/edgesharp/EdgeSharp/blob/main/src/EdgeSharp.Core/edgesharp.png?raw=true" /></p>
<h1 align="center">EdgeSharp</h1>

EdgeSharp is a .NET HTML5 Win32/WinForms/Wpf [WebView2](https://docs.microsoft.com/en-us/microsoft-edge/webview2/) desktop framework. 

- [Win32](https://github.com/edgesharp/EdgeSharp/tree/main/src/EdgeSharp) - Implements a .NET win32 window to host WebView2 as a browser.
- [WinForms](https://github.com/edgesharp/EdgeSharp/tree/main/src/EdgeSharp.WinForms) - Adds value to base Microsoft WebView2 WinForms offering.
- [Wpf](https://github.com/edgesharp/EdgeSharp/tree/main/src/EdgeSharp.Wpf) - Adds value to base Microsoft WebView2 Wpf offering.



[![EdgeSharp.Core](http://img.shields.io/nuget/vpre/EdgeSharp.Core.svg?style=flat&label=EdgeSharp.Core)](https://www.nuget.org/packages/EdgeSharp.Core)
[![EdgeSharp.Core.Owin](http://img.shields.io/nuget/vpre/EdgeSharp.Core.Owin.svg?style=flat&label=EdgeSharp.Core.Owin)](https://www.nuget.org/packages/EdgeSharp.Core.Owin)
[![EdgeSharp](http://img.shields.io/nuget/vpre/EdgeSharp.svg?style=flat&label=EdgeSharp)](https://www.nuget.org/packages/EdgeSharp)
[![EdgeSharp.WinForms](http://img.shields.io/nuget/vpre/EdgeSharp.WinForms.svg?style=flat&label=EdgeSharp.WinForms)](https://www.nuget.org/packages/EdgeSharp.WinForms)
[![EdgeSharp.Wpf](http://img.shields.io/nuget/vpre/EdgeSharp.Wpf.svg?style=flat&label=EdgeSharp.Wpf)](https://www.nuget.org/packages/EdgeSharp.Wpf)

A basic [EdgeSharp Win32 project](https://github.com/edgesharp/EdgeSharp.Samples/tree/main/win-32/EdgeSharp.Sample) requires:

````csharp
class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        AppBuilder
        .Create()
        .UseConfig<SampleConfig>()
        .UseWindow<SampleWindow>()
        .UseApp<SampleApp>()
        .Build()
        .Run(args);
    }
}
````

### EdgeSharp Samples 
Get started with using [samples](https://github.com/edgesharp/EdgeSharp.Samples). 

![edgesharp_screens_normal](https://user-images.githubusercontent.com/18384207/124758111-4115ba80-defc-11eb-8837-6fac14421fd4.gif)


### References
* WebView2 - https://docs.microsoft.com/en-us/microsoft-edge/webview2/
* Chromium.AspNetCore.Bridge - https://github.com/amaitland/Chromium.AspNetCore.Bridge
