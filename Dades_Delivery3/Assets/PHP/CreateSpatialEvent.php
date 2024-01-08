<?php
include 'database_Connection.php';

$connection = ConnectToDatabase();

$levelEventID = $_POST["LevelEventsID"];
$Type = $_POST["Type"];
$Level = $_POST["Level"];
$PositionX = $_POST["PositionX"];
$PositionY = $_POST["PositionY"];
$PositionZ = $_POST["PositionZ"];
$UserID = $_POST["UserID"];
$SessionID = $_POST["SessionID"];
$EventDate = $_POST["DateTime"];
$Step = $_POST["Step"];


// Attempt Insert a row query execution
$sqlData = "INSERT INTO SpatialEvents(levelEvent_id,Type,Level,PositionX,PositionY,PositionZ,player_id,session_id,dateEvent,step) VALUES ('$levelEventID','$Type','$Level','$PositionX','$PositionY','$PositionZ','$UserID','$SessionID','$EventDate','$Step')";

if(mysqli_query($connection, $sqlData))
{
    echo $sqlData;
    //Returns the value generated for an AUTO_INCREMENT column by the last query
    $lastlevelEventID = mysqli_insert_id($connection);
	 
	echo $lastlevelEventID;
} 
else
{
    echo "ERROR: Could not able to execute $sqlData. " . mysqli_error($connection);
}

CloseConToDatabase($connection);

?>