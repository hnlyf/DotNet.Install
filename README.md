# DotNet.Install
简单方便的将.Net Core程序以服务启动，支持Window和Unix（目前仅测试了CentOS）环境。QQ群：45132984
测试代码
class Program
    {
        [Description("这是一个测试服务")]
        [DisplayName("测试服务")]
        static void Main(string[] args)
        {
            DotNet.Install.ServiceInstall.Run("test", args, Run);
        }
        static void Run(string[] args)
        {
            HttpServer httpServer = new HttpServer();
            httpServer.Start();
        }
    }
    QQ群：45132984
    
   
