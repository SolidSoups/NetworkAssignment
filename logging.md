# Entry 1
First commit, basic project setup. Starting out i have these couple things:
- Basic player prefab. Player can move around with WASD or arrow keys and can shoot with mouse button. 
- Player Spawner. Players get spawned by the PlayerSpawner when joining, so if i want to tweak the spawning behahaviour i can do that in this component.
- NetworkManagerEditorWindow. A window that allows me to easily start host and client sessions.

My next steps are to interpolate movement as its getting hard on the eyesto see them stutter. Will try to get some client side prediction and reconciliation.

# Entry 2
Succesfully implemented interpolation for player movement and rotation. It's a little hard to test rn because when the unity editor is not in focus mode, the performance takes a drastic hit. BUT, I did a small check for if we were a client and moved the player and rotated so that a host could see what it would look like AND IT DEFINITELY INTERPOLATES. Anyways, im putting my focus on interpolated the bullets now...

# Entry 3
Now bullets are interpolated and i realized that there is a better way to interpolate, so i will be revising the player implementation of interpolation.
