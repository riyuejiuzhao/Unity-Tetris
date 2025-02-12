# 俄罗斯方块-Unity

本项目的初衷是练习帧同步服务器的实现，选择的客户端是俄罗斯方块游戏。

## Unity使用grpc

一般而言Unity是用不了grpc的，因为Unity不支持Http2，所以需要引入一个新的包:

[Cysharp/YetAnotherHttpHandler：YetAnotherHttpHandler 将 HTTP/2（和 gRPC）的强大功能引入 Unity 和 .NET Standard。](https://github.com/Cysharp/YetAnotherHttpHandler)

然后记得设置包的参数：

```csharp
var handler = new YetAnotherHttpHandler();
handler.Http2Only = true;
channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions() { HttpHandler = handler });
client = new TetrisService.TetrisServiceClient(channel);
```

## 后端

[GitHub - riyuejiuzhao/Unity-Tetirs-Svr: unity俄罗斯方块服务器](https://github.com/riyuejiuzhao/Unity-Tetirs-Svr.git)

## 协议

[GitHub - riyuejiuzhao/Unity-Tetris-Proto: 俄罗斯方块的协议层](https://github.com/riyuejiuzhao/Unity-Tetris-Proto.git)


