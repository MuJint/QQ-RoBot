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
* 如果只需要运行在本地，不需要源代码，请[下载](https://changqing-1253469534.cos.ap-chengdu.myqcloud.com/Release.rar)我已经打包好的包
  * 简单使用，首先解压压缩包，打开`Login`文件夹中`go-cqhttp.exe`将会生成默认的`config.json`，请配置默认如下
  * ```Json
    {
      "uin": '登陆账号',
      "password": "登陆密码",
      "encrypt_password": false,
      "password_encrypted": "",
      "enable_db": true,
      "access_token": "",
      "relogin": {
        "enabled": true,
        "relogin_delay": 3,
        "max_relogin_times": 0
      },
      "_rate_limit": {
        "enabled": false,
        "frequency": 1,
        "bucket_size": 1
      },
      "ignore_invalid_cqcode": false,
      "force_fragmented": false,
      "heartbeat_interval": 0,
      "http_config": {
        "enabled": false,
        "host": "127.0.0.1",
        "port": 8080,
        "timeout": 0,
        "post_urls": {}
      },
      "ws_config": {
        "enabled": false,
        "host": "127.0.0.1",
        "port": 8080
      },
      "ws_reverse_servers": [
        {
          "enabled": true,
          "reverse_url": "【WebSocket监听地址】 例如 ws://127.0.0.1:9200",
          "reverse_api_url": "",
          "reverse_event_url": "",
          "reverse_reconnect_interval": 3000
        }
      ],
      "post_message_format": "array",
      "use_sso_address": false,
      "debug": false,
      "log_level": "",
      "web_ui": {
        "enabled": true,
        "host": "127.0.0.1",
        "web_ui_port": 9999,
        "web_input": false
      }
    }
    ```
  * 可能会出现异地环境登陆或者失败，请多尝试几次
  * 打开`Socket`文件夹中`Qiushui.Lian.Bot.exe`运行，如出现闪退，请参考上文。
  * 找到自动生成的`config`文件夹中的配置文件`server_config.yaml`以及`config.yaml`，`config.yaml`文件默认存放在自动生成的当前登陆用户的文件夹中，例如[503745803]
  * 更改`server_config.yaml`配置，请注意此处的`Port`应与上文的`【WebSocket监听地址】`端口一致
  * 更改`config.yaml`配置，请根据自身需要更改，有`注释`
  * 注意如需使用人工智障对话，请配置`AiPath`的请求URL，同时启用`config.yaml`的`IsAi`
  * 人工智障使用，打开`AI`文件夹，安装`Python`环境，安装开源包`ChatterBot`，在当前文件夹中创建如下Python文件
    ``` Python
      from flask import Flask, render_template, request, jsonify
      from chatterbot import ChatBot

      app = Flask(__name__)

      bot = ChatBot(
           'Qiushui',
           database_uri='sqlite:///MainDb.sqlite3'
       )

      @app.route("/get")
      def get_bot_response():
          userText = request.args.get('msg')
          return str(bot.get_response(userText))

      @app.route("/api/chat/<text>")
      def get_bot_api(text):
          res = str(bot.get_response(text))
          return jsonify(res), 200


      if __name__ == "__main__":
          app.run(host='127.0.0.1', port=8889)
    ```
    * 在此打开Python命令，运行`Python xxx.py`<br />
    * `config.yaml`中的url路径则为`http://127.0.0.1:8889`
    * 如果需要映射到外网，通过Nginx转发，或者Utools内网穿透等等等
  * 请多看注释，然后再`Issue`
  * 可以参考我的博客图文使用攻略[点这](https://www.changqingmao.com)
* 如需通过源码方式，请直接下载当前源码，通过Nuget引入`Sora`，下方链接下载`go-cqhttp`，其它配置同上文
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
