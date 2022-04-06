# MajdataView&Edit
![Beta 3.0](https://img.shields.io/static/v1?label=Beta&message=2.4&color=546875)
[![State-of-the-art Shitcode](https://img.shields.io/static/v1?label=State-of-the-art&message=Shitcode&color=7B5804)](https://github.com/trekhleb/state-of-the-art-shitcode)

## Credits
- Main Programmer: bbben [>Twitter](https://twitter.com/bbben132329)
- Muri Detector: [Moying-moe](https://github.com/Moying-moe/maimaiMuriDetector)
- Mirroring: [Wh1tyEnd](https://github.com/Wh1tyEnd)
- Hanabi Effect: 青山散人
- *Special thanks*: Simai developed by [Celeca](https://twitter.com/formiku39854)

## 语言切换方法/言語切り替え方法/How to change language
- 使用设置菜单
- 設定メニューをご利用して下さい
- Please use the setting menu.

## 基本操作
1. 用鼠标滚轮或鼠标拖动调整时间轴
2. 按住Ctrl点击文本调整时间轴
3. 写谱
4. 设置->重新对准编辑器窗口可以让view恢复默认位置
5. 点击播放可以预览，点击录制模式可以在预览前后增加开始动画和AP动画
6. bg.mp4/【mv.mp4】/bg.wmv/【bg.jpg】/bg.png可设为背景(带括号的是maipad对应)
7. 可以更换皮肤（Skin文件夹）和音效（SFX文件夹）
8. 可以更改字号（EditorSetting.json）
9. 左边的几个参数栏可以用鼠标滚轮微调值
10. 谱面信息窗口里可以设置封面和其他&参数
11. View右上角有隐藏按钮，可以切换combo显示和全屏等
12. 在谱面信息>Other Commands里面可以设置歌曲开始前的guide音: &clock_count=4
13. 已对应maipad变速语法 <HS*1.0>

## 快捷键列表
把**焦点放在文本框**上这些快捷键才能起作用。
|快捷键|功能|备注|
|--|--|--|
|Ctrl+左键|选定进度| |
|Ctrl+S|保存| |
|Ctrl+Z|撤销| |
|Ctrl+Shift+C|播放/停止|适用于在写谱时重复听一个段落的情况|
|Ctrl+Shift+X|播放/暂停|适用于总览谱面的情况|
|Ctrl+Shift+Z|录制模式| |
|Ctrl+p|加快播放速度| |
|Ctrl+o|减缓播放速度|对音用|

要更改快捷键的话，请编辑EditorSetting.json

有关键位名称请参阅 [这里](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.input.key)

## Q&A

### 为什么软件无法启动？
- 请确认您安装了.net Framework 4.7.2
- 如果打开报错与PresentationCore有关，请尝试强制软件渲染

### 为什么我的谱面无法打开？（很长很吓人的报错）
- 本软件只读取&title，&artist，&des，&first，&lv_x和&inote_x字段，请删掉其他字段和空字段重试。
- 如果报错与ReadWaveFromFile有关，请确认您的mp3文件为固定比特率。

### 延迟很大/音乐谱面对不上。
- 请尝试更改您的时区
- 请不要在谱面中长时间使用一种拍号
- 请确保您的mp3文件为标准比特率。具体请看 [这里](https://github.com/LingFeng-bbben/MajdataEdit/issues/26)

### 我的封面无法加载
- 请确认您的封面比例为1：1。推荐的分辨率是1080x1080

### 我想全屏View/我想查看谱面物量
- 右上角有隐藏的全屏按钮，鼠标悬浮即可显示
- 还有一键拓宽按钮

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
|just_curv_l.png|这六个是星星判定图 不需要可以直接删|
|just_curv_r.png
|just_str_l.png
|just_str_r.png
|just_wifi_u.png
|just_wifi_d.png
