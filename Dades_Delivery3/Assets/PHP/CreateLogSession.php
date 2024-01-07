<?php
include 'database_Connection.php';

$connection = ConnectToDatabase();

$SessionDate = $_POST["DateTime"];
$UserID = $_POST["UserID"];

// Attempt Insert a row query execution
$sqlData = "INSERT INTO SessionData(playerID,SessionStart) VALUES ('$UserID','$SessionDate')";

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