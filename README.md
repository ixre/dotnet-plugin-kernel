.NET开源插件内核
====

.NET开源插件内核；支持WinForm和Asp.net.
设计的初衷是：利用“开发平台 + 插件内核"来开发子系统，及对系统进行一些扩展，
定制化开发。

应用案例：http://github.com/atnet/cms

## 如何使用？ ##

### 1.创建插件宿主
实现接口：IPluginHost

### 2. 编写部署插件   

编写插件，并放于指定的目录（默认plugins下)，插件需实现IPlugin接口

### 3. 连接插件:

  IPluginHost.Connect()
  
  
![截图](https://raw.githubusercontent.com/newmin/ntpk/master/snapshot1.png)
