<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <title>ParkUNT</title>

	<link href="https://fonts.googleapis.com/css?family=Bitter:400,700,400italic" rel="stylesheet" type="text/css">

    <link href="/assets/css/bootstrap.min.css" rel="stylesheet" media="screen">
    <link href="/assets/css/font-awesome.min.css" rel="stylesheet" media="screen">
    <link href="/assets/css/style.css" rel="stylesheet" media="screen">
    
    <link rel="icon" type="image/x-icon" href="/favicon.ico">  
</head>

<body>
<?php
	require("config.php");
	$db = new mysqli(MYSQL_HOST, MYSQL_USER, MYSQL_PASSWORD, MYSQL_DATABASE);
	$q = $db->prepare("SELECT id, available, plus_code FROM spot_availability");
	$q->bind_param('i', $_GET['spot-id']);
	$q->execute();
	$r = $q->get_result();
	$availability = array();
	while ($result = $r->fetch_array(MYSQLI_ASSOC)) {
		$availability[$result['id']] = $result['available'];
		$plus_code[$result['id']] = $result['plus_code'];
	}
	//$q->fetch();
	$q->close();
	
?>
<div class="container" style="width: 100%;">

		<div class="jumbotron pricing-tables-jumbotron" style="padding-bottom: 0px;">
		  	<h2>UNT Discovery Park - Parking Availability</h2>
		  	<h3>Available Spots: <span id="available-spots">0</span></h3>
			<button type="button" class="btn btn-success" onClick="javascript:take_me_to_available_spot();">Take Me to an Available Spot</button>
			<p style="margin-top: 10px;"><small>ParkUNT is a system for managing and streamlining car parking lots with high utilisation, such as on large campuses. Read more on <a href="https://github.com/sebastian-king/ParkUNT"><i class="fa fa-github"></i> GitHub</a>.</small></p>
		</div>
		<center></center>
		<div class="row attached">
			
			<?php
			$i;
			for ($i = 1; $i <= 5; $i++) {
				$spot = $i;
				$availability = $availability[$i];
			?>
			<div id="spot-<?php echo $spot; ?>" class="col-sm-2 col-md-2 <?php if ($i == 1) { echo 'col-lg-offset-1'; } ?>">
				<div class="plan <?php if ($available) { echo 'available'; } else { echo 'unavailable'; } ?>">
					<div class="plan-title">
						<h3>Spot #<?php echo $spot; ?></h3>
						<span>UNT DISCOVERY PARK</span>
					</div>
					<div class="plan-list">
						<div class="divide">
							<div class="icon">
								<i class="fa fa-<?php if ($available) { echo 'check'; } else { echo 'times'; } ?> fa-3x"></i>
							</div>
						</div>
						<ul class="item-list status">
                   			<li>CURRENTLY</li>
	                   		<li><strong><?php if ($available) { echo 'AVAILABLE'; } else { echo 'UNAVAILABLE'; } ?></strong></li>
	              		</ul>
	              		<a href="https://www.google.com/maps/search/<?php echo urlencode($plus_code[$spot]); ?>?z=20" class="btn btn-success directions">Directions</a>
              		</div>
				</div>
			</div>
			<?php
			}
			?>

		</div>
		
		<div class="row attached">
			
			<?php
			for ($i = 6; $i <= 10; $i++) {
				$spot = $i;
				$availability = $availability[$i];
			?>
			<div id="spot-<?php echo $spot; ?>" class="col-sm-2 col-md-2 <?php if ($i == 6) { echo 'col-lg-offset-1'; } ?>">
				<div class="plan <?php if ($available) { echo 'available'; } else { echo 'unavailable'; } ?>">
					<div class="plan-title">
						<h3>Spot #<?php echo $spot; ?></h3>
						<span>UNT DISCOVERY PARK</span>
					</div>
					<div class="plan-list">
						<div class="divide">
							<div class="icon">
								<i class="fa fa-<?php if ($available) { echo 'check'; } else { echo 'times'; } ?> fa-3x"></i>
							</div>
						</div>
						<ul class="item-list status">
                   			<li>CURRENTLY</li>
	                   		<li><strong><?php if ($available) { echo 'AVAILABLE'; } else { echo 'UNAVAILABLE'; } ?></strong></li>
	              		</ul>
	              		<a href="https://www.google.com/maps/search/<?php echo urlencode($plus_code[$spot]); ?>?z=20" class="btn btn-success directions">Directions</a>
              		</div>
				</div>
			</div>
			<?php
			}
			?>

		</div>
		
	</div>
		
   	<script src="/assets/js/jquery-2.0.3.min.js"></script>
   	<script src="/assets/js/bootstrap.min.js"></script>
   	
	<script>
		$(document).ready(function() {
			return;
			var i;
			for (i = 0; i <= 6; i++) {
				$this = $("#spot-" + i + " > .plan");
				if (i % 2 == 0) {
					$this.addClass("available");
					$this.find("div.icon > i").removeClass("fa-question").addClass("fa-check");
					$this.find(".status > li:last-child > strong").text("AVAILABLE");
				} else {
					$this.addClass("unavailable");
					$this.find("div.icon > i").removeClass("fa-question").addClass("fa-times");
					$this.find(".status > li:last-child > strong").text("UNAVAILABLE");
				}
			}
		});
	</script>
	
	<script src="/assets/js/ReconnectingWebsocket.js"></script>
	<script language="javascript" type="text/javascript">
		
		function take_me_to_available_spot() {
			var href = $(".plan.available").first().find('.directions');
			if (href.length) {
				window.location.href = href.attr('href');
			} else {
				alert('There are currently not available spots, sorry.');
			}
		}
		
		var ws_url = "wss://websocket.parkunt.tech";
		var errors = 0;
		function init() {
			init_websocket();
		}
		function init_websocket() {
			websocket = new ReconnectingWebSocket(ws_url, "feed");
			websocket.onopen = function(evt) {
				onOpen(evt)
			};
			websocket.onclose = function(evt) {
				onClose(evt)
			};
			websocket.onmessage = function(evt) {
				onMessage(evt)
			};
			websocket.onerror = function(evt) {
				onError(evt)
			};
		}
		function onOpen(evt) {
			console.log("CONNECTED");
			doSend("Hello");
		}
		function onClose(evt) {
			console.log("DISCONNECTED");
		}
		function onMessage(evt) {
			var received = JSON.parse(evt.data);
			if (received.key == "update") {
				if (typeof received.values == "object") {
					var i;
					for (i in received.values) {
						console.log("SPOT #" + received.values[i][0] + " UPDATED TO: " + received.values[i][1]);
						$this = $("#spot-" + received.values[i][0] + " > .plan");
						if (received.values[i][1] == "1") {
							$this.addClass("available").removeClass("unavailable");
							$this.find("div.icon > i").removeClass("fa-times").addClass("fa-check");
							$this.find(".status > li:last-child > strong").text("AVAILABLE");
						} else {
							$this.addClass("unavailable").removeClass("available");
							$this.find("div.icon > i").removeClass("fa-check").addClass("fa-times");
							$this.find(".status > li:last-child > strong").text("UNAVAILABLE");
						}
						
						$("#available-spots").text($(".plan.available").length);
					}
				}
			}
			console.log(received, typeof received.values);
			//websocket.close();
		}
		function onError(evt) {
			console.log(evt);
		}
		function doSend(message) {
			websocket.send(JSON.stringify(message));
		}
		window.addEventListener("load", init, false);
	</script>
</body>
</html>