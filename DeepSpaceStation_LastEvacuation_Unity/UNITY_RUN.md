# Unity 运行说明：Deep Space Station: Last Evacuation

## 1. 打开项目

1. 用 Unity Hub 打开仓库根目录。
2. 推荐 Unity 版本：`2022.3 LTS`。
3. 第一次打开时等待 Unity 导入 `Packages/manifest.json` 和 `Assets`。

## 2. 生成可玩场景

打开 Unity 后，在顶部菜单选择：

```text
Deep Space Station -> Build Playable Scene
```

Unity 会自动创建：

- `Assets/Scenes/Station_A.unity`
- 基础材质
- 玩家、摄像机和 HUD
- 三个维修终端
- 两道门
- 氧气补给
- 辐射危险区
- 撤离舱

## 3. 运行

1. 打开 `Assets/Scenes/Station_A.unity`。
2. 点击 Play。

## 4. 操作

- `WASD`：移动
- `Mouse`：视角
- `Left Shift`：冲刺
- `E`：交互
- `Esc`：释放鼠标
- `R`：胜利或失败后重开

## 5. 游戏目标

玩家需要在氧气耗尽和撤离窗口关闭前：

1. 修复 `Life Support`
2. 修复 `Navigation`
3. 修复 `Reactor`
4. 回到撤离舱并发射

## 6. 当前完成内容

- 第一人称控制器
- 氧气消耗和氧气罐补给
- 辐射区加速耗氧
- 射线交互系统
- 终端维修系统
- 维修完成后解锁撤离门
- 撤离成功和失败状态
- HUD：氧气、倒计时、维修进度、目标、提示和结算界面
