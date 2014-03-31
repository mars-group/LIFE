<?php
# list.php
# Provide clients with a list of currently 
# operational Daylight nodes to bootstrap from

header('Content-type: text/plain');

# We need this for testing
echo "127.0.0.1 8810\n";

# Open our database
$dbh = sqlite_open("bootstrap.sqlite");

# Get all hosts
$result = sqlite_query($dbh, "SELECT ip, port FROM nodes");

# Dump them
while ($row = sqlite_fetch_array($result, SQLITE_ASSOC)) {
    echo $row['ip'] . " " . $row['port'] . "\n";
}

# Close it up
sqlite_close($dbh);
?>