<?php
include 'database_Connection.php';

$connection = ConnectToDatabase();

$QueryWithFilter = $_POST["QueryFilter"];

// Attempt Insert a row query execution
$sqlData = $QueryWithFilter;

if(mysqli_query($connection, $sqlData))
{

    //Returns All the data with the proper filters
    echo $sqlData;
    
} 
else
{
    echo "ERROR: Could not able to execute $sqlData. " . mysqli_error($connection);
}

CloseConToDatabase($connection);

?>