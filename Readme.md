# 俄罗斯方块-Unity

本项目的初衷是练习帧同步服务器的实现，选择的客户端是俄罗斯方块游戏。

整体设计遵循esc原则

对应服务器为：

[GitHub - riyuejiuzhao/Unity-Tetirs-Svr: unity俄罗斯方块服务器](https://github.com/riyuejiuzhao/Unity-Tetirs-Svr.git)

对应协议为：

[GitHub - riyuejiuzhao/Unity-Tetris-Proto: 俄罗斯方块的协议层](https://github.com/riyuejiuzhao/Unity-Tetris-Proto.git)

## 代码解释

Entry.unity是初始界面，用于连接服务器和开始游戏，该界面主要有下面几个类：

- EntryUI，实现UI功能

- Net为网络连接类，负责实现KCP的发送接收，在整个游戏中只有一个

- PlayerInfo为玩家信息类，在整个游戏中只有一个，保存了玩家ID，房间信息

NetGame.unity是多人游戏界面，它主要由以下几个类组成：

- GameWorld，游戏中只有一个，负责处理玩家输入，保存同步来的报文，处理收到的报文。

- BlockMap，目前游戏里有两个， 一个是玩家自己的在左侧，另一个是对手的在右侧，它们会从GameWorld中抽取自身的最新的指令，并进行同步播放
