# Team 3

## How to setup

### Client
For the client a .exe file is provided. This file is located in 
```
./MVP
```
To execute the Client, open the .exe file

### Server
For the Server is a docker-compose file provided. This file starts the database, flyway, and the server.

To start all containers first switch to
```
./Source/Server
```

Then to start it run

```
docker compose up --build
```

## Branch Structure

From now on only this branch structure is to be used.

### Main Branch
In this branch is only code which has been tested and declared working.

### Development Branch
This Branch is to be used when implementing new features.

### Test Branch
When a new feature is implemented the dev-branch is merged to the test branch to mark the changes ready to test.
If the feature is fully tested and working this branch is merged into main branch.

