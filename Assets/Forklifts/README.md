# Forklift assets

## *Prefabs*
 - BasicForklift = a cube with Rigidbody, NavMeshAgent, and controller script
 - ForkliftBody =  just a cube with two spheres as front wheels (made with ProBuilder)
 - Forks = forks with a chassis (made with ProBuilder)
 - ForksFlat = just the forks (no chassis) (made with ProBuilder)
 - DriveWheel = a capsule turned on its side
 - FullForkliftAssembly = ForkliftBody + Forks + DriveWheel
 - ScoreText = some green UI text 
 - PenaltyText = some red UI text
 - StatsBox = some simple UI text

## *Materials*
 - Aluminum for the forks (just color)
 - ForksMat (for friction coefficients)
 - Rubber and RubberMat for the DriveWheel color and friction

## *Scripts*
 - DriveForklift takes in user input IF humanControlled and uses it to drive a forklift with a Rigidbody and NavMeshAgent (or it computes the next step and takes it)
 - FollowForklift should be attached to the main camera, and you must modify the name of the object to follow in the code to match the name of the player. This script looks for camera control input.
 - PenaltyTextController should be attached to the PenaltyText
 - ScoreTextController should be attached to the ScoreText
 - TextController could replace both of the above controllers
 - ScoreController should be attached to the StatsBox and needs a reference to all forklifts that need tracking
