<?
	//Este script respondera sólo con texto
	header("Content-Type:text/plain");

	//Recopilar los datos del usuario
	$id = array_key_exists('id', $_GET) ? htmlspecialchars($_GET['id']) : NULL;
	$object_name = array_key_exists('object_name', $_GET) ? htmlspecialchars($_GET['object_name']) : NULL;
	$pos_x = array_key_exists('pos_x', $_GET) ? htmlspecialchars($_GET['pos_x']) : NULL;
	$pos_y = array_key_exists('pos_y', $_GET) ? htmlspecialchars($_GET['pos_y']) : NULL;
	$pos_z = array_key_exists('pos_z', $_GET) ? htmlspecialchars($_GET['pos_z']) : NULL;
	$rot_x = array_key_exists('rot_x', $_GET) ? htmlspecialchars($_GET['rot_x']) : NULL;
	$rot_y = array_key_exists('rot_y', $_GET) ? htmlspecialchars($_GET['rot_y']) : NULL;
	$rot_z = array_key_exists('rot_z', $_GET) ? htmlspecialchars($_GET['rot_z']) : NULL;
	$current_state_hash= array_key_exists('current_state_hash', $_GET) ? htmlspecialchars($_GET['current_state_hash']) : NULL;
	$normalized_time = array_key_exists('normalized_time', $_GET) ? htmlspecialchars($_GET['normalized_time']) : NULL;
	$snap_date = array_key_exists('snap_date', $_GET) ? htmlspecialchars($_GET['snap_date']) : NULL;
	$date_milliseconds = array_key_exists('date_milliseconds', $_GET) ? htmlspecialchars($_GET['date_milliseconds']) : NULL;
	$trigger_hash = array_key_exists('trigger_hash', $_GET) ? htmlspecialchars($_GET['trigger_hash']) : NULL;
	
	$disconnect = array_key_exists('disconnect', $_GET) ? htmlspecialchars($_GET['disconnect']) : 'false';
	$ismaster = array_key_exists('ismaster', $_GET) ? htmlspecialchars($_GET['ismaster']) : 'false';

	// Create connection
	$conexion = mysqli_connect("localhost","bondzuco_winkapp","winkreality","bondzuco_wink");
	
	
	// Check connection
	if (mysqli_connect_error($conexion))
	{
		$msj = "Failed to connect to MySQL: " . mysqli_connect_error($conexion);
		echo("error: $msj");
		exit();
	}
	
	//Buscar el id existente en la base de datos
	$search_query = "SELECT * FROM `unity_animation_syncro` WHERE id = '$id'";
	$result = mysqli_query($conexion, $search_query);
	
	$query = '';
	if($result == false){ //Error
		return_nok($search_query);
		
	}else if($result->num_rows == 0){ //La animación no existe 
	
		//Crear query para agregarlo
		$query = getAddObjectQuery();
		
		//Enviar query
		hacerQueryYMostrarRespuesta($query);
		
	}else if($ismaster == 'true'){  //La animación sí existe pero la conexion viene del master
		
		//Crear query para agregarlo
		$query = getUpdateMasterQuery();
		
		//Enviar query
		hacerQueryYMostrarRespuesta($query);
		
	}else{ //existe y hay usuarios conectados
	
		//Obtener número de usuarios conectados
		$row = mysqli_fetch_array($result);
		$connected_users = $row['connected_users'];
		
		if($disconnect == 'true'){//se va a desconectar un usuario
		
			$connected_users--;
			
			if($connected_users <= 0){
				$query = getDeleteObjectQuery();
			}else{
				$query = getUpdateUsersQuery();
				
			}
			
			//Enviar query
			hacerQueryYMostrarRespuesta($query);
		
		}else{	//se va a agregar un usuario
		
			$connected_users++;
			
			//Crear query para agregarlo
			$query = getUpdateUsersQuery();
					
			//Enviar query
			$result = hacerQuery($query);

			if( $result == TRUE){	
				actualizarValores($row);
				return_ok($query,"");
			}else{
				$sqlerr = mysql_error($conexion);
				return_nok($query, $sqlerr, "");
			}
			
		}//if-else disconnect==true
		
	}//else numrows == 0
	
	
	//Close connection
	mysqli_close($conexion);
	
	
	
	
	
	/*************************************
				Funciones Queries
	*************************************/
	
	function actualizarValores($row){
		//Vincular variables globales
		global $pos_x;
		global $pos_y;
		global $pos_z;
		global $rot_x;
		global $rot_y;
		global $rot_z;
		global $current_state_hash;
		global $normalized_time;
		global $snap_date;
		global $date_milliseconds;
		global $trigger_hash;
	
		$pos_x = $row['pos_x'];
		$pos_y = $row['pos_y'];
		$pos_z = $row['pos_z'];
		$rot_x = $row['rot_x'];
		$rot_y = $row['rot_y'];
		$rot_z = $row['rot_z'];
		$current_state_hash = $row['current_state_hash'];
		$normalized_time = $row['normalized_time'];
		$snap_date = $row['snap_date'];
		$date_milliseconds = $row['date_milliseconds'];
		$trigger_hash = $row['trigger_hash'];
	}
	
	
	function hacerQuery($query){
		global $conexion;
		return mysqli_query($conexion, $query);
	}
	
	
	function hacerQueryYMostrarRespuesta($query){
		global $conexion;
		$result = mysqli_query($conexion, $query);
		
		//responder al usuario
		if( $result == TRUE){	
			return_ok($query,"");
		}else{
			$sqlerr = mysql_error($conexion);
			return_nok($query, $sqlerr, "");
		}
	}
	
	function getAddObjectQuery(){
		//Vincular variables globales
		global $id;
		global $object_name;
		global $pos_x;
		global $pos_y;
		global $pos_z;
		global $rot_x;
		global $rot_y;
		global $rot_z;
		global $current_state_hash;
		global $normalized_time;
		global $snap_date;
		global $date_milliseconds;
		global $trigger_hash;
		
		//El primer usuario en conectarse pone el tiempo de inicio
		$snap_date = $snap_date;
		
		//Crear Query
		$query = "INSERT INTO unity_animation_syncro ";
		$query .= "(id, object_name, pos_x, pos_y, pos_z, rot_x, rot_y, rot_z, current_state_hash, normalized_time, snap_date, date_milliseconds, trigger_hash, connected_users) ";
		$query .= "VALUES ";
		$query .= "('$id', '$object_name', '$pos_x', '$pos_y', '$pos_z', '$rot_x', '$rot_y', '$rot_z', '$current_state_hash', '$normalized_time', '$snap_date', '$date_milliseconds','$trigger_hash' , '1')";
		return $query;
	}
	
	function getDeleteObjectQuery(){
		//Vincular variables globales
		global $id;
		
		//Crear Query
		$query = "DELETE FROM `bondzuco_wink`.`unity_animation_syncro` WHERE `unity_animation_syncro`.`id` = $id;";
		return $query;
	}
	
	function getUpdateUsersQuery(){
		//Vincular variables globales
		global $id;
		global $connected_users;
		
		//Crear Query
		$query = "UPDATE  unity_animation_syncro SET ";
		$query .= "connected_users = '$connected_users'";
		$query .= " WHERE  id = '$id'";
		
		return $query;	
	}
	
	function getUpdateMasterQuery(){
		//Vincular variables globales
		global $id;
		global $object_name;
		global $pos_x;
		global $pos_y;
		global $pos_z;
		global $rot_x;
		global $rot_y;
		global $rot_z;
		global $current_state_hash;
		global $normalized_time;
		global $snap_date;
		global $date_milliseconds;
		global $trigger_hash;
		
		//Crear Query
		$query = "UPDATE  unity_animation_syncro SET ";
		$query .= "pos_x = '$pos_x',";
		$query .= "pos_y = '$pos_y',";
		$query .= "pos_z = '$pos_z',";
		$query .= "rot_x = '$rot_x',";
		$query .= "rot_y = '$rot_y',";
		$query .= "rot_z = '$rot_z',";
		$query .= "current_state_hash = '$current_state_hash',";
		$query .= "normalized_time = '$normalized_time',";
		$query .= "snap_date = '$snap_date',";
		$query .= "date_milliseconds = '$date_milliseconds',";
		$query .= "trigger_hash = '$trigger_hash'";
		$query .= " WHERE  id = '$id'";
		
		return $query;
	}
	
	
	/*************************************
				Funciones OK/NOK
	*************************************/
	
	//Función en caso de éxito
	function return_ok($last_query='',$log=''){
		global $connected_users;
		global $pos_x;
		global $pos_y;
		global $pos_z;
		global $rot_x;
		global $rot_y;
		global $rot_z;
		global $current_state_hash;
		global $normalized_time;
		global $snap_date;
		global $date_milliseconds;
		global $trigger_hash;
		global $disconnect;

		//debug
		/*$myObject = array();
		$myObject["status"] = "ok";
		$myObject["lastQuery"] = $last_query;
		$myObject["log"] = $log;
		print_r($myObject);*/
		
		//Sin espacios entre comas
		echo("ok,$pos_x,$pos_y,$pos_z,$rot_x,$rot_y,$rot_z,$current_state_hash,$normalized_time,$snap_date,$date_milliseconds,$trigger_hash");
	}
	
	//Función en caso de error
	function return_nok($last_query='', $sql_error, $log=''){
		
		//Debug
		$myObject = array();
		$myObject["status"] = "error";
		$myObject["error"] = error_get_last();
		$myObject["log"] = $log;
		$myObject["lastQuery"] = $last_query;
		$myObject["sqlError"] = $sql_error;
		
		echo("error,\n");
//		echo($myObject["error"].", LOG: ".$log.", LAST QUERY: ".$last_query.", SQLerr: ".$sql_error);
		print_r($myObject);
	}
?>