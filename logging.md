# Entry 1
First commit, basic project setup. Starting out i have these couple things:
- Basic player prefab. Player can move around with WASD or arrow keys and can shoot with mouse button. 
- Player Spawner. Players get spawned by the PlayerSpawner when joining, so if i want to tweak the spawning behahaviour i can do that in this component.
- NetworkManagerEditorWindow. A window that allows me to easily start host and client sessions.

My next steps are to interpolate movement as its getting hard on the eyesto see them stutter. Will try to get some client side prediction and reconciliation.
