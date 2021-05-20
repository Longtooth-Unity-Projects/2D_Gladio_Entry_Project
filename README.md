# 2D_Gladio_Entry_Project

A project that was required as part of an interview process.


### Desing Notes


Design Notes
1.	General
a.	Controls:
i.	WASD to move camera
ii.	Mousewheel to scroll camera
iii.	L to toggle the resource labels over structures
iv.	If you uncomment the debug methods, c will place a consumer and p will place a producer.
b.	I tried to design the project with future expansion in mind, so given the requested functionality, some implementations may seem like overkill.
c.	consumers also find a new route when producers are out of resources
2.	Camera
a.	I just made a basic movement script for the camera.
b.	As this is a prototype, I didn’t bother to clamp any values, so you can move the camera 
3.	Grid
a.	Tiles
i.	The tiles are setup so weights can be added in the future.
ii.	Roads and building foundations are walkable.
iii.	Grass is not walkable.
b.	Roads
i.	I chose to build roads from each consumer to each producer so they can possibly be used in the future.
4.	Pathfinding
a.	I used a separate algorithm for laying the roads than I did for pathfinding, mainly for aesthetic reasons.
b.	I removed diagonal tiles from pathfinding, but they can easily be added back.
c.	Currently, there are no weights for the tiles, they are just walkable or not walkable. However, the infrastructure is in place so that weights can be added and the algorithm will use them.
5.	Transport
a.	Aborting missions
i.	Currently, the transports register with the producer when starting the route. If the OutOfResources Event is invoked, the transports abort their mission. However, the only thing that causes that event to fire at this point is if the producer is manually disabled in the editor.
