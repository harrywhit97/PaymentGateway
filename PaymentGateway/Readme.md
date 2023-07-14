# Harry Whittaker - Checkout.com techinical take home

## How to run it
- Ensure you have the Net 7 CLR
- Open in Visual Studio
- Make sure the Api project is selected as the start up project
- Run it (F5)

A console window and browser will open with the swagger page.

## How to use it
Use the swagger page to hit the endpoints. 
Note that it uses an in memory DB so any created records will no longer exist when restarting.

## Design
I have split it up in to three projects
- Domain - This is where the core logic and models live
- Api - the Api web project to expose the domain functionality via an API
- Domain.Tests - Unit tests for the domain

### Domain
- Models
    - I used GUID ids to prevent client side Id enumeration
- Mediatr
    - I have used Mediatr to encapsulate the logic of the two requests. Reasons:
        - It allows cross cutting concerns (e.g. logging, validation) to be handled an a single place i.e pipeline behaviours
- Fluent validation
    - Each Mediatr request has a fluent validator which is invoked in the request pipeline. 
        If the validator fails then a validation exception is thrown or is returned depending 
            if the repose is a result type without invoking the request handler.
- Result Type
    - I added a Result/Maybe type monad to allow the passing of errors up from the Domain layer in to the web layer. 
        Then it becomes the web layers decision on how to handle the error e.g what HTTP status code to return
        I used the Result type as the response type of the Mediatr requests along with custom exceptions to enable the following mapping
        - ValidationException -> 400 (Bad Request). Generated when the fluent validator fails
        - NotFoundException -> 404 (Not Found). Generated when client queries for payment that does not exist
        - Any other exception -> 500 (Internal Server Error)

## Improvements

### Database
- I created my own in memory database using lists which was a bit of a shortcut
        It would be better to use entity framework as there is an in memory option for that. 
- Normalise DB by placing cards in a separate table
- Since the card information is sensitive I would also perform field level DB encryption on it
- Create database models which are used to decouple the persistence models from the domain models
### Security
- Add authentication using OAuth2 and JWTs

### Improve validation
  - Merchant exists
  - Specified merchant matches authenticated connecting merchant
  - Currency is valid and matches card currency - potentially handled by CKO API?
### API
- Move DTO mapping to out of controller and test it. Maybe use AutoMapper for it depending on the complexity of the mappings?
### Resiliency
- If the CKO api requests fails with a time out or for an un expected reason it would be good to have a retry policy. This could be done using Polly.
### Client experience
- A client library could be built and published to Nuget to make it easier for client to connect
- If the CKO API has performance issues it would be better to do the following to unblock the client:
        - Create the payment in a state of 'Processing'
        - Enqueue a job using a job scheduler e.g. Hangfire
        - Return the Id of the payment to the client with a 202 (Accepted) code to denote that we accepted their request but have not completed the processing
        - The client can then poll the API until the job is completed
### Testing
- I did not test everything as I ran out of time, but it would be good to write more for maximum test coverage
## Deployment
- I would containerize the application by building it in to a Linux docker image
- The image would be released to an Azure Application Service with blue/green deployemets enables to minimize downtime during deployments
- I would have two environments.
    - UAT - used to run an automated API testing suite and for manual tesing/investigation of PROD issues
    - PROD
- I would enable Azure Log Analytics to capture the logs and metrics.
     Then I would build queries, alerts and dashboards to improve monitoring