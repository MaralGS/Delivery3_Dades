<?php
include 'database_Connection.php';

$connection = ConnectToDatabase();

// Secure Input
$QueryWithFilter = $_POST["QueryFilter"];
$modifiedString = stripslashes($QueryWithFilter);
$QueryString = mysqli_real_escape_string($connection, $modifiedString);

// Attempt to execute the query
$sqlData = $QueryString;

if ($queryResult = mysqli_query($connection, $sqlData)) {

    // Fetch results row by row

    while ($row = mysqli_fetch_assoc($queryResult)) {

        // Process each row as needed
        echo $row["Type"] . "," . $row["PositionX"] . "," . $row["PositionY"] . "," . $row["PositionZ"] . "," ."\n";
    }
    
    // Free the result set
    mysqli_free_result($queryResult);
} else {
    echo "ERROR: Could not execute $sqlData. " . mysqli_error($connection);
}

CloseConToDatabase($connection);

?>