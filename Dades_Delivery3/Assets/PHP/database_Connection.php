<?php
function ConnectToDatabase()
{
$dbhost = "localhost";
$dbuser = "polrb";
$dbpass = "zNn2xSkf8AcA";
$dbname = "polrb";
$conn = new mysqli($dbhost, $dbuser, $dbpass,$dbname) or die("Connect failed: %s\n". $conn -> error);
return $conn;
}

function CloseConToDatabase($conn)
{
$conn -> close();
}
?>