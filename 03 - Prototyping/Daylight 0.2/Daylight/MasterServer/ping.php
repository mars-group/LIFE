<?php
# ping.php
# Note a client checking in in the database

header('Content-type: text/plain');

# Get info from client
$port = filter_input(INPUT_GET, 'port', FILTER_VALIDATE_INT);
if($port == "") {
	echo "No port!";
	exit;
}
$ip = $_SERVER['REMOTE_ADDR'];

# Open our database
$dbh = sqlite_open("bootstrap.sqlite");

# Make sure we have a table
if(0) {
	sqlite_query($dbh, "CREATE TABLE nodes (id INTEGER PRIMARY KEY, ip VARCHAR(255), port INTEGER);");
}

# Insert new row
sqlite_query($dbh, "INSERT INTO nodes (ip, port) VALUES ('$ip', '$port');");
echo "Registered '$ip $port'\n";

# Get host count and oldest host
$result = sqlite_query($dbh, "SELECT min(id) AS oldest, count(id) AS total FROM nodes;");

# If we have too many hosts, remove oldest
$row = sqlite_fetch_array($result, SQLITE_ASSOC);
if($row['total'] > 10) {
	sqlite_query($dbh, "DELETE FROM nodes WHERE id = '" . $row['oldest'] . "';");
	echo "Removed older.\n";
}

# Close it up
sqlite_close($dbh);
?>