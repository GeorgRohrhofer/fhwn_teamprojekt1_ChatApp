CREATE SEQUENCE MessageIdSequence START WITH 1;

CREATE TABLE Messages(
    message_id INT PRIMARY KEY DEFAULT nextval('MessageIdSequence'),
    message_text TEXT NOT NULL,
    message_timestamp TIMESTAMP NOT NULL,
    user_id INT NOT NULL REFERENCES Users(user_id),
    group_id INT NOT NULL REFERENCES UserGroups(group_id)
);