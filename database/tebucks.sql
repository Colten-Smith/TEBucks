USE master
GO

IF DB_ID('tebucks') IS NOT NULL
BEGIN
	ALTER DATABASE tebucks SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE tebucks;
END

CREATE DATABASE tebucks
GO

USE tebucks
GO

CREATE TABLE tebucks_user (
	user_id int IDENTITY(1001,1) NOT NULL,
	firstname varchar(100) NULL,
	lastname varchar(MAX) NULL,
	username varchar(50) NOT NULL,
	password_hash varchar(200) NOT NULL,
	salt varchar(200) NOT NULL,

	CONSTRAINT PK_user PRIMARY KEY (user_id),
	CONSTRAINT UQ_username UNIQUE (username)
)
CREATE TABLE account (
	account_id int IDENTITY(1, 1) NOT NULL,
	user_id int NOT NULL,
	balance money NOT NULL,

	CONSTRAINT PK_account PRIMARY KEY (account_id),
	CONSTRAINT FK_account_user FOREIGN KEY (user_id) REFERENCES tebucks_user(user_id)
)
CREATE TABLE transfer_type (
	type_id int identity(1, 1) NOT NULL,
	type_name varchar(15) NOT NULL,

	CONSTRAINT PK_type_id PRIMARY KEY (type_id)
)
CREATE TABLE transfer_status (
	status_id int identity(1, 1) NOT NULL,
	status_name varchar(15) NOT NULL,

	CONSTRAINT PK_type PRIMARY KEY (status_id)
)
CREATE TABLE transfer (
	transfer_id int identity(1, 1) NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount money NOT NULL,
	status_id int NOT NULL,
	type_id int NOT NULL,

	CONSTRAINT CHK_Transfer CHECK (amount > 0),

	CONSTRAINT PK_transfer_id PRIMARY KEY (transfer_id),
	CONSTRAINT FK_transfer_from_account FOREIGN KEY (account_from) REFERENCES account(account_id),
	CONSTRAINT FK_transfer_to_account FOREIGN KEY (account_to) REFERENCES account(account_id),

	CONSTRAINT FK_transfer_status FOREIGN KEY (status_id) REFERENCES transfer_status(status_id),
	CONSTRAINT FK_transfer_type FOREIGN KEY (type_id) REFERENCES transfer_type(type_id)
)
INSERT INTO transfer_status (status_name) values('Rejected')
INSERT INTO transfer_status (status_name) values('Approved')
INSERT INTO transfer_status (status_name) values('Pending')

INSERT INTO transfer_type (type_name) values('Send')
INSERT INTO transfer_type (type_name) values('Request')