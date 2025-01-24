CREATE SEQUENCE PrivilegeIdSequence
START WITH 1;

CREATE SEQUENCE UserIdSequence
START WITH 1;

CREATE SEQUENCE GroupIdSequence
START WITH 1;

CREATE TABLE Privileges (
    privilege_id INT PRIMARY KEY DEFAULT nextval('PrivilegeIdSequence'),
    privilege_description TEXT
);

CREATE TABLE Users (
    user_id INT PRIMARY KEY DEFAULT nextval('UserIdSequence'),
    username TEXT NOT NULL UNIQUE,
    password bytea NOT NULL,
    salt bytea NOT NULL,
    email TEXT NOT NULL UNIQUE,
    is_banned BOOLEAN NOT NULL,
    reset_pw BOOLEAN NOT NULL DEFAULT false,
    privilege_id INT REFERENCES Privileges(privilege_id)
);

CREATE TABLE UserGroups (
    group_id INT PRIMARY KEY DEFAULT nextval('GroupIdSequence'),
    group_name TEXT NOT NULL
);

CREATE TABLE UserGroupMembers (
    group_id INT NOT NULL REFERENCES UserGroups(group_id),
    user_id INT NOT NULL REFERENCES Users(user_id),
    PRIMARY KEY(group_id, user_id)
);