   M S B A M L   `   `   `  ����  NBot$    NBot.Pages.Config.Page_EditPLC   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Z VFirstFloor.ModernUI, Version=1.0.6.0, Culture=neutral, PublicKeyToken=2d21ec3cd074c59a0mui&http://firstfloorsoftware.com/ModernUI  5      -   5        .��5	      �    ContentRoot#��[  6   .+�5
   
   F� -   5          F�Name$  nok_icon��5
      $��23��6!   $��23��6,    �� Stretch$
 Fill=�68   
���  ���������C�<R�'t�<R@�e�E�MTn5�Tn5|�M�<R|�M�:Tn5���!�E���<R��C�<R���C��Ӭ�/��(����(�t���(�H����/���C����W��hH���ht����h�����W����C���(�2��X�+�x��D�[�x���h2��(�2�$y�Hidden=�6   F� 5   
   -   5        $  ok_icon��5      $��23��6    $��23��6+   $
 Fill=�67   
���/���F�����ej�Z�����x��,E=R�ܿe������Tn5�$�!��C��ej�Z�$y�Hidden=�6�   �� 5   
   .��5      �� $	��200��6   .+�5      �� $��Edit PLC unit��6     Heading2#��[ 61   
��� E��6S   � 5      $	��349��6   .+�5      �� -   5         ��Name$ NameL��5       $
��Name��6/   
��� ���$��	10,14,0,0q�6O   $
��Left=�6b   $	��106��6}   $��22��6�   $	��Top=�6�   �� 5      -   5         ��Name$ NameTB��5      $��	88,14,0,0q�6.   $��22��6A   $	��Top=�6M   $
��Left=�6e   $	��190��6�   6�   �� 5      $
��Left=�6   $��23��68   $��
283,14,0,0q�6D   $	��Top=�6X   $��23��6p   .+�5      F� -   5        $  
INameCheck��5      $��23��63   $��23��6>   $
 Fill=�6J   5      �� 5      -   5        $ IPaddrL��5       $��
IP address��61   
��� ���$��	10,41,0,0q�6W   $
��Left=�6j   $	��106��6�   $��22��6�   $	��Top=�6�   �� 5      -   5        $ IPAddTB��5      $��	88,41,0,0q�6/   $��22��6B   $	��Top=�6N   $
��Left=�6f   $	��190��6�   6�   �� 5      $
��Left=�6   $��23��68   $��
283,42,0,0q�6D   $	��Top=�6X   $��23��6p   .+�5      F� -	   5        $  IIpAddrCheck��5      $��23��65   $��23��6@   $
 Fill=�6L   5      �� 5      -
   5        $ SlotNL��5       $��Slot number��60   
��� ���$��	10,68,0,0q�6W   $
��Left=�6j   $	��106��6�   $��22��6�   $	��Top=�6�   �� 5      -   5        $ SlotNTB��5      $��	88,68,0,0q�6/   $��22��6B   $	��Top=�6N   $
��Left=�6f   $	��190��6�   6�   �� 5      $
��Left=�6   $��23��68   $��
283,70,0,0q�6D   $	��Top=�6X   $��23��6p   .+�5      F� -   5        $  ISlotNCheck��5      $��23��64   $��23��6?   $
 Fill=�6K   5      �� 5       $��Beacon configuration��6    #��[ 6<   
��� E��$��10,115,-10,201q�6u   �� 5!      -   5        $ RefreshTimeL��5!       $��Refresh time��66   
��� ���$��
10,153,0,0q�6^   $
��Left=�6r   $	��106��6�   $��22��6�   $	��Top=�6�   �� 5"      -   5        $ 	RefTimeTB��5"      $��121,153,0,0q�61   $��22��6F   $	��Top=�6R   $
��Left=�6j   $	��157��6�   6�   �� 5#      $
��Left=�6   $��23��68   $��283,153,0,0q�6D   $	��Top=�6Y   $��23��6q   .+�5$      F� -   5        $  IRefreshCheck��5$      $��23��66   $��23��6A   $
 Fill=�6M   5%      �� 5&      -   5        $ 	SamplesNL��5&       $��Number of samples��63   
��� ���$��
10,180,0,0q�6`   $
��Left=�6t   $	��106��6�   $��22��6�   $	��Top=�6�   �� 5'      -   5        $ 
SamplesNTB��5'      $��121,180,0,0q�62   $��22��6G   $	��Top=�6S   $
��Left=�6k   $	��157��6�   6�   �� 5(      $
��Left=�6   $��23��68   $��283,181,0,0q�6D   $	��Top=�6Y   $��23��6q   .+�5)      F� -   5        $  ISamplesNCheck��5)      $��23��67   $��23��6B   $
 Fill=�6N   5*      �� 5+      -   5        $
 DBNL��5+       $��	DB number��6.   
��� ���$��
10,207,0,0q�6S   $
��Left=�6g   $	��106��6�   $��22��6�   $	��Top=�6�   �� 5,      -   5        $ DBNTB��5,      $��121,207,0,0q�6-   $��22��6B   $	��Top=�6N   $
��Left=�6f   $	��157��6�   6�   �� 5-      $
��Left=�6   $��23��68   $��283,207,0,0q�6D   $	��Top=�6Y   $��23��6q   .+�5.      F� -   5        $  	IDBNCheck��5.      $��23��62   $��23��6=   $
 Fill=�6I   5/      �� 50      -   5        $ ByteNL��50       $��Byte number��60   
��� ���$��
10,234,0,0q�6W   $
��Left=�6k   $	��106��6�   $��22��6�   $	��Top=�6�   �� 51      -   5        $ ByteNTB��51      $��121,234,0,0q�6/   $��22��6D   $	��Top=�6P   $
��Left=�6h   $	��157��6�   6�   �� 52      -   5        $ BitNL��52       $��
Bit number��6/   
��� ���$��
10,261,0,0q�6U   $
��Left=�6i   $	��106��6�   $��22��6�   $	��Top=�6�   �� 53      $
��Left=�6   $��23��68   $��283,235,0,0q�6D   $	��Top=�6Y   $��23��6q   .+�54      F� -   5        $  IByteNCheck��54      $��23��64   $��23��6?   $
 Fill=�6K   55      �� 56      -   5        $ BitNTB��56      $��121,261,0,0q�6.   $��22��6C   $	��Top=�6O   $
��Left=�6g   $	��157��6�   6�   �� 57      $
��Left=�6   $��23��68   $��283,261,0,0q�6D   $	��Top=�6Y   $��23��6q   .+�58      F� -   5        $  
IBitNCheck��58      $��23��63   $��23��6>   $
 Fill=�6J   59      �� 5;      -   5         ��Name$ BCancel��5;      $��Cancel��6/     SquareButtonStyle#���  6@   $��203,309,0,0q�6l   $
��Left=�6�   $��75��6�   $��30��6�   $	��Top=�6�   6�   �� 5<      -   5        $
 BAdd��5<      $
��Save��6,   #���  6;   $��
10,309,0,0q�6g   $
��Left=�6{   $��75��6�   $��30��6�   $	��Top=�6�   6�   5=      5>      5?      5@      5A      