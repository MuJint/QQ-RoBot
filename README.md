# 简介
Qiushui-Bot是一个基于onebot标准的机器人/当然目前只写了部分我常用的功能，框架已写好，需要添加指令请自行根据相应逻辑添加。<br />
请注意：是基于`.Net5`所写<br />
默认数据库存档是：`LiteDb`

# 功能概览
* 来点色图
* 人工智障对话
* 莲Bot（基于天刀整容交流团一系列命令）
* 其它娱乐功能

# 平台兼容性
><b>注意</b><br />
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

# 常用指令
  <details>
  <summary>指令</summary>

  | 指令        | 功能                        |
  | ------------ | --------------------------- |
  | [签到]    | [当天签到赠送积分]                   |
  | [查询]  | [查询积分]                      |
  | [优质睡眠 昏睡红茶 昏睡套餐 健康睡眠]   | [被禁言8小时]                    |
  | [早安 晚安]      | [某时间段进行]                     |
  | [莲]   | [随机一句话]                  |
  | [分来]   | [几率送分] |
  | [排行榜]   | [统计积分排行榜]                      |
  | [特殊事件] | [特殊时间产生的记录]                  |
  | [技能 菜单 功能]    | [呼出菜单]              |
  | [抽奖]     | [可能产生送分或者禁言]                  |
  | [打劫]    | [可能产生送分或者禁言]                 |
  | [救援 劫狱]    | [救出被禁言的对象，可能被禁言]                 |
  | [赠送]    | [赠送某个对象积分]                 |
  | [加分 扣分 全体加分 全体扣分]    | [针对某人或全体成员加减积分]                 |
  | [积分记录 个人积分]    | [个人积分记录详情]                 |
  | [添加数据密码 添加词库]    | [添加一条数据密码或添加一条随机词库]                 |
  | [骰子 扔骰子 掷骰子 色子]    | [骰子]                 |
  | [词云]    | [生成个人发言特点图片]                 |
  | [发言榜]    | [当前群聊的发言次数榜单]                 |
  | [来点色图 来点涩图 我要看色图 色图 涩图]    | [发送一张H图，非R18]                 |

  </details>

<br>

