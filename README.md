# MajdataView&Edit
Alpha6.3 [![State-of-the-art Shitcode](https://img.shields.io/static/v1?label=State-of-the-art&message=Shitcode&color=7B5804)](https://github.com/trekhleb/state-of-the-art-shitcode)

点击链接加入群聊【majdata工具交流反馈】：[607473320](https://jq.qq.com/?_wv=1027&k=TV6EGwC2)

## 更新内容
1. 修复Edit issue#12
2. 自定义快捷键
3. 支持offset
4. 现在播放谱面View会一直显示，而开始画面
   只有点击录制模式才会出现

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
要更改快捷键的话，请编辑EditorSetting.json
如“Ctrl+Space”

## 已知问题
1. 一键镜像对Touch会发生错位
2. 重叠touch问题
3. touch无法调整速度
4. 不支持动态比特率的mp3文件
5. 高DPI显示问题
6. 潜在的性能问题
