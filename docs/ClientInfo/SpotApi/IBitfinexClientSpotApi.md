---
title: IBitfinexClientSpotApi
has_children: true
parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexClient > SpotApi`  
*Spot API endpoints*
  
***
*Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.*  
**ISpotClient CommonSpotClient { get; }**  
***
*Endpoints related to account settings, info or actions*  
**[IBitfinexClientSpotApiAccount](IBitfinexClientSpotApiAccount.html) Account { get; }**  
***
*Endpoints related to retrieving market and system data*  
**[IBitfinexClientSpotApiExchangeData](IBitfinexClientSpotApiExchangeData.html) ExchangeData { get; }**  
***
*Endpoints related to orders and trades*  
**[IBitfinexClientSpotApiTrading](IBitfinexClientSpotApiTrading.html) Trading { get; }**  
