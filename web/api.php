<?php

// this file is obsolete as we now use NodeJS to handles updates

if (isset($_GET['spot-id']) && isset($_GET['available'])) {
	require("config.php");
	$db = new mysqli(MYSQL_HOST, MYSQL_USER, MYSQL_PASSWORD, MYSQL_DATABASE);
	$q = $db->prepare("SELECT id, available FROM spot_availability WHERE id = ?");
	$q->bind_param('i', $_GET['spot-id']);
	$q->execute();
	$q->bind_result($id, $availability);
	$q->fetch();
	$q->close();

	if ($id != NULL) {
		$q = $db->prepare("UPDATE spot_availability SET available = ? WHERE id = ?");
		$q->bind_param('ii', $_GET['available'], $id);
		$q->execute();
		$q->close();
	}
}