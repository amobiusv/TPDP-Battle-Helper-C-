# TPDP Battle Helper — 汉化版支持分支（cn-fixed）

本分支基于上游 `amobiusv/TPDP-Battle-Helper-C-` 的 `master`（`e1b77ba`），
让工具适配《幻想人形演舞 -梦之碎片-》汉化版，并修复若干原有 bug。

## 支持的游戏版本

实测（2026-11）同时支持以下两个汉化版，无需改配置：

| 版本 | 主程序 | 窗口标题 |
|---|---|---|
| AI 汉化版 | `sod_extended.exe` | `幻想人形演舞 -梦之碎片- ver1.103 [xx.xfps]` |
| 4.0 人工汉化版 | `SoD-Extended-CN-Final.exe` | `TPDP Shard of Dreams ver1.103 (Translation Patch) [xx.xfps]` |

两个构建的数据段布局经实机校验完全一致（切人形内存差分唯一命中同一地址），
工具默认地址对两者通用；标题按前缀匹配，两条标题均已覆盖。

## 声明

本分支内的代码改动主要由 AI 在人工指导下完成修改与重构，并非全部由人类手写。
所有改动均基于原仓库源码，涉及窗口标题匹配、自动重连 / 游戏退出、切换人形后主面板
实时刷新、技能伤害倍率实时读取等。`Resources` 下的中文属性图标与换色倍率图标
为本仓库作者手工制作。请在使用前自行审阅代码，并自担风险；如有问题欢迎
在原仓库或本分支提出 issue。

## 汉化版来源

- 汉化版游戏发布地址：<https://www.bilibili.com/video/BV1NGJ86LEy5/?p=2&t=1&>
- 注意：汉化作者发布的是**直接可玩的汉化版游戏**（非补丁），本分支即针对该汉化版进行适配。

## 改动内容

1. **窗口标题匹配（汉化版适配）**
   在 `Config/AppSettings.toml` 的 `game_title` 中增加汉化版窗口标题
   `幻想人形演舞 -梦之碎片-`，使工具能正确 attach 到汉化版游戏进程
   （原版只认 `TPDP Shard of Dreams` / `TPDP v1.`）。

2. **自动重连 + 游戏关闭自动退出**（`App.xaml.cs`）
   新增 3 秒定时重连；检测到「之前已连接、现在进程消失」时调用
   `Application.Current.Shutdown()`，游戏关掉后工具自动退出。

3. **切换人形后主面板不刷新**（`Views/PageBattleMain.xaml.cs`）
   改为实时读取我方 / 敌方内存，仅在原始值变化时重绘，
   修复原源码切换人形后数据不更新的 bug。

4. **技能伤害倍率错乱**（`Views/PageBattleMove.xaml.cs`）
   改为每帧实时读取我方 / 敌方 / 4 技能，倍率按「技能变化或敌方变化」重算，
   不再显示上一只人形的技能倍率。

5. **内存地址可配置**（`GameHook.cs` + `Config/AppSettings.toml` 的 `[ADDRESSES]` 节）
   6 个内存地址（battle_flag / battle_context_menu / map_id / player_puppet /
   enemy_puppet / player_skills）从硬编码改为静态字段，可在配置里用
   十六进制字面量覆盖（如 `player_puppet = 0xC59FDB`）。缺省键 = 内置默认值，
   不写 `[ADDRESSES]` 节则行为与旧版完全一致。以后游戏出偏移漂移的新构建时，
   改配置即可，无需重新编译。

6. **中文资源图**（`Resources/`）
   属性图标（`Types/type_*.png`）改为中文，倍率空图标（`Effectiveness/blank_*.png`）
   换色，迷你地图缺省图（`UI/map_unknown.png`）更新——均为手工修改。

## 运行环境

- .NET 8 桌面运行时（.NET 8 Desktop Runtime）

## 使用方法

1. 先启动汉化版游戏（AI 版双击 `sod_extended.exe`，4.0 版双击
   `SoD-Extended-CN-Final.exe`）并进入战斗；
2. 再启动本工具（TPDP Battle Helper）；
3. 工具自动 hook 到游戏窗口，战斗信息实时叠加显示；
4. 关闭游戏后，工具会自动退出（无需手动关闭）。

## 内存地址（默认值，可在 `[ADDRESSES]` 覆盖）

| 键 | 地址 | 含义 |
|---|---|---|
| `battle_flag` | `0x93C0DF` | u8，0=非战斗，非 0=战斗中 |
| `battle_context_menu` | `0x93C164` | u8，2=技能菜单，10-19=指令菜单 |
| `map_id` | `0x9969BC` | u16 野外地图 id |
| `player_puppet` | `0xC59FDB` | u16 我方出战人形 id，+2 为样式字节 |
| `enemy_puppet` | `0xC5A510` | u16 敌方人形 id，+2 为样式字节 |
| `player_skills` | `0xC59FED` | 4 × u16 我方技能 id |

## 致谢

- 原项目：`amobiusv/TPDP-Battle-Helper-C-`
- 汉化版游戏：见上方 B 站发布地址

