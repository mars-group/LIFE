CREATE TABLE datacube_test_ch.d_t1
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t10
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t2
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t3
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t4
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t5
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t6
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t7
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t8
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.d_t9
(
  value TEXT NOT NULL,
  time VARCHAR(36) NOT NULL,
  functionalId VARCHAR(36) UNIQUE NOT NULL,
  id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE datacube_test_ch.facts
(
  f_fact TEXT NOT NULL,
  f_t1 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t1 ( functionalId ),
  id SERIAL PRIMARY KEY NOT NULL,
  f_t2 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t2 ( functionalId ),
  f_t3 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t3 ( functionalId ),
  f_t4 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t4 ( functionalId ),
  f_t5 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t5 ( functionalId ),
  f_t6 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t6 ( functionalId ),
  f_t7 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t7 ( functionalId ),
  f_t8 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t8 ( functionalId ),
  f_t9 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t9 ( functionalId ),
  f_t10 VARCHAR(36) NOT NULL REFERENCES datacube_test_ch.d_t10 ( functionalId )
);
