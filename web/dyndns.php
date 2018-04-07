<?php

// this file is obsolete as we now use NodeJS to handles updates

if (isset($_GET['ip'])) {
	require("config.php");
	$db = new mysqli(MYSQL_HOST, MYSQL_USER, MYSQL_PASSWORD, MYSQL_DATABASE);
	$q = $db->prepare("UPDATE piip SET ip = ?");
	if ($q) {
		$q->bind_param('s', $_GET['ip']);
		$q->execute();
		$q->close();
		
	} else {
		var_dump($db->error);
	}
}