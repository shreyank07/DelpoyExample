   M S B A M L   `   `   `  ����W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   ComboBoxToggleButton �� 
TargetType � ColumnDefinitions ��Name J� SnapsToDevicePixels  BackgroundNormal  BorderBrushNormal F�Name J� Opacity  
Foreground �� Triggers	 P� Property
 P� Value �� 
TargetName �� Property �� Value  BorderBrushHighlighted  BackgroundSelected  ComboBoxTextBox  StandardComboBox �� 
TargetType �� 	ClickMode d� 	IsChecked �� Path �� Mode �� RelativeSource J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate ��Name �� 
CaretBrush �� 
IsReadOnly �� 
IsReadOnly � VerticalOffset �� IsDropDownOpen � AllowsTransparency � ActualWidth �� MaxDropDownHeight  K� IsItemsHost-   'System.Windows.Input.KeyboardNavigation!    DirectionalNavigation" �� 	Resources# �� Padding  BackgroundHighlighted�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml      5        5      �� 5   
   $X Rpack://application:,,,/ProdigyFramework;component/ResourceDictionary/Resource.xaml?�6   %,  &	        7  7  7  7  7  &	 �    &	 "    7  7   7  7  7  7  7  7  7  �� 5      " d�63   .R�5   
   �  5      �� 5	      �� 5
      $��20 �6#   .+�5      �� $ Border��60   $
 True��6   $��2��6@   8��  8�� $��1q�6�   F� 5      $ Arrow��6   $��1��6"   $	 0.6Q�62   8�� $��Center=�6c   $��Center=�6�   "����        � 	=  	= � z     	6�   5       5   
   P� 5      	 �@J�IsMouseOver$

 true��6:   . �5      �� $ Arrow��6    �@J�Opacity$ 1Q�6?   �� 5      $ Border��6   8   � ��6D   5      P� 5      	 �@d�	IsChecked$

 trueY�68   . �5      �� $ Arrow��6    �@J�Opacity$ 1Q�6?   �� 5      $ Arrow��6   8   � ��6<   5      5      �� 5      " ��6.   .R�5   
   �� $ PART_ContentHost��6   $|�False��6+   #��z��6=   5      �� 5      " ��6%   .�5    
   ��  � ��8   6'   �� 5!   
    �@J�SnapsToDevicePixels$
 true��60   �� 5"   
    �@�OverridesDefaultStyle$
 true��62   �� 5#   
    � ��$
 Auto=�6G   �� 5$   
    � ��$
 Auto=�6E   �� 5%   
    � ��$
 true��6:   �� 5&   
    � ��$ 50��6%   �� 5'   
    � ��$ 20��6&   �� 5(   
    � �� 5)      �� 5*      " ��6"   .R�5+      � .+�5,      d� $��ToggleButton��6'   8�� $��2��5.   '   $|�false��5/   '   $ Press=�51   '    50   '   �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+51   '   �� 52      $��ContentSite��6+   $ False��53   +   #��z 54   +   #��z 55   +   #��z��56   +   $��3,3,23,3q�57   +   $��Center=�58   +   $
��Left=�59   +   �� 5:      $ PART_EditableTextBox��6"   8  8�� $
��Left=�5>   "   $��Center=�5?   "   $��3,3,23,3q�5@   "   $
|�True��5A   "   
������ 8�� $y�Hidden=�5D   "   # z 5E   "   ��5<   "   Z� 5E   "   � 5F      $��Popup��6O   $ -1��6    $
 True��64   $��Bottom=�6\   #��z 6o   $
 True��6�   $|�False��6�   $��Slide=�6�   .��5G      � $��DropDown��6#   $
 True��63   #��z 6N   #��z 6w   .+�5H   "   �� $ DropDownBorder��6)   8�� $��1q�6p   8�� 6�   �� 5I   "   $
 True��6/   .��5J   &   �� $
  True��61   $! 	Contained=�6D   5K   #   5L      5M      5N       5O      P� 5P      	 �@��HasItems$
 false��66   . �5Q      �� $ DropDownBorder��6%    � ��$ 95��6V   5R      P� 5S      	 �@��
IsGrouping$

 true��68   . �5T      ��  � ��$ false��6N   5U      P� 5V      	 �@��
IsEditable$

 true��68   . �5W      ��  � ��$ false��6:   �� 5X      $ PART_EditableTextBox��6%    � y�$ Visible=�6]   �� 5Y      $ ContentSite��6%    � y�$ Hidden=�6T   5Z      5\      5^      " 5_   
   ���� 5`      '��       " ��6   .�5a      ��  �@J�SnapsToDevicePixels$
 true��68   �� 5b       �@�OverridesDefaultStyle$
 true��6:   �� 5c       � �� 5d      �� 5e      " ��6*   .R�5f      �� $��Border��6%   $# 2q�63   $
 true��6?   $��1q�6Z   ._�5g   "   �� 5h       5i      P� 5j   "   	 �@��IsHighlighted$

 true��6C   . �5k   &   �� $ Border��6-   8   � ��6W   �� 5l   &   $ Border��6-   8   � ��6X   5m   #   5o      5q      5r      5s      5t      5u      