---
title: Socket API documentation
has_children: true
---
*[generated documentation]*  
### BitfinexSocketClient  
*Client for accessing the Bitfinex websocket API*
  
***
*Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.*  
**void SetApiCredentials(ApiCredentials credentials);**  
***
*Spot streams*  
**[IBitfinexSocketClientSpotStreams](SpotApi/IBitfinexSocketClientSpotStreams.html) SpotStreams { get; }**  
