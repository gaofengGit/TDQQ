   M S B A M L   `   `   `  ����M  IFluent, Version=1.3.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5   	    TDQQ   TDQQ.MainWindow   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   5fluent$clr-namespace:Fluent;assembly=Fluent    5   	     � WindowStartupLocation$  CenterScreen=�5   	    � WindowState$ 	Maximized=�6.   ���	#FFEBEDF0 � Icon$ /Image/globe.ico��5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   .+�5   
      Fluent.Ribbon  $��0��6      Tabs. 5         Fluent.RibbonTabItem     ReduceOrder$ (P),(P),(P),(P),(P)��6#      Header$ 数据��6E   $��15(�6Q      Groups. 5         Fluent.RibbonGroupBox  -   5          Name$ Default��5   (   	   Header$	 打开��69   .@�5         Fluent.Button  -   5        $��ButtonOpenMap��5   O      Fluent.RibbonControl
   Text$
 打开地图��6$      	LargeIcon$ Image\open-alt.png��60   6O   5        5      $	 关闭��6)   .@�5        -   5        $��ButtonCloseMap��5   L   $
 关闭地图��6$   $ Image\error.png��60   6L   5      5        5      $A ;Default,Default,Default,Large,Large,Large,Other,Other,Other��6#   $ 编辑��6m   $��15(�6y   . 5        $	 字段��6)   .@�5        -   5        $��ButtonAddField��5   O   $
 编辑字段��6$   $ Image\plus_alt.png��60   6O     5      -   5        $��ButtonDefault��5   L   $
 设值��6$   $ Image\default.png��6.   6L   5        5      $	 提取��6)   .@�5        -   5        $��ButtonBoundary��5   Q   $
 提取四至��6$   $ Image\milestone.png��60   6Q     5       -   5        $��	ButtonJzd��5    Q   $
 提取界址点��6$   $ Image\plotchart.png��61   6Q     5!      -   5        $��	ButtonJzx��5!   O   $
 提取界址线��6$   $ Image\screen.png��61   6O     5"      -	   5        $��
ButtonDkbm��5"   N   $
 地块编码��6$   $ Image\compile.png��60   6b   5#        5$      $	 更新��6(   .@�5%        -
   5        $��ButtonSetHtmj��5%   T   $
 设置合同面积��6$   $ Image\contractmj.png��62   6T     5&      -   5        $��ButtonSetScmj��5&   T   $
 设置实测面积��6$   $ Image\contractmj.png��62   6T     5'      -   5        $��ButtonUpdateCbfbm��5'   R   $
 更新承包方编码��6$   $ Image\refresh.png��63   6R     5(      -   5        $��ButtonUpdateCbfmc��5(   R   $
 替换承包方名称��6$   $ Image\replace.png��63   6R     5)      -   5        $��ButtonChangeCbfmc��5)   K   $
 纠错��6$   $ Image\check.png��6.   6K   5*      5+        5,      $A ;Default,Default,Default,Large,Large,Large,Other,Other,Other��6#   $ 查询��6m   $��15(�6y   . 5-        $	 查询��6)   .@�5.        -   5        $��ButtonField��5.   X   $
 地块信息��6$   $! Image\rounded-rectangle.png��60   6X     5/      -   5        $��ButtonFarmer��5/   M   $
 农户信息��6$   $ Image\farmer.png��60   6M     50      -   5        $��
ButtonArea��50   K   $
 面积信息��6$   $ Image\info.png��60   6K   51      52        53      $A ;Default,Default,Default,Large,Large,Large,Other,Other,Other��6#   $ 分析��6m   $��15(�6y   . 54        $	 地块点选��6)   .@�55        -   5        $��ButtonStartPoint��55   K   $
 开始点选��6$   $ Image\edit.png��60   6K     56      -   5        $��ButtonStopPoint��56   M   $
 结束点选��6$   $ Image\cancel.png��60   6M   57        58      $	 地块框选��6)   .@�59        -   5        $��ButtonStartBox��59   X   $
 开始框选��6$   $! Image\rounded-rectangle.png��60   6X     5:      -   5        $��ButtonStopBox��5:   M   $
 结束框选��6$   $ Image\cancel.png��60   6M   5;      5<        5=      $A ;Default,Default,Default,Large,Large,Large,Other,Other,Other��6#   $ 输出��6m   $��15(�6y   . 5>        $	 标准输出��6)   .@�5?        -   5        $��ButtonATable��5?   F   $

 A表��6$   $ Image\A.png��6.   6F     5@      -   5        $��ButtonBTable��5@   F   $

 B表��6$   $ Image\b.png��6.   6F     5A      -   5        $��ButtonCTable��5A   F   $

 C表��6$   $ Image\c.png��6.   6F     5B      -   5        $��ButtonDTable��5B   F   $

 D表��6$   $ Image\d.png��6.   6F     5C      -   5        $��ButtonETable��5C   G   $

 E表��6$   $ Image\e.png��6.   6G   5D        5E      $	 需求输出��6)   .@�5F        -   5        $��ButtonFamily��5F   M   $
 家庭成员��6$   $ Image\family.png��60   6M     5G      -   5        $��ButtonOpenTable��5G   J   $
 	公示表��6$   $ Image\open.png��6/   6J     5H      -   5        $��ButtonSignTable��5H   \   $
 	签字表��6$   $% Image\digital_signature_pen.png��6/   6\   5I        5J      $	 成果输出��6)   .@�5K        -   5        $��ButtonContract��5K   Q   $
 	合同书��6$   $ Image\transaction.png��6/   6Q     5L      -   5        $��ButtonCertification��5L   W   $
 经营权证��6$   $  Image\ssl_certificates.png��60   6W     5M      -    5        $��
ButtonList��5M   K   $
 颁证清册��6$   $ Image\list.png��60   6K     5N      -!   5        $��ButtonRegister��5N   K   $
 	登记薄��6$   $ Image\diary.png��6/   6K     5O      -"   5        $��ButtonDepartment��5O   V   $
 公示公告��6$   $ Image\departmentbroad.png��60   6V   5P      5Q        5R      $A ;Default,Default,Default,Large,Large,Large,Other,Other,Other��6#   $ 工具��6m   $��15(�6y   . 5S        $	 	承包方��6)   .@�5T        -#   5        $��ButtonCbfbm��5T   M   $
 承包方编码��6$   $ Image\excel.png��61   6M     5U      -$   5        $��ButtonImportBasic��5U   V   $
 导入基础数据表��6$   $ Image\table_export.png��63   6V   5X        5Y      $	 	发包方��6)   .@�5Z        -%   5        $��ButtonImportFbf��5Z   T   $
 导入发包方��6$   $ Image\table_export.png��61   6T   5[      5\        5]      $A ;Default,Default,Default,Large,Large,Large,Other,Other,Other��6#   $ 帮助��6m   $��15(�6y   . 5^        $	 帮助��6)   .@�5_        -&   5        $��
ButtonHelp��5_   I   $
 帮助��6$   $ Image\help.png��6.   6I   5`      5a      5b      � 5c   
   $��1��6   .+�5d      7  1System.Windows.Forms.Integration.WindowsFormsHost  -'   5        $��MainFormsHost��5d   +   $��0q�6    6+   5e      5f      5g      