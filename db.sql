CREATE DATABASE expenses;

CREATE TABLE IF NOT EXISTS users (
	id SERIAL NOT NULL PRIMARY KEY,
	name CHAR(200) NOT NULL,
	email CHAR(45) NOT NULL,
	password CHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS expenses (
	id SERIAL NOT NULL,
	description CHAR(200) NOT NULL,
	value DECIMAL(8,2) NOT NULL,
	type INT NOT NULL,
	user_id INT NOT NULL,
	FOREIGN KEY (user_id) REFERENCES users (id)
);
