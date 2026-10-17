# TPDP Battle Helper — 汉化版支持分支（cn-fixed）

本分支基于上游 `amobiusv/TPDP-Battle-Helper-C-` 的 `master`（`e1b77ba`），
让工具适配《幻想人形演舞 -梦之碎片-》汉化版，并修复若干原有 bug。

## 声明

本分支内的代码改动主要由 AI 在人工指导下完成修改与重构，并非全部由人类手写。
所有改动均基于原仓库源码，涉及窗口标题匹配、自动重连 / 游戏退出、切换人形后主面板
实时刷新、技能伤害倍率实时读取等。请在使用前自行审阅代码，并自担风险；如有问题欢迎
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
   改为实时读取我方 / 敌方内存（`0xC59FDB` / `0xC5A510`），
   仅在原始值变化时重绘，修复原源码切换人形后数据不更新的 bug。

4. **技能伤害倍率错乱**（`Views/PageBattleMove.xaml.cs`）
   改为每帧实时读取我方 / 敌方 / 4 技能，倍率按「技能变化或敌方变化」重算，
   不再显示上一只人形的技能倍率。

## 运行环境

- .NET 8 桌面运行时（.NET 8 Desktop Runtime）

## 使用方法

1. 先启动汉化版游戏《幻想人形演舞 -梦之碎片-》并进入战斗；
2. 再启动本工具（TPDP Battle Helper）；
3. 工具自动 hook 到游戏窗口，战斗信息实时叠加显示；
4. 关闭游戏后，工具会自动退出（无需手动关闭）。

## 致谢

- 原项目：`amobiusv/TPDP-Battle-Helper-C-`
- 汉化版游戏：见上方 B 站发布地址
