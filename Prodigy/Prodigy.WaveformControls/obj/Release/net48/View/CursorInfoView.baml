   M S B A M L   `   `   `  ����   Prodigy.WaveformControls2    ,Prodigy.WaveformControls.View.CursorInfoView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   $��	cusorInfo��.���   � ColumnDefinitions  �� $
��Auto ��� $
��Auto ��� $
��Auto � � RowDefinitions �� .+���  �� Orientation$ 
Horizontal=�$��0��.+��� $��0��$��Roboto)�$��12(�$��SemiBold"�$��Center=�$��0��$��Center=� �� ContentStringFormat$  {0} =��$��0q����� *Cursor1LabelForeColor+ �� ElementName$ 	cusorInfo������ *Cursor1Label+$ 	cusorInfo���� $��1��$��Roboto)�$��12(�$��SemiBold"�$��Center=�$��5,0,0,0q�$��Center=�$��0��$��0q����� *Cursor1LabelForeColor+$ 	cusorInfo�� � DataContext ��  �� RelativeSource �� *FindAncestor+ �� AncestorType UserControl����  �� 
TargetType" ��	 �� Triggers	 �� 
 �� Binding
 ��  �� Path$ 
CursorType� �� Value	 V_Bar.a���  �� Property � �� �� Value `�    doubleToTimeConverter `� 	Converter# [  .1��� $ Cursor1Position��� $ TriggerPosition��� $ 
SampleRate��� 
 �� $ 
CursorType�	 H_Bar.a���  � �� �� $ 
Horizontal=�.+��� ���� *Cursor1Position+  doubleToNumberConverter �� 	Converter# [ �� ���� *
HUnitLabel+�� $ 
Horizontal=�$��1��.+��� $��0��$��Center=�$��Roboto)�$��12(�$��SemiBold"�$��Center=�$��1��$  {0} =��$��0q����� *Cursor2LabelForeColor+$ 	cusorInfo������ *Cursor2Label+$ 	cusorInfo���� $��1��$��1��$��5,0,0,0q�$��Center=�$��Roboto)�$��12(�$��SemiBold"�$��Center=�$��0q����� *Cursor2LabelForeColor+$ 	cusorInfo�� ��  �� *FindAncestor+ UserControl���� " ��	 �� 
 �� $ 
CursorType�	 V_Bar.a���  � �� `� # [  .1��� $ Cursor2Position��� $ TriggerPosition��� $ 
SampleRate��� 
 �� $ 
CursorType�	 H_Bar.a���  � �� �� $ 
Horizontal=�.+��� ���� *Cursor2Position+# [ �� ���� *
HUnitLabel+�� $ 
Horizontal=�$��2��.+��� $��0��$��Center=�$��2��$��Roboto)�$��12(�$��SemiBold"�$��Center=�$  {0} =��$��0q����� *CursorDeltaLabelForeColor+$ 	cusorInfo������ *CursorDeltaLabel+$ 	cusorInfo���� $��1��$��2��$��Roboto)�$��12(�$��SemiBold"�$��Center=�$��5,0,0,0q�$��Center=�$��0q����� *CursorDeltaLabelForeColor+$ 	cusorInfo�� ��  �� *FindAncestor+ UserControl���� " ��	 �� 
 �� $ 
CursorType�	 V_Bar.a���  � �� `� # [  .1��� $ CursorDelta��� $ 
SampleRate��� 
 �� $ 
CursorType�	 H_Bar.a���  � �� �� $ 
Horizontal=�.+��� ���� *CursorDelta+# [ �� ���� *
HUnitLabel+