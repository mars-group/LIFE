CREATE TABLE d_t1
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t10
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t2
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t3
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t4
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t5
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t6
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t7
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t8
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE d_t9
(
  value VARCHAR(36) NOT NULL,
  time BIGINT NOT NULL,
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId)
);

CREATE TABLE facts
(
  f_fact VARCHAR(36) NOT NULL,
  f_t1 VARCHAR(36) NOT NULL REFERENCES d_t1 ( functionalId ),
  f_t2 VARCHAR(36) NOT NULL REFERENCES d_t2 ( functionalId ),
  f_t3 VARCHAR(36) NOT NULL REFERENCES d_t3 ( functionalId ),
  f_t4 VARCHAR(36) NOT NULL REFERENCES d_t4 ( functionalId ),
  f_t5 VARCHAR(36) NOT NULL REFERENCES d_t5 ( functionalId ),
  f_t6 VARCHAR(36) NOT NULL REFERENCES d_t6 ( functionalId ),
  f_t7 VARCHAR(36) NOT NULL REFERENCES d_t7 ( functionalId ),
  f_t8 VARCHAR(36) NOT NULL REFERENCES d_t8 ( functionalId ),
  f_t9 VARCHAR(36) NOT NULL REFERENCES d_t9 ( functionalId ),
  f_t10 VARCHAR(36) NOT NULL REFERENCES d_t10 ( functionalId ),
  functionalId VARCHAR(36) NOT NULL,
  PRIMARY KEY (functionalId),
);

PARTITION TABLE d_t1 ON COLUMN functionalId;
PARTITION TABLE d_t2 ON COLUMN functionalId;
PARTITION TABLE d_t3 ON COLUMN functionalId;
PARTITION TABLE d_t4 ON COLUMN functionalId;
PARTITION TABLE d_t5 ON COLUMN functionalId;
PARTITION TABLE d_t6 ON COLUMN functionalId;
PARTITION TABLE d_t7 ON COLUMN functionalId;
PARTITION TABLE d_t8 ON COLUMN functionalId;
PARTITION TABLE d_t9 ON COLUMN functionalId;
PARTITION TABLE d_t10 ON COLUMN functionalId;
PARTITION TABLE facts ON COLUMN functionalId;

CREATE PROCEDURE FROM CLASS InsertDimension1;
CREATE PROCEDURE FROM CLASS InsertDimension2;
CREATE PROCEDURE FROM CLASS InsertDimension3;
CREATE PROCEDURE FROM CLASS InsertDimension4;
CREATE PROCEDURE FROM CLASS InsertDimension5;
CREATE PROCEDURE FROM CLASS InsertDimension6;
CREATE PROCEDURE FROM CLASS InsertDimension7;
CREATE PROCEDURE FROM CLASS InsertDimension8;
CREATE PROCEDURE FROM CLASS InsertDimension9;
CREATE PROCEDURE FROM CLASS InsertDimension10;

CREATE PROCEDURE FROM CLASS InsertFact;

PARTITION PROCEDURE InsertDimension1 ON TABLE d_t1 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension2 ON TABLE d_t2 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension3 ON TABLE d_t3 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension4 ON TABLE d_t4 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension5 ON TABLE d_t5 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension6 ON TABLE d_t6 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension7 ON TABLE d_t7 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension8 ON TABLE d_t8 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension9 ON TABLE d_t9 COLUMN functionalId PARAMETER 2;
PARTITION PROCEDURE InsertDimension10 ON TABLE d_t10 COLUMN functionalId PARAMETER 2;

PARTITION PROCEDURE InsertFact ON TABLE facts COLUMN functionalId PARAMETER 1;