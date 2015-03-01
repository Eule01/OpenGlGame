
2015.03.01	FEATURE		Created a MenuForm in which maps can be saved and loaded (minimal implementation so far).
2015.02.26	FEATURE		The turret towers will now rotate at a maximum speed.
2015.02.24	IMPROVEMENT	Made game object locations and direction Vector3 and tydied up.
2015.02.23	IMPROVEMENT	The turrets can now turn their tower. For test purposes the tower will point at the player once he gets within 10m.
2015.02.22	IMPROVEMENT	The map can now be changed by holding CTRL and clicking on a tile (this is a very basic implementation).
2015.02.22	IMPROVEMENT	A material can now be selected in the creative HUD.
2015.02.22	IMPROVEMENT	Majorly improved the map render by using MipMap in the texture array.
2015.02.22	IMPROVEMENT	Shifted the test map to be centred around the origine.
2015.02.22	IMPROVEMENT	Added some 32x32 ground textures.
2015.02.22	FEATURE		One can now cycle through different sky boxes using 'k'.
2015.02.22	IMPROVEMENT	Fixed the SkyBox, now seams are visiable now.
2015.02.21	IMPROVEMENT	Tidyed SceneManager and RenderLayers.
2015.02.19	FEATURE		Added a SceneManager.
2015.02.19	FEATURE		Added a RenderLayerMapDrawArrays which works.
2015.02.18	FEATURE		Added a RenderLayerMap based on MultiDrawElementsIndirect (min OpenGl 4.3).
2015.02.11	CHANGE		Made sure game engine status does not report to the console.
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