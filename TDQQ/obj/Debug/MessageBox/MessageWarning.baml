   M S B A M L   `   `   `  ����  TDQQ$    TDQQ.MessageBox.MessageWarning   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   -   5          �Name$
  main��5   	    � Title$ MessageWarning��5      $	��130��6(   $	��280��65    � WindowStyle$
 None=�6A    � 
ResizeMode$ NoResize=�6T    � AllowsTransparency .  � WindowStartupLocation$ CenterScreen=�5   1   ���	#FFA4A4A4 � Triggers 5      :� 5   
    :� RoutedEvent$ Window.Loaded��6   .[�5      �� .��5	      �� -   5         g�Name$ sbOpShow��5	      .��5
      W� 	 �� 
TargetName$
	 main��5      
 �� TargetProperty$
 Opacity�5       W� From$ 0X�5      	 W� To$ 1X�6    g� Duration$ 0:0:0.15D�6%   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��70 �6   �� 5      $��35 �6   .+�5   
   �� $��0��6   $��25��6   $��0,0,0,1q�5      ���	#FFEFE2E2��5   *   ��  �� ElementName$
 main�� �� Path$ Width� �� UpdateSourceTrigger$ PropertyChanged=�._�5      �� -   5         ��Name$ lblTitle��5      $
��test��6*   $
��Left=�66   
���   �$��14(�5   /   $��Center=�5      $��5q�67   5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   .+�5      �� $��/Image/Warning.png��6   $��
5,10,10,10q�60   �� 5       -   5        $ lblMsg��5       
�������$��14(�6<   $
��Wrap=�6K   $
��test��6_   $��5,0,0,0q�6k   $��Center=�5!      $��Center=�65   5"      �� 5#   
   $��2��6   $ 
Horizontal=�6"   .+�5$      �� -   5        ���	#FF60C1C1$	��0.5q�5$   -   $��22��6C   $��Center=�6O   $��50��6l   $��200,-15,0,0q�6w   ���	#FFC7C7C7 �� CornerRadius$ 2��6�   ._�5%      �� $��确定��6   $��Center=�6&   $��Center=�6C   $��15(�6^    � Cursor$
 Hand��6l   5&      5'      5(      5)      