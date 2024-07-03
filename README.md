Testing web socket:

Connect to https endpoint
wss://localhost:8081/ws

SignalR require Unicode Character (U+001E) at the end of each message. You can copy it from here https://unicodeplus.com/U+001E

Send handshake message
{
  "protocol": "json",
  "version": 1
}

Send subscription message
{
  "arguments": ["btcusdt"],
  "target": "JoinBroadcast",
  "type": 1
}