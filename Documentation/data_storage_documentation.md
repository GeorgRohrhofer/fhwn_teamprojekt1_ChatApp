# Data Storage Documentation
This document outlines which data is stored and why it is stored.

## Database

### User
In the User Relation everything regarding user information is stored. 

- User Id
- Username
- Password
- Salt
- Email 
- Is banned
- Reset password
- Privilege Id

#### User Id
The user id is used to identify a user

#### Username
The username is set by the user while registering and can then be used to login into the application. 

#### Password
The password is stored as a hashed value is used to authenticate a user during login.

#### Salt
The password is hashed while using a salt value. This salt value is also stored in the database

#### Email
The email can also be used to identify a user and has to be inserted when registering

#### Is banned
The database stores also if a user is banned. This can only happen, when an administrator decides to ban a user.

#### Reset Password
If the reset_password flag is set in the database the user can login the next time without using a password, but then has to insert a new password. After that the password is set the reset_password flag is set back to false.

#### Privilege Id
This is stored to keep track of the users permissions. Currently there are only two options: "User" and "Admin".

### Group
For the group the following values are stored

- Group Id
- Name

#### Group Id
The group id is used to identify a unique group within the dataset

#### Name
The name of the group is stored so the user can identify the groups. \
A chat with only two users is also represented as a group chat with only two participants.

### Privileges
For the Privileges the following values are stored

- Privilege Id
- Description

#### Privilege Id
The privilege id is used to identify a privilege. This is used to assign users their permissions.

#### Description
This is a short description of the privilege. Currently this only contains "User" and "Admin" and is used as a name field. In future use this field could be used for a finer description when finer user permissions are added.

### Group Membership
The Group Memmership contains which users are in what groups.
The following values are stored

- User Id
- Group Id

This combination is used to assign users to their groups.

## Client: Config-File
The config file on the client side is used to store the login information, which is stored when a login attempt was successful.

### IP Address
This is the IP adress of the server which the client was connected the last time.

### Username
This is the username which the last login attempt was successful with.

## Server: Config-File
This Config-File is used to store the parameters which are needed for the server start.