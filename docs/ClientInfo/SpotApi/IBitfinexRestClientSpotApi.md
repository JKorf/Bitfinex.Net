---
title: IBitfinexRestClientSpotApi
has_children: true
parent: IBitfinexClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexClient > SpotApi > IBitfinexRestClient`  
*Spot API endpoints*
  
***
*Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.*  
**ISpotClient CommonSpotClient { get; }**  
***
*Endpoints related to account settings, info or actions*  
**IBitfinexRestClientSpotApiAccount Account { get; }**  
***
*Endpoints related to retrieving market and system data*  
**IBitfinexRestClientSpotApiExchangeData ExchangeData { get; }**  
***
*Endpoints related to orders and trades*  
**IBitfinexRestClientSpotApiTrading Trading { get; }**  
