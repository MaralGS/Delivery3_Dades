<?php
include 'database_Connection.php';

$connection = ConnectToDatabase();

$SessionDate = $_POST["DateTime"];
$SessionID = $_POST["SessionID"];

// Attempt Insert a row query execution
$sqlData = "UPDATE SessionData SET SessionEnd = '{$SessionDate}' WHERE SessionID = '{$SessionID}'";

if(mysqli_query($connection, $sqlData))
{
    //Returns the value generated for an AUTO_INCREMENT column by the last query
    $lastSessionID = mysqli_insert_id($connection);
	 
	echo $lastSessionID;
} 
else
{
    echo "ERROR: Could not able to execute $sqlData. " . mysqli_error($connection);
}

CloseConToDatabase($connection);

?>