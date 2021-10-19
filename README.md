# 2D_Gladio_Entry_Project

A project that was required as part of an interview process.


### Design Notes

1.	General
	1.	Controls:
		1.	WASD to move camera
		2.	Mousewheel to scroll camera
		3.	L to toggle the resource labels over structures
		4.	If you uncomment the debug methods, c will place a consumer and p will place a producer.
	2.	I tried to design the project with future expansion in mind, so given the requested functionality, some implementations may seem like overkill.
	3.	consumers also find a new route when producers are out of resources
2.	Camera
	1.	I just made a basic movement script for the camera.
	2.	As this is a prototype, I didn’t bother to clamp any values, so you can move the camera 
3.	Grid
	1.	Tiles
		1.	The tiles are setup so weights can be added in the future.
		2.	Roads and building foundations are walkable.
		3.	Grass is not walkable.
	2.	Roads
		1.	I chose to build roads from each consumer to each producer so they can possibly be used in the future.
4.	Pathfinding
	1.	I used a separate algorithm for laying the roads than I did for pathfinding, mainly for aesthetic reasons.
	2.	I removed diagonal tiles from pathfinding, but they can easily be added back.
	3.	Currently, there are no weights for the tiles, they are just walkable or not walkable. However, the infrastructure is in place so that weights can be added and the algorithm will use them.
5.	Transport
	1.	Aborting missions
		1.	Currently, the transports register with the producer when starting the route. If the OutOfResources Event is invoked, the transports abort their mission. However, the only thing that causes that event to fire at this point is if the producer is manually disabled in the editor.
