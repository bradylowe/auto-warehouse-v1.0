# Pallet assets

## *Prefabs*
 - BasicPallet is a cube with a rigidbody and controller script
 - Pallet is a slightly realistic empty pallet with the rigidbody and script
 - FullPallet is a slightly realistic full pallet with rb and script
 - DestinationBox is something that every pallet needs (for delivery)
 - Lumber is just two 4x4 like objects that the pallet is built with

## The pallet controller script
The pallet controller is responsible for spawning the pallet in a designated "SpawnZone". The SpawnZone is a cube that defines the volume in which pallets should be spawned. Create and name this object in your scene, and after placing it, remove the mesh collider from the object and turn off rendering.

The shelves in your scene should have their long axis as their z axis and should have the "Shelf" tag so that the pallet controller can spawn the destination boxes on the shelves.
