   M S B A M L   `   `   `  ���� W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   WindowColor  ContentAreaColorLight  ContentAreaColorDark  DisabledControlLightColor  DisabledControlDarkColor  DisabledForegroundColor  SelectedBackgroundColor  SelectedUnfocusedColor  ControlLightColor 	 ControlMediumColor 
 ControlDarkColor  ControlMouseOverColor  ControlPressedColor  
GlyphColor  GlyphMouseOver  BorderLightColor  BorderMediumColor  BorderDarkColor  PressedBorderLightColor  PressedBorderDarkColor  DisabledBorderLightColor  DisabledBorderDarkColor  DefaultBorderBrushDarkColor  HeaderTopColor " DatagridCurrentCellBorderColor  SliderTrackDarkColor  NavButtonFrameColor  MenuPopupBrush �� EndPoint �� 
StartPoint � Color � Offset $  ProgressBarIndicatorAnimatedFill  ScrollBarLineButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name �� CornerRadius'   !System.Windows.VisualStateManager    VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� 
TargetName �� TargetProperty8   2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value F�Name �� RelativeSource �� Color  ScrollBarPageButton  ScrollBarThumb   VerticalScrollBar � RowDefinitions ]�Name ]� DecreaseRepeatButton ]� Thumb ]� IncreaseRepeatButton ! HorizontalScrollBar E� IsClosed E� 
StartPoint �� Point " customscrollbar �� Triggers P� Property  P� Value # ScrollThumbs! �Name" �� Triggers# �� 
TargetName�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml        �� $X Rpack://application:,,,/ProdigyFramework;component/ResourceDictionary/Resource.xaml?�%�  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 r     &	 �     &	 �     &		 �     &	
 �     &	 �     &	 �     &	 �     &	     &	 ;    &	 N    &	 a    &	 t    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	     &	     &	 u    &	     7  7  &	 K    &	     &	  �    7  7  7  7  7  &	! D
    7  7  7  &	" �    7 ! 7   &	# �    ��	#FFE8EDF9����	#FFC5CBF9����	#FF7381F9����	#FFE8EDF9����	#FFC5CBF9����	#FF888888����	#FFC5CBF9����	#FFDDDDDD����	White����	#FF7381F9����	#FF211AA9����	#FF3843C4����	#FF211AA9����	#FF444444����/+sc#1, 0.004391443, 0.002428215, 0.242281124����	#FFCCCCCC����	#FF888888����	#FF444444����	#FF888888����	#FF444444����	#FFAAAAAA����	#FF888888����	Black����	#FFC5CBF9����	Black����	#FFC5CBF9����	#FF3843C4���� $ 0.5,1&�$ 0.5,0&�.��� # �  $ 0Q�� # � 	 $	 0.5Q�� # �  $ 1Q��� $	 0,0&�$	 1,0&���� � $ 	#000000FF��$ 0Q�� $ 	#600000FF��$	 0.4Q�� $ 	#600000FF��$	 0.6Q�� $ 	#000000FF��$ 1Q��� " ��.���  �@J�SnapsToDevicePixels$
 True����  �@�OverridesDefaultStyle$
 true����  � |�$ false����  � �� �� "	 ��.R��� $
 Border��$��1q�$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $	 0.0Q�� # �  $	 1.0Q����� $	 0,0&�$	 0,1&���� � # �  � # � 	 $	 1.0Q�   CommonStates��.   
Normal��  	MouseOver��  Pressed��. �� .���� $ Border��$c ](Panel.Background).                     (GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $ 0��8     Disabled��. �� .���� $ Arrow��$? 9(Shape.Fill).                     (SolidColorBrush.Color)�.g�  $ 0��8  ._�F� $ Arrow��$��Center=�$��Center=����� *	Content+ �� *TemplatedParent+���� # �  �� " ��.���  �@J�SnapsToDevicePixels$
 True����  �@�OverridesDefaultStyle$
 true����  � ��$ false����  � |�$ false����  � �� �� "	 ��.R��� 
������ �� " n�.���  �@J�SnapsToDevicePixels$
 True����  �@�OverridesDefaultStyle$
 true����  � ��$ false����  � |�$ false����  � �� �� "	 n�.R��� $ 2��#��z��#��z��$��1q��� "	 ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3��$ 2�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $ 
PART_Track��$��1��$
��true�� �� 8�� $��ScrollBar.PageUpCommand�� n� 8�� $��1,0,1,0q����� $	 0,0&�$	 1,0&���� � # �  $	 0.0Q�� # �  $	 1.0Q����� $	 0,0&�$	 1,0&���� � # �  $	 0.0Q�� # � 	 $	 1.0Q� �� 8�� $��ScrollBar.PageDownCommand���� $��2��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� "	 ��.R�� .+��� $��3��$��1q�
���   �
������F� $��1��$��Stretch=�$��20������ " F�.���  �@�OverridesDefaultStyle$
 True����  � ��
 �   ���  � ��$ 1����  �@��Stretch$
 Fill=���  �@��StrokeDashArray$ 1R���  � �� B� ��D� E� $ false��$	 0,0&���@� �� $ 0,300&�E� $ false��$
 30,0&���@� �� $ 30,300&�E� $ false��$
 60,0&���@� �� $ 60,300&�E� $ false��$
 90,0&���@� �� $ 90,300&�E� $ false��$ 120,0&���@� �� $ 120,300&�E� $ false��$ 150,0&���@� �� $ 150,300&�E� $ false��$ 180,0&���@� �� $ 180,300&�E� $ false��$ 210,0&���@� �� $ 210,300&�E� $ false��$ 240,0&���@� �� $ 240,300&�E� $ false��$ 270,0&���@� �� $ 270,300&�E� $ false��$ 300,0&���@� �� $ 300,300&�]� $ 
PART_Track��$��1��$��False�� �� 8��  $��ScrollBar.PageLeftCommand�� n� 8�� $��0,1,0,1q�
���   �
������ �� 8�� $ ��ScrollBar.PageRightCommand���� " ��.���  �@J�SnapsToDevicePixels$
 True����  �@�OverridesDefaultStyle$
 true�� P�  �@��Orientation$  
Horizontal=�. ���  � ��$
 Auto����  � ��$ 20����  � ��8   P�  �@��Orientation$  Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  �� " n�.���  � �� �� .R�� $
! Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ �� $
 
Rectangle1��$ 	10 0 0 10��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��#��z��" P�  �@�Tag$  
Horizontal��. ��� $# 
Rectangle1�� � ��$
 Auto���� $# 
Rectangle1�� � ��$ 7��