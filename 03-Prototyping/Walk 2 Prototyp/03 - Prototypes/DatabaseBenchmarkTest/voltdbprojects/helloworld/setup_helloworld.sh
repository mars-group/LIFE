#!/bin/sh

echo "Compiling Helloworld VoltDB Schema..."

echo "Setting classpath..."
CLASSPATH="./:../voltdb-3.3.0.1/lib/*:../voltdb-3.3.0.1/voltdb/*"
export CLASSPATH
echo "...done"
echo "compiling Insert Procedure..."

javac Insert.java
echo "...done"

echo "compiling Select Procedure..."
javac Select.java
echo "...done"

echo "compiling Client..."
javac Client.java
echo "...done"

echo "compiling db catalog..."
../voltdb-3.3.0.1/bin/voltdb compile --classpath="./" -o helloworld.jar helloworld.sql 
echo "...done"

echo "creating catalog..."
../voltdb-3.3.0.1/bin/voltdb create catalog helloworld.jar
echo "...done."
