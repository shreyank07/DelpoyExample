   M S B A M L   `   `   `  ���� W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   AnimatedExpanderButtonTemp �� 
TargetType ��Name �� Padding �� CornerRadius � ColumnDefinitions F�Name J� RenderTransformOrigin	 �� Angle
 ��Name �� Triggers P� Property P� Value N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions �� 
TargetName �� Property �� Value  StretchyExpanderTemp d�Name � OverridesDefaultStyle d� 	IsChecked �� Path �� RelativeSource � LayoutTransform �� ScaleY  	blueLabel �� 
TargetType  TextBoxStyle1 ��Name  J� SnapsToDevicePixels  blueTextBox  Headers  cmbStyle  ComboBoxToggleButton  ComboBoxTextBox 	 bluecomboStyle! �� 	ClickMode" �� Mode# J� IsHitTestVisible$ �� SelectionBoxItem% �� SelectionBoxItemTemplate& ��Name' �� 
IsReadOnly( �� 
IsReadOnly) �� IsDropDownOpen* � AllowsTransparency+ � ActualWidth, �� MaxDropDownHeight- K� IsItemsHost-   'System.Windows.Input.KeyboardNavigation.    DirectionalNavigation/ P� 
SourceName0 �� Triggers 
 WindowColor  ContentAreaColorLight  ContentAreaColorDark  DisabledControlLightColor  DisabledControlDarkColor  DisabledForegroundColor  SelectedBackgroundColor  SelectedUnfocusedColor  ControlLightColor  ControlMediumColor  ControlDarkColor  ControlMouseOverColor  ControlPressedColor  
GlyphColor  GlyphMouseOver  BorderLightColor  BorderMediumColor  BorderDarkColor  PressedBorderLightColor  PressedBorderDarkColor  DisabledBorderLightColor  DisabledBorderDarkColor   DefaultBorderBrushDarkColor ! HeaderTopColor "" DatagridCurrentCellBorderColor # SliderTrackDarkColor $ NavButtonFrameColor % MenuPopupBrush1 �� EndPoint2 �� 
StartPoint3 � Color4 � Offset $&  ProgressBarIndicatorAnimatedFill ' CircleCheckbox5 A�Name ( checkMarkStyle ) CheckBoxFocusVisual6 �� 
Background7 �� Bullet8 �� Color'  !System.Windows.VisualStateManager9   VisualStateGroups%  System.Windows.VisualStateGroup:   States   System.Windows.VisualState;   
Storyboard8   2System.Windows.Media.Animation.EasingColorKeyFrame< �� KeyTime= �� Value> R� KeyTime   System.Windows.Visibility?   Visible@ R� Value * toggleRadioButtonA �� Stretch + toggleButtonCheckMark !, toggleButtonIndeterminateMark - radiobuttonstyleB � Cursor . SwitchTypeToggleButtonC �� 	Resources / Key 0 ToggleButtonContentTextboxD �NameE �� ElementNameF � RowDefinitionsG ��Name 1 ToggleButtonTrueBackground 2 ToggleButtonFalseBackgroundH ��Name 3 OnContentControl 4 OffContentControlI �� Orientation 5 ToggleButtonTrueForeground 6 ToggleButtonFalseForeground�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml        �� $ ButtonStyles.xaml?��� $ GroupBoxStyles.xaml?��� $ MenuStyles.xaml?��� $ Brushes.xaml?��� $ ScrollBarStyles.xaml?�%m<  &	        &	 �    7   &	 �    &	 �    &	 �    &	 X    &	 �    &	 x	    &	 �
    &		 �
    7  7  '�� �    &	
 �    &	 �    &	     &	     &	 +    &	 >    &	 Q    &	 d    &	 w    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	     &	 -    &	 @    &	 S    &	 f    &	 y    &	 �    &	  �    &	! �    &	" �    &	# �    &	$ �    &	% �    &	& T    &	' �    &	(     7  7  7  7  &	* �    &	+ �$    &	, #*    &	- l/    &	. �0    7 0 7 0 7 0 7 1 7 2 7 1 7 3 7 4 7 5 7 2 7 6 7 5 7 1 �� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��$ 2,0,0,0��._�� $��Center=� �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��14��$��14��$��Center=�$��Center=�F� $ Arrow��$��0��"����@B `� � �D @KL � z `� 	���	#FF666666$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $	 0Q��� $
 HeaderContent��$��Center=�$��1��$��4,0,0,0q�$��Content�� P�  �@d�	IsChecked$
 TrueY� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)� 180$ 0:0:0.4D� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)� 0$ 0:0:0.4D�P�  �@J�IsMouseOver$
 true��. ��� $ Circle�� � �� �	#FF3C7FB1�� $ Arrow�� � �� �#222P�  � ��$
 true��. ��� $ Circle�� � �� �	#FF526C7B�� $ Circle�� � ��$	 1.5���� $ Arrow�� � �� �	#FF003366�� " 9�.R�]� .+�d� $ ExpanderButton��$��Center=�$	��Top=�8��  #��z��$
 True��$��1.5,0q� �� $ 
IsExpanded� �� *TemplatedParent+�� $
 ExpanderContent��$��Content��$��Bottom=� �� $ 0Q� P�  �@9�
IsExpanded$
 True�� �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)� 1$ 0:0:0.4D� �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)� 0$ 0:0:0.4D��� " ��.���  � ��$ 14(���  � ��$ Roboto)���  � ��$ 40����  � ��$
 Auto����  � �� �#29b6f6��  � ��$ 0 0 0 0q��� " ��.���  � �� �� " ��.R�� .+��� $ PART_ContentHost��#  z  $��2,2,0,0q��� ���	#FFBAB7B7$��0,0,1,1q�$��1,1,0,0q��� 
���   �$��1,1,0,0q�$
��Auto����  � ��$ Calibri)���  � ��$ 14(��� " ��.���  � ��$ 22(���  � ��
 ���� ��  � ��$ 0 0 0 2q���  � �� �#483883��  � ��$
 Left=���  � ��$ Right=���  � ��$	 150����  � ��$ 30���� " ��.���  � ��$ Roboto)���  � ��$ 20(���  � ��$ Stretch=��� " ��.���  � ��$ 15(���  � ��$ Calibri)���  � ��$
 Left=���  � ��$ 0 0 0 2q���  � �� �#483883��  �@��
IsEditable$ False����  � ��$	 150���� " d�.R��  �� �� $��20 �.+��� $ Border��$��2��$ 2�����#03a9f4���#03a9f4$��1q��� $��0��$ 2��$��1q����#03a9f4���#03a9f4$��0,0,1,0q�F� $ Arrow��$��1��
�������$��Center=�$��Center=�=����        �    �� � 	= ��[ � z �� � z     � 	=  	= 	�� " ��.R��� $ PART_ContentHost��$|�False��#��z���� " ��.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � ��$
 Auto=���  � ��$
 Auto=���  � ��$
 true����  � ��$ 50����  � ��$ 20����  � ��
 �������  � �� �� " ��.R�� .+�d� $��ToggleButton��8��  $��2��$|�false��$! Press=� �� $ IsDropDownOpen�$" TwoWay=� �� *TemplatedParent+�� $��ContentSite��$# False��#��z$ #��z% #��z��$��3,3,23,3q�$��Center=�$��Center=��� $& PART_EditableTextBox��8�� $
��Left=�$��Center=�$��3,3,23,3q�$
|�True�����#03a9f4
��� � �$y�Hidden=�#( z' ��Z� � $��Popup��$��Bottom=�#��z) $
* True��$|�False��$��Slide=�.��� $��DropDown��$
  True��#��z+ #��z, .+��� $ DropDownBorder�����#03a9f4$��1q����#888888�� $��4,6,4,6q�$
  True��.���� $
- True��$. 	Contained=� P�  �@��HasItems$ false��. ��� $ DropDownBorder�� � ��$ 95��P�  � {�$ false��. ���  � �� �#888888P�  �@��
IsGrouping$
 true��. ���  � ��$ false��P� $/ Popup�� �@�AllowsTransparency$
 true��. ��� $ DropDownBorder�� �@��CornerRadius$ 0���� $ DropDownBorder�� � ��$ 0,2,0,0q�P�  �@��
IsEditable$
 true��. ���  � ��$ false���� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=�0 �� " ��.���  �@J�SnapsToDevicePixels$
 true����  � ��
 �������  �@�OverridesDefaultStyle$
 true����  � �� �� " ��.R��� $��Border��$ 2q�$
  true��._���  P�  �@��IsHighlighted$
 true��. ��� $ Border�� � �� �#0277bdP�  � {�$ false��. ���  � �� �#888888��	#FFE8EDF9����	#FFC5CBF9����	#FF7381F9����	#FFE8EDF9����	#FFC5CBF9����	#FF888888����	#FFC5CBF9����	#FFDDDDDD����	White����	#FF7381F9����	#FF211AA9����	#FF3843C4����	#FF211AA9����	#FF444444����/+sc#1, 0.004391443, 0.002428215, 0.242281124����	#FFCCCCCC����	#FF888888����	#FF444444����	#FF888888����	#FF444444����	#FFAAAAAA����	#FF888888����	Black����	#FFC5CBF9����	Black����	#FFC5CBF9����	#FF3843C4���� $1 0.5,1&�$2 0.5,0&�.��� #3 �  $4 0Q�� #3 �  $	4 0.5Q�� #3 �  $4 1Q��� $	2 0,0&�$	1 1,0&���� � $3 	#000000FF��$4 0Q�� $3 	#600000FF��$	4 0.4Q�� $3 	#600000FF��$	4 0.6Q�� $3 	#000000FF��$4 1Q��� " ��.���  �@�Cursor$
 Hand����  � ��$  ����  � �� �� " ��.R�� .+�A� $5 outerEllipse��$��18��$��18����
� .��� $4 0Q�$3 #b0bec5��� $
4 0.88Q�$3 #b0bec5��� $4 1Q�$3 #b0bec5��A� $��18��$��18����
� .��� $4 1Q�$3 White��A� $5 highlightCircle��$��10��$��10������ .��� $4 1Q�$3 Green���� $
 content��$��Center=�$��Center=� P�  �@d�	IsChecked$
 TrueY�. ��� $ highlightCircle�� � �� �� .��� $4 1Q�$3 #03a9f4���� $ outerEllipse�� � �� 
� .��� $4 1Q�$3 #03a9f4��P�  �@d�	IsChecked$ FalseY�. ��� $ highlightCircle�� � �� �� $2 0.3,0&�$1 0.7,1&�.��� $4 0Q�$3 #b0bec5��� $	4 0.5Q�$3 #b0bec5��� $4 1Q�$3 #b0bec5���� " ��.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  �@�FocusVisualStyle# � ) ��  � �� �� " ��.R��� 
6 ���� 7 �� $ Border��$��15��$��15��$ 0��$��1q����� $	2 0,0&�$	1 0,1&���� � #3 �  $	4 0.0Q�� #3 �  $	4 1.0Q����� $	2 0,0&�$	1 0,1&���� � #3 �  � #3 �  $	4 1.0Q�._�� .+�F� $ 	CheckMark��$y�	Collapsed=�$��8��$��8��$  False��$��2��+����        ���j ��j �    ��j ���j     	���� #8 �  F� $ InderminateMark��$y�	Collapsed=�$��8��$��8��$  False��$��2������    ��j ���j     	���� #8 �  9   CommonStates��.:   
Normal��  	MouseOver��.; �� .���� $ Border��$e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $< 0��8=     Pressed��.; �� .���� $ Border��$e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $< 0��8=  �� $ Border��$g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�.g�  $< 0��8=  �� $ Border��$g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $< 0��8=    Disabled��  CheckStates��.:   Checked��.; �� .��T� $ (UIElement.Visibility)�$ 	CheckMark��..�i� $> 0��#@ Z? �� $ Border��$e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $< 0��$= Orange���� $ Border��$e _(Panel.Background).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�.g�  $< 0��$= Orange��  	Unchecked��  Indeterminate��.; �� .��T� $ (UIElement.Visibility)�$ InderminateMark��..�i� $> 0��#@ Z? ._��� $��4,0,0,0q�$��Center=�$
��Left=�$
��True���� " d�.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � �� �� " d�.R��� 
6 ���� 7 �� $ Border��$��27��$��27��$ 0��$��1q�._�� .+�F� $ 	CheckMark��$y�	Collapsed=�$
A Fill=����#66bb6a$  False��$��1���������C@�e�X��@�e@�eX��@�e��C�@�e���X����!��C��!Ө����!��!�����!��C���!X�����@�e��C@�e��2
��рހ������ю�h�[��:Ѡm`.�� 6n����p^��ͼѠm�1�h�[Tk����(�c��2
@�4��2
��F� $ InderminateMark��$y�	Collapsed=�$
A Fill=����#4dd0e1$  False��$��1���������C��!�X����!@�e���@�e��C�@�eX��X��@�e��C@�eӨ��@�e��!X����!��C���!��������!��C��!��2
���рހ������h���h�[h�LѠm�|�� 6n@���p^���Ѡm��U�h�[�?�����$��2
�S��2
���9   CommonStates��.:   
Normal��  Disabled��  CheckStates��.:   Checked��.; �� .��T� $ (UIElement.Visibility)�$ 	CheckMark��..�i� $> 0��#@ Z?   	Unchecked��.; �� .��T� $ (UIElement.Visibility)�$ InderminateMark��..�i� $> 0��#@ Z?   Indeterminate��.; �� .��T� $ (UIElement.Visibility)�$ InderminateMark��..�i� $> 0��#@ Z? ._��� $��4,0,0,0q�$��Center=�$
��Left=�$
��True���� " 	�.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � �� �� " 	�.R��� 
6 ���� 7 �� $ Border��$��27��$��27��$ 0��$��1q�._�� .+�F� $ CheckMarkSel��$y�	Collapsed=�$
A Fill=����#03a9f4$  False��$��1���������C@�e�X��@�e@�eX��@�e��C�@�e���X����!��C��!Ө����!��!�����!��C���!X�����@�e��C@�e��2
��рހ������ю�h�[��:Ѡm`.�� 6n����p^��ͼѠm�1�h�[Tk����(�c��2
@�4��2
��F� $ 	CheckMark��$y�	Collapsed=�$
A Fill=�
�������$  False��$��1���������C@�e�X��@�e@�eX��@�e��C�@�e���X����!��C��!Ө����!��!�����!��C���!X�����@�e��C@�e��2
��рހ������ю�h�[��:Ѡm`.�� 6n����p^��ͼѠm�1�h�[Tk����(�c��2
@�4��2
��9   CommonStates��.:   
Normal��  Disabled��  CheckStates��.:   Checked��.; �� .��T� $ (UIElement.Visibility)�$ CheckMarkSel��..�i� $> 0��#@ Z?   	Unchecked��.; �� .��T� $ (UIElement.Visibility)�$ 	CheckMark��..�i� $> 0��#@ Z?   Indeterminate��.; �� .��T� $ (UIElement.Visibility)�$ 	CheckMark��..�i� $> 0��#@ Z? ._��� $��4,0,0,0q�$��Center=�$
��Left=�$
��True���� " 	�.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � �� �� " 	�.R��� 
6 ���� 7 �� $ Border��$��27��$��27��$ 0��$��1q�._�� .+�F� $ InderminateMarkSel��$y�	Collapsed=�$
A Fill=����#03a9f4$  False��$��1���������C��!�X����!@�e���@�e��C�@�eX��X��@�e��C@�eӨ��@�e��!X����!��C���!��������!��C��!��2
���рހ������h���h�[h�LѠm�|�� 6n@���p^���Ѡm��U�h�[�?�����$��2
�S��2
���F� $ InderminateMark��$y�	Collapsed=�$
A Fill=�
�������$  False��$��1���������C��!�X����!@�e���@�e��C�@�eX��X��@�e��C@�eӨ��@�e��!X����!��C���!��������!��C��!��2
���рހ������h���h�[h�LѠm�|�� 6n@���p^���Ѡm��U�h�[�?�����$��2
�S��2
���9   CommonStates��.:   
Normal��  Disabled��  CheckStates��.:   Checked��.; �� .��T� $ (UIElement.Visibility)�$ InderminateMarkSel��..�i� $> 0��#@ Z?   	Unchecked��.; �� .��T� $ (UIElement.Visibility)�$ InderminateMark��..�i� $> 0��#@ Z?   Indeterminate��.; �� .��T� $ (UIElement.Visibility)�$ InderminateMark��..�i� $> 0��#@ Z? ._��� $��4,0,0,0q�$��Center=�$
��Left=�$
��True���� " 	�.���  � �� �� " 	�.R��� 
6 �����$
B Hand��7 � $��13��$��13��.+��� $��
RadioOuter��
������ ���#78909c$��1q�$ 30���� $��	RadioMark��$ 30��$��3q����#03a9f4$y�Hidden=�._��� $��5,1,0,0q�$��12(���  P�  �@d�	IsChecked$
 trueY�. ��� $ 	RadioMark�� � y�$ Visible=��� $ 
RadioOuter�� � �� �#03a9f4�� " d�C ����Color.Additional.LightGrey/ #989EA8����Color.Additional.MediumGrey/ #61656B����)%Color.MedtronicBranding.MedtronicBlue/ #004B87����&"Color.MedtronicBranding.CobaltBlue/ #0085CA���� ToggleButtonContentTextbox/ " ��.���  � ��$ Stretch=���  � ��$ Stretch=��� ToggleButtonFalseBackground/ $8 #03a9f4���� ToggleButtonTrueBackground/ $8 #03a9f4���� ToggleButtonFalseForeground/ $8 #1976d2���� ToggleButtonTrueForeground/ $8 #1976d2���� 
OffContent/ 8��  $��1���� OnContentControl/ " ��.���  � ��$ Center=���  � ��$ Center=���  � �� �� .R��� $��Roboto)�$��16(�$��SemiBold"�
�������8�� $��2����  � ��$ Center=���  � ��$ Center=��� OffContentControl/ " ��.���  � ��$ Center=���  � ��$ Center=���  � �� �� .R��� $��Roboto)�$��16(�$��SemiBold"�
�������8�� $��1����  � ��$ Center=���  � ��$ Center=�.���  � �� �� " d�.R�� $D MainRow��.+��  �� ���� $E TopRow��$ ActualHeight��� �� ���� $E TopRow��$ ActualHeight�F �� $G TopRow���� .+�A� $5 BackgroundEllipse1��$��0��$��2��$��0��$��3��$
��Left=�8�� ���� $E MainRow��$ ActualHeight����� $E MainRow��$ ActualHeight�A� $5 BackgroundEllipse2��$��0��$��2��$��0��$��3��$��Right=�8�� ���� $E MainRow��$ ActualHeight����� $E MainRow��$ ActualHeight��� $ BackgroundBorder��$��0��$��2��$��1��8�� � $��Stretch=� �� $H AnimationWidth���� ���� $E MainRow��$ ActualHeight�.+��� $ AnimationSizeBorder��$��0���� $��0��8�� � $��Stretch=� �� ���� $E MainRow��$ ActualHeight��� .+��� $��1��8�� � 
������ .+��� $
��Left=�$I 
Horizontal=�.+��� ���� $E AnimationSizeBorder��$ ActualWidth�A� $5 ForegroundEllipse��$��Right=�8�� ���� $E MainRow��$ ActualHeight����� $E MainRow��$ ActualHeight� P�  �@d�	IsChecked$ FalseY�. ��� $ BackgroundBorder��8 	  � ���� $ ForegroundEllipse��8 
  � ���� $ AnimationSizeBorder�� � ��$ 0��P�  �@d�	IsChecked$
 TrueY�. ��� $ ForegroundEllipse��8   � ���� $ BackgroundBorder��8   � ��