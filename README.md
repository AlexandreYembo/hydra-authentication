# Hydra Authentication
Api to Register and Authenticate the user in the platform.

Workflow for Signup:

![alt text](https://github.com/AlexandreYembo/hydra-customers/blob/main/Customer_Architecture.png)

### Architecture

### Hydra.Identity.API

Implements the interface to perform actions available so far:
- Register a new user

- Allow to login an user and get the token

- Allow to refresh a token by providing the token Guid.

#### Hydra.Identity.Application

Implement the CQRS design architecture

1. Commands and Commands Handlers (change the entity status on database)

2. Events and Event Handlers (represent the action that happened)


#### Hydra.Ideniry.Infrastructure

1. EF context

2. Repositories (concreate implementation)

3. Mappings

4. Migrations

5. Services (for intergration propose)
