# 深空废站：最后撤离（Deep Space Station: Last Evacuation）

一个 Unity 第一人称科幻生存撤离小游戏。玩家在废弃空间站中修复关键系统，在氧气和倒计时压力下打开撤离通道并逃离。

## 如何运行

1. 用 Unity Hub 打开本文件夹。
2. 推荐 Unity 版本：`2022.3 LTS`。
3. 第一次打开后等待 Unity 导入项目。
4. 在顶部菜单选择：`Deep Space Station -> Build Playable Scene`。
5. 打开生成的场景：`Assets/Scenes/Station_A.unity`。
6. 点击 Play 运行。

更详细步骤见：`UNITY_RUN.md`。

## 控制说明

- `WASD`：移动
- `Mouse`：视角
- `Left Shift`：冲刺
- `E`：交互
- `Esc`：释放鼠标
- `R`：胜利或失败后重开

## 当前完成内容

- 第一人称移动和视角控制
- 射线交互系统
- 氧气持续消耗、低氧失败、氧气罐补给
- 倒计时失败条件
- 三个维修终端
- 维修完成后解锁撤离门
- 辐射危险区加速耗氧
- 撤离舱胜利条件
- HUD 和胜负界面
- Unity Editor 一键生成可玩场景工具
