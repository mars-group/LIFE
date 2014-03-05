CREATE TABLE d_t1
(
    value TEXT NOT NULL,
    time VARCHAR NOT NULL,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t10
(
    value TEXT NOT NULL,
    time VARCHAR NOT NULL,
   id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t2
(
    value TEXT NOT NULL,
  time VARCHAR NOT NULL,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t3
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t4
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t5
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t6
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t7
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t8
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE d_t9
(
    value TEXT NOT NULL,
  time varchar not null,
    id SERIAL PRIMARY KEY NOT NULL
);

CREATE TABLE facts
(
    f_fact TEXT NOT NULL,
    f_t1 INT NOT NULL,
    id SERIAL PRIMARY KEY NOT NULL,
    f_t2 INT NOT NULL,
    f_t3 INT NOT NULL,
    f_t4 INT NOT NULL,
    f_t5 INT NOT NULL,
    f_t6 INT NOT NULL,
    f_t7 INT NOT NULL,
    f_t8 INT NOT NULL,
    f_t9 INT NOT NULL,
    f_t10 INT NOT NULL
);
ALTER TABLE facts ADD FOREIGN KEY ( f_t1 ) REFERENCES d_t1 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t10 ) REFERENCES d_t10 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t2 ) REFERENCES d_t2 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t3 ) REFERENCES d_t3 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t4 ) REFERENCES d_t4 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t5 ) REFERENCES d_t5 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t6 ) REFERENCES d_t6 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t7 ) REFERENCES d_t7 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t8 ) REFERENCES d_t8 ( id );
ALTER TABLE facts ADD FOREIGN KEY ( f_t9 ) REFERENCES d_t9 ( id );