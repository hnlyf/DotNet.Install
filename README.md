# DotNet.Install
简单方便的将.Net Core程序以服务启动，支持Window和Unix（目前仅测试了CentOS）环境。QQ群：45132984
测试代码
<pre style="font-family:新宋体;font-size:13px;color:gainsboro;background:#1e1e1e;"><span style="color:#569cd6;">class</span>&nbsp;<span style="color:#4ec9b0;">Program</span>
&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:#569cd6;">static</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">Main</span>(<span style="color:#569cd6;">string</span>[]&nbsp;<span style="color:#9cdcfe;">args</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DotNet<span style="color:#b4b4b4;">.</span>Install<span style="color:#b4b4b4;">.</span><span style="color:#4ec9b0;">ServiceInstall</span><span style="color:#b4b4b4;">.</span><span style="color:#dcdcaa;">Run</span>(<span style="color:#d69d85;">&quot;服务名称&quot;</span>,&nbsp;<span style="color:#9cdcfe;">args</span>,&nbsp;<span style="color:#dcdcaa;">Run</span>,<span style="color:#d69d85;">&quot;服务器显示名称&quot;</span>,<span style="color:#d69d85;">&quot;服务说明&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:#569cd6;">static</span>&nbsp;<span style="color:#569cd6;">void</span>&nbsp;<span style="color:#dcdcaa;">Run</span>(<span style="color:#569cd6;">string</span>[]&nbsp;<span style="color:#9cdcfe;">args</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:#57a64a;">//启动服务</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;}</pre>
    QQ群：45132984
    
   