# 使用
* 如果只需要运行在本地，不需要源代码，请前往[Release](https://github.com/MuJint/Qiushui-Bot/releases)下载最新包
* 分为两个文件，一个是`client`一个是`server`目前仅打包了`64`位操作系统
* 请注意`server`端是`go-cqhttp`归原作者所有，可前往[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)自行下载
* 默认配置文件如下，仅需要填入账号以及密码
  ``` Yaml
  # go-cqhttp 默认配置文件
      account: # 账号相关
        uin: 123456 # QQ账号
        password: '' # 密码为空时使用扫码登录
        encrypt: false  # 是否开启密码加密
        status: 0      # 在线状态 请参考 https://docs.go-cqhttp.org/guide/config.html#在线状态
        relogin: # 重连设置
          delay: 3   # 首次重连延迟, 单位秒
          interval: 3   # 重连间隔
          max-times: 0  # 最大重连次数, 0为无限制

        # 是否使用服务器下发的新地址进行重连
        # 注意, 此设置可能导致在海外服务器上连接情况更差
        use-sso-address: true

      heartbeat:
        # 心跳频率, 单位秒
        # -1 为关闭心跳
        interval: 5

      message:
        # 上报数据类型
        # 可选: string,array
        post-format: array
        # 是否忽略无效的CQ码, 如果为假将原样发送
        ignore-invalid-cqcode: false
        # 是否强制分片发送消息
        # 分片发送将会带来更快的速度
        # 但是兼容性会有些问题
        force-fragment: false
        # 是否将url分片发送
        fix-url: false
        # 下载图片等请求网络代理
        proxy-rewrite: ''
        # 是否上报自身消息
        report-self-message: false
        # 移除服务端的Reply附带的At
        remove-reply-at: false
        # 为Reply附加更多信息
        extra-reply-data: false
        # 跳过 Mime 扫描, 忽略错误数据
        skip-mime-scan: false

      output:
        # 日志等级 trace,debug,info,warn,error
        log-level: warn
        # 日志时效 单位天. 超过这个时间之前的日志将会被自动删除. 设置为 0 表示永久保留.
        log-aging: 15
        # 是否在每次启动时强制创建全新的文件储存日志. 为 false 的情况下将会在上次启动时创建的日志文件续写
        log-force-new: true
        # 是否启用 DEBUG
        debug: false # 开启调试模式

      # 默认中间件锚点
      default-middlewares: &default
        # 访问密钥, 强烈推荐在公网的服务器设置
        access-token: ''
        # 事件过滤器文件目录
        filter: ''
        # API限速设置
        # 该设置为全局生效
        # 原 cqhttp 虽然启用了 rate_limit 后缀, 但是基本没插件适配
        # 目前该限速设置为令牌桶算法, 请参考:
        # https://baike.baidu.com/item/%E4%BB%A4%E7%89%8C%E6%A1%B6%E7%AE%97%E6%B3%95/6597000?fr=aladdin
        rate-limit:
          enabled: false # 是否启用限速
          frequency: 1  # 令牌回复频率, 单位秒
          bucket: 1     # 令牌桶大小

      database: # 数据库相关设置
        leveldb:
          # 是否启用内置leveldb数据库
          # 启用将会增加10-20MB的内存占用和一定的磁盘空间
          # 关闭将无法使用 撤回 回复 get_msg 等上下文相关功能
          enable: true

      # 连接服务列表
      servers:
        # 添加方式，同一连接方式可添加多个，具体配置说明请查看文档
        #- http: # http 通信
        #- ws:   # 正向 Websocket
        #- ws-reverse: # 反向 Websocket
        #- pprof: #性能分析服务器
        # 反向WS设置
        - ws-reverse:
            # 反向WS Universal 地址
            # 注意 设置了此项地址后下面两项将会被忽略
            universal: ws://127.0.0.1:9200
            # 反向WS API 地址
            api: ws://your_websocket_api.server
            # 反向WS Event 地址
            event: ws://your_websocket_event.server
            # 重连间隔 单位毫秒
            reconnect-interval: 3000
            middlewares:
              <<: *default # 引用默认中间件
  ```
* 可能会出现异地环境登陆或者失败，请多尝试几次
* 打开`Client`文件夹中`Qiushui.Bot.exe`运行，如出现闪退，请参考上文。
* 找到自动生成的`config`文件夹中的配置文件`server_config.yaml`以及`config.yaml`，`config.yaml`文件默认存放在自动生成的当前登陆用户的文件夹中，例如[123456]
* 更改`server_config.yaml`配置，请注意此处的`Port`应与上文的`【WebSocket监听地址】`端口一致
* 更改`config.yaml`配置，请根据自身需要更改，有`注释`
* 注意如需使用人工智障对话，请配置`AiPath`的请求URL，同时启用`config.yaml`的`IsAi`
* 人工智障使用，打开`AI`文件夹，安装`Python`环境，安装开源包`ChatterBot`，在当前文件夹中创建如下Python文件
  * 安装Python环境请自行百度，安装ChatterBot也请自行百度
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
* 可以参考我的博客图文使用攻略[点这](https://www.qiubb.com)
* 或者参考[`bilibili`](https://b23.tv/dAwA7S)
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
[JieBa.NET](https://github.com/anderscui/jieba.NET) `结巴中文分词.net库`<br />
[WordCloud](https://github.com/amueller/word_cloud) `词云`<br />
[.net实现wordcloud](https://github.com/AmmRage/WordCloudSharp) `.net词云`<br />
[ChatterBot](https://github.com/gunthercox/ChatterBot) `人工智障`开源库
