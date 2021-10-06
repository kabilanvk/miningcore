DROP TABLE IF EXISTS payment_schedule;

CREATE TABLE poolstate
(
	poolid TEXT NOT NULL PRIMARY KEY,
	hashvalue DOUBLE PRECISION NOT NULL DEFAULT 0,
	lastpayout TIMESTAMP NULL
);