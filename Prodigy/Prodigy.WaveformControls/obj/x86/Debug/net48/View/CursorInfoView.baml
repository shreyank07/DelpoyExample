   M S B A M L   `   `   `  ����  Prodigy.WaveformControls2    ,Prodigy.WaveformControls.View.CursorInfoView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        $��	cusorInfo��5      .��5	      �   � ColumnDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � RowDefinitions 5   
   �� 5      .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   .+�5      �� $��0��6   $��Roboto)�6!   $��12(�65   $��SemiBold"�6C   $��Center=�6Y   $��0��5      $��Center=�6     �� ContentStringFormat$  {0} =��5   G   $��0q�5      ��5   }   �� *Cursor1LabelForeColor+ �� ElementName$ 	cusorInfo����5      �� *Cursor1Label+$ 	cusorInfo��5      �� 5      $��1��6   $��Roboto)�6!   $��12(�65   $��SemiBold"�6C   $��Center=�6Y   $��5,0,0,0q�5      $��Center=�6!   $��0��5      $��0q�5      ��5   }   �� *Cursor1LabelForeColor+$ 	cusorInfo�� � DataContext 5      ��  �� RelativeSource �� *FindAncestor+ �� AncestorType UserControl��5      �� 5       �� 
TargetType" ��6   	 �� Triggers	 5      �� 5       
 �� Binding
 6*   ��  �� Path$ 
CursorType� �� Value	 V_Bar6N   .a�5!   "   ��  �� Property � �� �� Value 5"   &   `� 5#   *      doubleToTimeConverter `� 	Converter# [  5$   )   .1�5%   .   �� $ Cursor1Position�66   �� 5&   .   $ TriggerPosition�66   �� 5'   .   $ 
SampleRate�66   5(   +   5*   #   5+      �� 5,      
 6*   �� $ 
CursorType�	 H_Bar6N   .a�5-   "   ��  � �� 5.   &   �� 5/   *   $ 
Horizontal=�65   .+�50   .   �� ��68   �� *Cursor1Position+  doubleToNumberConverter �� 	Converter# [ �� 51   .   ��68   �� *
HUnitLabel+52   +   54   #   55      57      59      5:      �� 5;   
   $ 
Horizontal=�6   $��1��6.   .+�5<      �� $��0��6   $��Center=�6!   $��Roboto)�5=      $��12(�6#   $��SemiBold"�61   $��Center=�6G   $��1��5>      $  {0} =��5?   G   $��0q�5@      ��5=   k   �� *Cursor2LabelForeColor+$ 	cusorInfo����5?      �� *Cursor2Label+$ 	cusorInfo��5@      �� 5A      $��1��6   $��1��5B      $��5,0,0,0q�5C      $��Center=�6!   $��Roboto)�6<   $��12(�6P   $��SemiBold"�6^   $��Center=�6t   $��0q�5E      ��5C   �   �� *Cursor2LabelForeColor+$ 	cusorInfo�� 5D      ��  �� *FindAncestor+ UserControl��5F      �� 5G      " ��6   	 5H      �� 5I      
 6*   �� $ 
CursorType�	 V_Bar6N   .a�5J   "   ��  � �� 5K   &   `� 5L   *   # [  5M   )   .1�5N   .   �� $ Cursor2Position�66   �� 5O   .   $ TriggerPosition�66   �� 5P   .   $ 
SampleRate�66   5Q   +   5S   #   5T      �� 5U      
 6*   �� $ 
CursorType�	 H_Bar6N   .a�5V   "   ��  � �� 5W   &   �� 5X   *   $ 
Horizontal=�65   .+�5Y   .   �� ��68   �� *Cursor2Position+# [ �� 5Z   .   ��68   �� *
HUnitLabel+5[   +   5]   #   5^      5`      5b      5c      �� 5d   
   $ 
Horizontal=�6   $��2��6.   .+�5e      �� $��0��6   $��Center=�6!   $��2��5f      $��Roboto)�6    $��12(�64   $��SemiBold"�6B   $��Center=�6X   $  {0} =��5g   K   $��0q�5h      ��5f   |   �� *CursorDeltaLabelForeColor+$ 	cusorInfo����5g      �� *CursorDeltaLabel+$ 	cusorInfo��5h      �� 5i      $��1��6   $��2��5j      $��Roboto)�6    $��12(�64   $��SemiBold"�6B   $��Center=�6X   $��5,0,0,0q�5k      $��Center=�6!   $��0q�5m      ��5j   |   �� *CursorDeltaLabelForeColor+$ 	cusorInfo�� 5l      ��  �� *FindAncestor+ UserControl��5n      �� 5o      " ��6   	 5p      �� 5q      
 6*   �� $ 
CursorType�	 V_Bar6N   .a�5r   "   ��  � �� 5s   &   `� 5t   *   # [  5u   )   .1�5v   .   �� $ CursorDelta�66   �� 5w   .   $ 
SampleRate�66   5x   +   5z   #   5{      �� 5|      
 6*   �� $ 
CursorType�	 H_Bar6N   .a�5}   "   ��  � �� 5~   &   �� 5   *   $ 
Horizontal=�65   .+�5�   .   �� ��68   �� *CursorDelta+# [ �� 5�   .   ��68   �� *
HUnitLabel+5�   +   5�   #   5�      5�      5�      5�      5�      5�      