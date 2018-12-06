# Worker
为老板维护大批服务器时衍生的项目，现已脱离最初目的开始放飞自我。这个项目现在的目标是实现一个跨平台的设备统一管理系统：作为后台服务运行的Worker连接到Server上，监听Server发来的命令并作出响应。它将完全基于.NET的托管线程池和`await/async`异步实现，支持AES加密传输，并且以跨平台、可复用、高扩展为目标。

### 平台支持情况

* .NET Framework下的客户端`WorkerService`项目，运行于Windows操作系统
* .NET Core下的客户端`WorkerCore`项目，运行于Core支持的操作系统(Windows, Ubuntu, etc.)
* .NET Framework下的服务端`Server`项目，运行于Windows操作系统

### 各平台功能支持情况

<table border="1">
  <tr align="left">
    <th>项目</th>
    <th>系统信息</th>
    <th>远程命令行</th>
    <th>进程管理</th>
    <th>文件上传</th>
    <th>文件下载</th>
    <th>从URL下载</th>
    <th>屏幕快照</th>
    <th>Camera快照</th>
  </tr>
  <tr>
    <td><b>Server</b></td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
  </tr>
  <tr>
    <td><b>WorkerService</b></td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>Y</td> 
  </tr>
  <tr>
    <td><b>WorkerCore</b></td> 
    <td>Y</td> 
    <td>Y</td> 
    <td>N</td> 
    <td>N</td> 
    <td>N</td> 
    <td>N</td> 
    <td>N</td> 
    <td>N</td> 
  </tr>
</table>

### 近期的工作

* 完善WorkerCore的功能。
* 支持其它设备（Windows, Linux, Android, etc.）登陆到Server来间接管理Server上的Worker们。
* 给Server在.NET Core平台实现ServerCore项目，使其可部署在Linux下。