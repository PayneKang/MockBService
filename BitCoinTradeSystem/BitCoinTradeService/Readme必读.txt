1. 系统需求 
	1.1 Windows 64 Bit
	1.2 .NetFramework 4.0
	1.3 如果系统是32位，有可能需要替换InstallUtil.exe (可以在系统盘搜索此文件)
2. 配置内容存储在BitCoinTradeService.config文件内
	2.1 日志配置信息
	2.2 接口配置信息
	2.3 OrderInterval 每次计算订单成功后，进行下一次计算的间隔时间，单位：ms 毫秒
3. InstallService.bat 安装并自动启动Service
4. UninstallServic.bat 删除Service