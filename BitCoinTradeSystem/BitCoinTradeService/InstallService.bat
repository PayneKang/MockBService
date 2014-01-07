
installutil.exe BitCoinTradeService.exe
Net Start TradeOrderService
sc config TradeOrderService start= auto
pause