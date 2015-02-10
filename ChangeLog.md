
2015.02.10	IMPROVEMENT Renamed ObjMesh into ObjGroup which is a much better description.
2015.02.10	IMPROVEMENT ObjMesh now has a Location, Orientation and Scale.
2015.02.09	IMPROVEMENT The shutdown somewhat, still not perfect.
2015.02.09	IMPROVEMENT The shutdown somewhat, still not perfect.
2015.02.08	IMPROVEMENT Models loader now loads multiple ObjObjects into a ObjMesh and loads the Materials via the .mtl file.
2015.02.08	FEATURE		Models can now be loaded from file (Wavefront (.obj). This is not perfect yet.
2015.02.08	FEATURE		The light direction now rotates around (this can be toggled on/off with the r key).
2015.02.07	FEATURE		Added a form showing the key bindings.
2015.02.06	IMPROVEMENT Tile textue bitmap will now be loaded if the correct image file is present.
2015.02.06	FEATURE		Added lighting direction.
2015.02.06	BUGFIX		The tile normals were wrong.
2015.02.06	CHANGE		Swapped coordinate system, ground plane is now the x z plane.
2015.02.06	FEATURE		Added basic skyBox.
2015.02.03	FEATURE		Started implementing a MaterialManager.
2015.02.03	IMPROVEMENT	Reorganized the project folder to make it easier to find things.
2015.02.02	IMPROVEMENT	The console window is now placed on the secondary screen.
2015.02.02	IMPROVEMENT	Implemented an ObjectPrimitives class that helps create cubes and squares.
2015.02.01	IMPROVEMENT	Implemented a semi transparent bitmap for the HUD Panel.
2015.02.01	IMPROVEMENT	Added keyBindings class. The keyboard controlles can now easily be remapped.
2015.01.31	IMPROVEMENT	All console messages are now broadcasted via the GameCore.TheGameCore.RaiseMessage() method.