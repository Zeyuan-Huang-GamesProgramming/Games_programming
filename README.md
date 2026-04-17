# 深空废站：最后撤离（Deep Space Station: Last Evacuation）

一个 Unity 第一人称科幻生存撤离小游戏。玩家在废弃空间站中修复关键系统，在氧气和倒计时压力下打开撤离通道并逃离。

## 当前完成内容

- Unity 工程基础结构：`Assets/`、`Packages/`、`ProjectSettings/`
- 第一人称移动和视角控制
- 射线交互系统，按 `E` 与终端、门、氧气罐、撤离舱互动
- 氧气系统：持续消耗、低氧失败、氧气罐补给
- 倒计时系统：撤离窗口关闭后失败
- 三个维修终端：`Life Support`、`Navigation`、`Reactor`
- 门禁系统：普通门可打开，撤离门需要完成全部维修后解锁
- 危险区：辐射泄漏会加速氧气消耗
- 撤离舱胜利条件
- HUD：氧气、剩余时间、维修进度、目标提示、交互提示、胜负界面
- Unity Editor 一键生成可玩场景工具

## 如何运行

1. 用 Unity Hub 打开仓库根目录。
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

## 游戏目标

1. 找到并修复三个系统终端。
2. 避开或快速穿过辐射泄漏区域。
3. 必要时拾取氧气罐补给。
4. 全部系统恢复后进入撤离舱。

## 项目结构

```text
Assets/
  Editor/
    DeepSpaceStationSceneBuilder.cs
  Scripts/
    Core/
    Player/
    UI/
    World/
Packages/
ProjectSettings/
UNITY_RUN.md
```

## 开发文档

- 实操开发指南：`深空废站_最后撤离_开发指南.md`
- 评分对照与补全计划：`课程评分对照与补全计划.md`
- Unity 运行说明：`UNITY_RUN.md`

## 外部资源与版权声明

当前版本使用 Unity 基础几何体、内置 UI 字体和代码生成材质，没有引入第三方模型、贴图、字体或音频。

| 资源名称 | 来源 | 许可证 | 用途 |
|---|---|---|---|
| Unity Primitive Meshes | Unity Engine | Unity 默认项目资源 | 场景几何体 |
| Arial built-in font reference | Unity built-in resource | Unity 默认项目资源 | HUD 文本 |

## 测试与调试记录

- 本分支已做静态文件检查：确认 12 个 C# 文件括号结构平衡。
- 由于当前执行环境没有 Unity Editor，尚未在本机完成 Unity 导入和 Play Mode 测试。
- 下一步建议：在 Unity 2022.3 LTS 中打开项目，运行 `Deep Space Station -> Build Playable Scene`，进入 Play Mode 验证完整流程。

## GitHub 使用规范（课程要求）

- 每周至少 3 次 commit，commit 信息要说明“做了什么、为什么做”。
- 功能分支开发：`feature/...`、`fix/...`。
- 每个里程碑提 Pull Request，PR 内写：
  - 目标
  - 改动清单
  - 测试证据（截图/视频/日志）
  - 已知问题
- 避免“最后一天一次性大提交”。
