# MajdataView&Edit
![Beta 1.3](https://img.shields.io/static/v1?label=Beta&message=1.3&color=546875)
[![State-of-the-art Shitcode](https://img.shields.io/static/v1?label=State-of-the-art&message=Shitcode&color=7B5804)](https://github.com/trekhleb/state-of-the-art-shitcode)

点击链接加入群聊【majdata工具交流反馈】：[607473320](https://jq.qq.com/?_wv=1027&k=TV6EGwC2)

## 更新内容
1. 重做tap动画
2. 加入星星判定条(just)
3. 感谢Moying-moe带来的谱面无理检查
4. 修复Edit issue #28
5. 修复45度旋转星星不正确的问题
6. 修复View无法加载封面或贴图的问题
7. 修复正常的直线星星报错的问题

## 基本操作
1. 用鼠标滚轮或鼠标拖动调整时间轴
2. 按住Ctrl点击文本调整时间轴
3. 写谱
4. 双击窗口空白处可唤起Viewer，
   且自动与Editor对齐
5. 点击发送到查看器开始播放预览
6. bg.mp4/bg.wmv/bg.jpg/bg.png可设为背景

## 快捷键列表
把焦点放在文本框上这些快捷键才能起作用。
|快捷键|功能|备注|
|--|--|--|
|Ctrl+左键|选定进度| |
|Ctrl+S|保存| |
|Ctrl+Z|撤销| |
|Ctrl+Shift+C|播放/停止|适用于在写谱时重复听一个段落的情况|
|Ctrl+Shift+X|播放/暂停|适用于总览谱面的情况|
|Ctrl+Shift+Z|发送到编辑器| |
|Ctrl+p|加快播放速度| |
|Ctrl+o|减缓播放速度|对音用|

要更改快捷键的话，请编辑EditorSetting.json

有关键位名称请参阅 [这里](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.input.key)

## Q&A

### 为什么软件无法启动？
- 请确认您安装了.net Framework 4.7.2
- 如果打开报错与PresentationCore有关，请尝试强制软件渲染

### 为什么我的谱面无法打开？
- 本软件只读取&title，&artist，&des，&first，&lv_x和&inote_x字段，请删掉其他字段和空字段重试。
- 如果报错与ReadWaveFromFile有关，请确认您的mp3文件为固定比特率。

### 延迟很大/音乐谱面对不上。
- 请尝试更改您的时区
- 请不要在谱面中长时间使用一种拍号
- 请确保您的mp3文件为标准比特率。具体请看 [这里](https://github.com/LingFeng-bbben/MajdataEdit/issues/26)

### 我的封面无法加载
- 请确认您的封面比例为1：1。推荐的分辨率是1080x1080

### 我的背景视频被拉伸
- 请使用视频剪辑软件调整您的视频为1:1。推荐的分辨率是1080x1080

### 我想全屏View/我想查看谱面物量
- 右上角有隐藏的全屏按钮，鼠标悬浮即可显示
- 拉宽窗口即可看到物量显示信息

### 为什么该重合的星星没有重合？
- 因为所有slide都是我手摆的

### 我对xxx音效不满意
- SFX文件夹内的wav自个儿换

### 我对xxx外观不满意
- Skin文件夹内的png自个儿画

### 能写Phixxos的谱吗
- 操你妈 滚

### 我是第一次写maimai谱面，该怎么做?
1. 请先选好歌，剪辑好长度后新建文件夹，并把歌曲重命名为track.mp3
2. 请点击新建，然后在编辑->一般谱面信息中，设置好谱面信息
3. 请设置背景，将（bg.mp4/bg.wmv/bg.jpg/bg.png）文件存入和track.mp3一个文件夹内
4. 请打开SimaiWiki（日语），熟悉各种note的摆放方式和语法
5. 请使用工具中的BPMtap或其他软件测试好歌曲bpm
6. 请使用偏移来对准小节线

## 已知问题
1. 重叠touch问题
2. touch无法调整速度
3. **不支持动态比特率的mp3文件**
4. 潜在的性能问题(hold释放音)

## Skin文件夹需要的纹理列表

*动动手 动动脑 你一定能找到你的需要*

|文件名|备注|
|--|--|
|outline.png
|tap.png
|tap_each.png
|tap_break.png
|tap_ex.png|ex是白色的外圈发光贴图|
|slide.png
|slide_each.png
|star.png|请确保旋转中心和图像中心重合|
|star_double.png
|star_each.png
|star_each_double.png
|star_break.png
|star_break_double.png
|star_ex.png
|star_ex_double.png
|hold.png|上下切图的像素距离为58|
|hold_each.png
|hold_ex.png
|just_curv.png|这三个是星星判定图 不需要可以直接删|
|just_str.png
|just_wifi.png
