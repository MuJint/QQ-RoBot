# 简介
Qiushui-Bot是一个基于onebot协议的机器人/当然目前只写了部分我常用的功能，框架已写好，需要添加指令请自行根据相应逻辑添加。<br />
请注意：是基于`.Net5`所写
# 功能概览
* 来点色图
* 人工智障对话
* 莲Bot（基于天刀整容交流团一系列命令）
* 其它娱乐功能
# 平台兼容性
>注意<br />
>仅支持反向WebSocket通讯方式<br />
>仅支持Array事件上报格式<br />

可以运行在支持[onebot](https://github.com/howmanybots/onebot)协议的平台下<br />
不需要任何运行环境<br />
使用单文件模式进行编译（其实还有些运行时库没法打包就是了）<br />
对Windows/Liunx/OSX平台都进行了支持（包括ARM架构）<br />
~~如果其他平台上有什么运行问题一定要提issue啊~~<br />
如果出现闪退现象，请前往微软[下载](https://dotnet.microsoft.com/download).Net5环境，请根据自身电脑环境执行下载
# 开源协议
![mahua](https://camo.githubusercontent.com/3bd1dd6998bcac11dad3430fc4213d8f979b5b133b0e8f66018917be06e3f8f7/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f434247616e2f537569736569426f743f7374796c653d666f722d7468652d6261646765)
# 声明
* Qiushui-Bot是完全免费且开放源代码的软件，仅供学习和娱乐用途使用，不会进行一切形式的收费
* 如果对源代码进行的引用或修改并发布的版本请使用`AGPLv3`开源协议
* 本插件不支持且不鼓励一切的商业用途
# 使用
# 底层框架
[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)<br />
[cqhttp-mirai](https://github.com/yyuueexxiinngg/cqhttp-mirai)<br />
[mirai](https://github.com/mamoe/mirai)<br />
[mirai-console](https://github.com/mamoe/mirai-console)
# 使用到开源库
[System.Text.Encoding.CodePages](https://github.com/dotnet/runtime/tree/master/src/libraries/System.Text.Encoding.CodePages)<br />
[SharpYaml](https://github.com/xoofx/SharpYaml)<br />
[Sora](https://github.com/Yukari316/Sora)<br />
[LiteDB](https://www.litedb.org/)<br />
[ChatterBot](https://github.com/gunthercox/ChatterBot)`人工智障`开源库
