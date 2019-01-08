# Executables with warehouse automation assets

### *Controls:*
 - Use (aswd) for (left back forward right) controls
 - Use (u / j) for lifting / lowering the load
 - Use (space) to drop the current load
 - Use (+ / -) for camera zoom in/out
 - Use ([ \ ]) for camera pan left/right
 - Use \ for toggling to sky view camera

## *ForkliftFrenzy_humanVsCPU*
 - In this executable, we have BasicForklifts, BasicShelves, BasicPallets
 - A basic controller script, not considering any real motion of forklifts
 - Unity3D NavMesh system plus a little hand-written code for forklift automation
 - A simple score controller for keeping track of average delivery time
 - The number of deliveries and collisions is displayed
 - The destination box for a given load becomes identifiable when the pallet is picked up

## *NiceForkliftManyPallets*
 - This executable has objects designed using Unity's ProBuilder and ProGrid plugins
 - Here, we have some different code for load manipulation due to the existence of forks
 - We have many pallets here that are fun to crash into (they respawn when they fall off the edge)
 - The destination box appears only when you pick up a pallet
