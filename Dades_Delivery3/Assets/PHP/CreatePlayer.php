<?php
include 'database_Connection.php';

$connection = ConnectToDatabase();

$name = $_POST["Name"];
$dateOfCreation = $_POST["DateTime"];

// Attempt create table query execution
$sqlData = "INSERT INTO UserData(Name,CreationDate) VALUES ('$name','$dateOfCreation')";

//CHECK IF THE CONSULT WORKED
if(mysqli_query($connection, $sqlData))
{
	//Returns the value generated for an AUTO_INCREMENT column by the last query
	$lastID = mysqli_insert_id($connection);
	 
	echo $lastID;
	
} else{
    echo "ERROR: Could not able to execute $sqlData. " . mysqli_error($connection);
}

CloseConToDatabase($connection);

?>