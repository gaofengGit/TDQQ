   M S B A M L   `   `   `  ����U  QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35[<clr-namespace:Microsoft.Windows.Controls;assembly=WPFToolkitMicrosoft.Windows.Controls  5   
    TDQQ!   TDQQ.MyWindow.WinFarmerInfo   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation       5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   NtoolKit<clr-namespace:Microsoft.Windows.Controls;assembly=WPFToolkit    5   
     � Title$  WinFarmerInfo��5   
   $	��500��6    $	��300��6-    � WindowStyle$
 None=�69    � WindowStartupLocation$ CenterScreen=�6L    � AllowsTransparency . 
������  � 
ResizeMode$ NoResize=�5   "   .��5      �  � RowDefinitions 5   
   �� 5	      $��30 �6   �� 5
      $��30 �6   �� 5      $��* �6   .+�5   
   �� $��3��6    �� CornerRadius$ 10��6"   ��5      �� 5       �� 
StartPoint$	 0,0&�6&    �� EndPoint$	 0,1&�67   .��5      � 	 � Color$
	 Gray��6#   
 � Offset$	
 0.1Q�60   � 5      $	 DarkGray��6#   $	
 0.9Q�64   5      5      �� 5   
   -   5        $��	LabelMove��5   C   $��0��6    � Cursor$
 Hand��6   $	��300��6+   $��30��67   6C   �� 5   
   -   5        $��
ImageClose��5   l   $��/Image/error.png��6   $��Right=�6*   $��3q�6F   $��0��6Q   $
 Hand��6^   6l   �� 5   
   $��农户信息��6   $��1��6   $��Center=�6,   $��Center=�6G   $��17(�6d   � 5   
   $��2��6   .+�5      )   #Microsoft.Windows.Controls.DataGrid  -   5        $��DataGridFarmerInfo��5   6   $��10q�5      
�������"   VerticalScrollBarVisibility$
 Auto=�6A      AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserResizeRows .     SelectionMode$ Single=�5      ��5       ��    Columns 5      3   -Microsoft.Windows.Controls.DataGridTextColumn  5      /   )Microsoft.Windows.Controls.DataGridColumn   Header$ 承包方编码��61      Width8   2Microsoft.Windows.Controls.DataGridLengthConverter$	 150 6_   4   .Microsoft.Windows.Controls.DataGridBoundColumn   Binding 6@   ��  �� Path$ Cbfbm�5         5!      $ 承包方名称��61   $	 120 6_    6@   �� $ Cbfmc�5"      5$      5%      5'      5(      