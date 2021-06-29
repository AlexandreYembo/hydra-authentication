# Hydra Authentication
Api to Register and Authenticate the user in the platform.

Workflow for Signup:

![alt text](https://github.com/AlexandreYembo/hydra-customers/blob/main/Customer_Architecture.png)

### Architecture

### Hydra.Identity.API

Implements the interface to perform actions available so far:
1. Register a new user

2. Allow to login an user and get the token

3. Allow to refresh a token by providing the token Guid.

4. Dispatch a command to the application to process the logic and expect a CommandResult (with success or failed scenario).

#### Hydra.Identity.Application

Contains commands and event and Business logic.

1. Commands and Commands Handlers (change the entity status on database)

2. Events and Event Handlers (represent the action that happened)


#### Hydra.Ideniry.Infrastructure

1. EF context

2. Repositories (concreate implementation)

3. Mappings

4. Migrations
