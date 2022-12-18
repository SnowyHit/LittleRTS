# LittleRTS
little RTS game base made with unity , Made in 2020.3.42f1
Uses TextMeshPro for better text.

#Videos



https://user-images.githubusercontent.com/25795916/208319243-7f9f7d2a-03d4-4f89-99ae-d940eaf59696.mp4




https://user-images.githubusercontent.com/25795916/208319130-73df700b-50b3-43b7-9586-a054da156eae.mp4




#Project Overview.

Generally i tried to make best use of design patterns , and get something together that can be playable in a weekend.

-Grid System.
-Camera System.
-Building Placement.
-Agents.


#Grid System

First off we need some grids to handle all the game logic on top of it. For that i used basic 32x32 tiles and created a 30x30 grid on my GridMapManager.Every grid should have a weight(for traversing) , and occupation(to take a log for whats on it) , and also location and other simple stuff.

After that i created the simple functions to handle the grids such as highlighting them , selecting grids , selecting every grid with given 2 grid points.

One last function is the pathfind Function. So every route finding operation could take its part on the gripmap manager function. For the algorithm i choosed A* with weights. Thats because we can sacrifice some accuracy for 5x performance on searching the grid. You can find the algorithm on the GridMapManager Script.

There's other functionality such as finding closest unoccpied grids (Used for mostly agent movement avoidance). and other simple information getters.

#Camera system

Movement of the camera can be done by both WASD , and moving the cursor on the sides of your screen. Also zooming in and out.
Not much to talk about, classic moving with speed*Time.DeltaTime. See CameraManager script.

#Building Placement

After getting the grids ready , placing a building is only instantiating a prefab and occupy grids with right values. But the hard part was actually thinking of a way that scales , with different buildings , different functionality.
The way i came up with was creating a Master "Building" script , the other buildings going to inherit from. But the only problem i have is when we are selecting the building , i have to get the type , cast the building to a type i want , and do functions after that. That part is a bit sloppy but for now its the best i can think of. Needs refactoring for better system.

#Agents

We have a moveabe class called agents , like buildings the agents is a base class for units that can move. its spawned the same way as buildings , only difference is when moving , the occupying grid shifts with it , updating the grids.

And for spice , there's soldier class , that can attack buildings or other agents. But the same thing i talked about for buildings apply here , there can be better structure for inheritence on these systems.

