   M S B A M L   `   `   `  ����  TDQQ$    TDQQ.MyWindow.WinSelectFeature   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	     � Title$  选择要素类��5   	   $	��220��6   $	��300��6$    � WindowStyle$
 None=�60    � 
ResizeMode$ NoResize=�6C    � WindowStartupLocation$ CenterScreen=�6Y   
������  � AllowsTransparency .  � 	Resources 5         RadioButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 A�Name �� EndPoint �� 
StartPoint �� RelativeTransform �� CenterX �� CenterY �� CenterX �� CenterY �� Angle �� CenterX �� CenterY � Color � Offset J� RenderTransformOrigin �� ScaleX �� ScaleY �� AngleX �� AngleY ��Name J� SnapsToDevicePixels �� Triggers P� Property  P� Value! �� 
TargetName��%�  &	        �� 5   
   " ��6$   .�5      ��  � �� 5	      �� 5
      "	 ��6&   .R�5      � .+�5      A� $
 ellipse��6&   ���	#FF414343��5   "   �� 5   &   $ 
0.443,1.22&�6:   $ 0.5,0&�6P    5   *   V� 5   .   .~�5   2   �� $	 0.5Q�6A   $	 0.5Q�6O   �� 5   2   $	 0.5Q�6@   $	 0.5Q�6N   �� 5   2   $ -52.415Q�6B   $	 0.5Q�6R   $	 0.5Q�6`   T� 5   2   5   /   .��5   *   � $ 	#FF434040��67   $ 0Q�6I   � 5   *   $ White��67   $ 1Q�6E   5   '   5      A� 5      $��5.333,4.928q�68   $ 0.5,0.5&�6M   ��6&   Z� ��5   "   �� 5   &   $ 0.563,-0.397&�6:   $ 0.406,1.322&�6R    5   *   V� 5    .   .~�5!   2   �� $	 0.5Q�6A   $	 0.5Q�6O   $ -1Q�6]   $ -1Q�6i   �� 5"   2   $ 0Q�6@   $ 0Q�6K   $	 0.5Q�6V   $	 0.5Q�6d   �� 5#   2   $ -52.415Q�6B   $	 0.5Q�6R   $	 0.5Q�6`   T� 5$   2   5%   /   .��5'   *   � $ 	#FF434040��67   $ 1Q�6I   � 5(   *   $ White��67   5)   '   5+      �� 5,      $ contentPresenter��6/   #��z��6I   #��z��6�   # z 6�   ��. 6  5-       5.      P� 5/       �@J�	IsFocused  . 6;   P� 50       �@��IsDefaulted  . 6=   P� 51       �@J�IsMouseOver  . . �52   "   �� $! ellipse��6;    � ��
 �����6P   �� 53   "   $! ellipse��6D    � ��$	 1.5��6Y   54      P� 55       � ��  . . �56   "   �� $! ellipse��69    � �� 57   &   �� 58   *   $ 
0.443,1.22&�6>   $ 0.5,0&�6T    59   .   V� 5:   2   .~�5;   6   �� $	 0.5Q�6E   $	 0.5Q�6S   �� 5<   6   $	 0.5Q�6D   $	 0.5Q�6R   �� 5=   6   $ -52.415Q�6F   $	 0.5Q�6V   $	 0.5Q�6d   T� 5>   6   5?   3   .��5A   .   � $ 	#FF434040��6;   $ 1Q�6M   � 5B   .   $ White��6;   $ 0Q�6I   5C   +   5E   #   �� 5F   "   $! contentPresenter��6;    � ��$ 5,5,0,0q�6Y   5G      P� 5H       � {�  .  6;   5J      5L      5M      5N      .��5O      � " � RowDefinitions" 5P   
   �� 5Q      $��25 �6   �� 5R      $��* �6   6)   .+�5T   
   �� # �� CornerRadius$# 10��6   $��2��6#   ��5U      �� 5V      $	 0,0&�6&   $	 0,1&�67   .��5W      � $ 	LightGray��6#   $	 0.1Q�65   � 5X      $ DarkGray��6#   $	 0.9Q�64   5Y      5[      �� 5\   
   -   5        $��
ImageClose��5\   l   $��/Image/error.png��6   $��Right=�6*   $��3q�6F   $��0��6Q   $ � Cursor$
$ Hand��6^   6l   �� 5]   
   $��Center=�6   % �� Orientation$% Vertical=�63   $��1��6J   .+�5^      �� -   5        $��
LabelTitle��5^   z   $��请选择要素类��6   $��Center=�6%   $��19(�6B   $	��Top=�6P   $��0,20,0,0q�6h   6z   �� 5_      -   5        $��SelectFeauture��5_      $��Center=�6-   $	��200��6J   $��25��6V   $��0,5,0,0q�6b   �� 5`      -   5        $��ButtonConfirm��5`      $��50��6*   $��50��66   $��Center=�6A   $��0,10,0,0q�6^   $��确定��6p   #���   5a      $��18(�6<   $
��Bold"�6K   $
$ Hand��6]   5b      5d      5e      