#!/bin/sh

echo "Compiling Starschema VoltDB Schema..."

echo "Setting classpath..."
CLASSPATH="./:/opt/voltdb/voltdb/*:/opt/voltdb/lib/*"
export CLASSPATH
echo "...done"

echo "compiling Insert-Procedures..."
javac InsertDimension1.java
javac InsertDimension2.java
javac InsertDimension3.java
javac InsertDimension4.java
javac InsertDimension5.java
javac InsertDimension6.java
javac InsertDimension7.java
javac InsertDimension8.java
javac InsertDimension9.java
javac InsertDimension10.java

javac InsertFact.java
echo "...done"

echo "compiling db catalog..."
voltdb compile --classpath="./" -o starschema.jar voltdb_star_schema.sql 
echo "...done"

echo "creating catalog..."
voltdb create catalog starschema.jar deployment deployment.xml host localhost
echo "...done."
