# MLAgentsAtSpa
Unity ML Agent Racing Car learns to Drive at Spa !

#### Video Demo:  https://www.youtube.com/watch?v=xxxxxxx
### Description:

This is an Unity ML- Agents Reinforcement Learning Project.  It emulates a car racing around a long race track with inclines and slopes.  It uses the same pronicpels as Code Monkeys ML Agent Race Car emulation : https://www.youtube.com/c/CodeMonkeyUnity 

However I wanted to train against more challenging longer circuits with slopes and inclines (c.f. The challenging Nurburgring and Spa Tracks, as opposed to the tediously flat Silverstone and modern circuits)  


* * *
![ScreenShot](RaceScene1.PNG)
* * *
### Race Track Generation 

The Race Track has been moddled in Blender. A road segment with Barriers was modelled, and then an Array Modifer and a Curver Modifer applied to generate the whole Circuit. The .FBX modle was then imported into UNity, and a Mesh collider applied. Seperate models for the Race Track mesh, and the Barreirs Mesh were loaded into Unity, to enable the Barriers render Mesh Object to be disabled, and have its own specific mesh Collider system. 

* * *
![ScreenShot](BaseRaceTrack.PNG)
* * *
Unity Terrain, Trees and building feature models were added for some interesting track side scenery

As per Code Monkeys ML Agent Race Car Training, I used a Check Point System.  I put in a total of 70x Check Point Trigger Colliders, to monitor progress.
* * *
![ScreenShot](CheckPointSystem.PNG)
* * *

### Race Car Agent Configuration 
A basic Race Car was modelled in Blender, an imported into Unity. Wheel Collider dynamics were employed. 

- Rear wheel (collider) propulsion torque
- Front wheel (collider) steer
This provides lots of under steer and sliding dynamics![image](https://user-images.githubusercontent.com/2668431/182033651-dbe777e5-6fd9-4709-9b48-8f219f9eda63.png)


The Race Car Agent employs a 3D Ray Cast Sensor, which is configured to intercept the Race Track Barriers. 
![ScreenShot](RaceCarColliderss.PNG)

I added three Scalar Observations:
- Car Speed
- Race Track Incline (Slope of the Rcae Track)
- Race Car to Track Alignment:  This is a modified Dot Product of the Car Forward vector to a LERPED direction corresponding to the two Check Point Colliders the Car is between. 

The Actions were simply to Drive the Car through the Wheel Collder Dynamnics.  Two Action Branches: 

- Branch 1: NOOP, Forward Torque, Braking/Reverse Torque
- Branch 2: NOOP, Steer Right, Steer Left

## Reward Profile
A Positive Reward if the Car reaches the Goal Check Point. A negative Reward if the Car runs out of Time in Seconds. And a small penalty per Barrier Collision:
 -  +1.0 – 0.5* (LapTime / MaxLapTime)  -  Upon Successful Lap/ Final Check point (End Episode) 
-   -1.0  if the agent runs exceeds MaxLapTime (End Episode) 
-   -0.025  per barrier collision



### ML Agent Training



![ScreenShot](TrainingGrowth.PNG)



* * *

### Acknowledgements

- Blender: 3D Modelling :   https://www.blender.org/
- Unity: Game Development : https://unity.com/
- Code Monkey Unity and ML –Agent Tutorials:  https://www.youtube.com/c/CodeMonkeyUnity
- Imphenzia: 3D Blender Modelling : https://www.youtube.com/c/Imphenzia
- Sound Track: Fleetwood Mac: “ The Chain “ : https://www.fleetwoodmac.com
