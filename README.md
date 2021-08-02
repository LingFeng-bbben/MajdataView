# MajdataView&Edit
Alpha6.2 [![State-of-the-art Shitcode](https://img.shields.io/static/v1?label=State-of-the-art&message=Shitcode&color=7B5804)](https://github.com/trekhleb/state-of-the-art-shitcode)

## 更新内容
1. 调音台，更改各个声音的音量大小(设置->调音台)
2. 保存各谱面的编辑位置，速度设置等(majSetting.json)
3. 修正Touch镜像问题（感谢小e）
4. 修正comboBox难度名称展示

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
***
**注意：A5.1起快捷键已改变**
***

## 已知问题
1. 一键镜像对Touch会发生错位
2. 重叠touch问题
3. touch无法调整速度
4. 不支持动态比特率的mp3文件
5. 高DPI显示问题
6. 潜在的性能问题
