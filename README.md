# MLAgentsAtSpa
Unity ML Agent Racing Car learns to Drive at Spa !

#### Video Demo:   https://www.youtube.com/watch?v=M_zVFtJtCs0
### Description:

This is an Unity ML- Agents Reinforcement Learning Project.  It emulates a car racing around a long race track which includes steep inclines and slopes.  It uses the same Check Point Collider system as suggested in as Code Monkeys ML Agent Race Car emulation : https://www.youtube.com/c/CodeMonkeyUnity 

However I wanted to see if it was possibel to Train an Agent against more challenging longer circuits with slopes and inclines.  To represent driving through the challenging Nurburgring and Spa racing circuits, as opposed to the rather flat Silverstone and modern circuits.   

* * *
![ScreenShot](RaceScene1.PNG)
* * *
### Race Track Generation 

The Race Track has been modelled in Blende: A road segment with Barriers was modelled, and then an Array Modifer and a Curver Modifier applied to generate the whole Circuit. The model was  .FBX exported then imported into Uuity and a Mesh collider applied. Seperate models were used for the Race Track Mesh and another the set for the Barriers Mesh, based upon the same Track Curve. Both Meshes were loaded into Unity, with the Barriers render Mesh Object to be disabled, but with its own specific Mesh Collider system. The Race Car could then detect the Barriers using the 3D Ray sensors. 

* * *
![ScreenShot](BaseRaceTrack.PNG)
* * *
A Unity Terrain, Trees and various building feature models were added for some interesting track side scenery. (Note these are EXCLUDED from the Unity Package provided here, to avoid excessive Git Upload files size.) 

As per Code Monkeys ML Agent Race Car Training, I used a Check Point System.  I put in a total of 70x Check Point Trigger Colliders, to monitor progress.  This was a little tedious and suggests that the method does not scale. It may have been possible to also generate and export these Check Point objects in Blender using a Blender python API script to automate the process.  The Checkpoint has a Script to monitor the Race Car triggering a pass through. The orientation of the Checkpoints are also employed as an Indicator of Race Car to Race Track Alignment. 9See later)  

* * *
![ScreenShot](CheckPointSystem.PNG)
* * *

### Race Car Agent Configuration 
A basic Race Car was modelled in Blender, an imported into Unity. The typical Wheel Collider dynamics were employed against all four wheels. 

- Rear wheel (collider) Propulsion Torque
- Front wheel (collider) Steer

This modelled dynamics provides lots of under steer and sliding dynamics!   It is actully quite challenging to drive around the circuit, avoiding excessive speed, spins and collisions with barriers. After some practice I was able to achieve a lap time in around 150 seconds. 

The Race Car Agent employs a 3D Ray Cast Sensor, which is configured to intercept the Race Track Barriers. 
* * *
![ScreenShot](RaceCarColliders.PNG)
* * *

I added three Scalar Observations:
- Car Speed
- Race Track Incline (Slope of the Race Track to help coordinate the appropriate Engine Troque when going up or down hills) 
- Race Car to Track Alignment:  (+1.0 when facing in the correct direction, Negative when facing in the wrong direction) 

The Race Car alignment is a modified Dot Product of the Car Forward vector and the Race Track Direction.  The Race Track Direction is calculated as a LERPED interpolation between two Check Point Collider Directions which the Car is currently between. The torr being the relative distance between the two Check Points.     I actually used Alignment = 1.0 – 2.5*(1.0-Vector Dot Product of a LERPED direction))  To puit more emphasise on this Alignment Observation.  

The Actions were simply to Drive the Car through the Wheel Collider Dynamics.  The Two driving Action Branches being: 

- Branch 1: NOOP, Forward Torque, Braking/Reverse Torque
- Branch 2: NOOP, Steer Right, Steer Left

## Reward Profile
A Positive Reward if the Car reaches the Goal Check Point. A negative Reward if the Car runs out of Time in Seconds. And a small penalty per Barrier Collision:
 -  +1.0 – 0.5* (LapTime / MaxLapTime)  -  Upon Successful Lap/ Final Check point (End Episode) 
-   -1.0  if the agent runs exceeds MaxLapTime (End Episode) 
-   -0.025  per barrier collision

The MaxLapTime being a growing proportional to the Training Level (Number of Checkpoints in the Track length) 

### Incremential Learning
GAIL Imitation learning was employed, to help influence the initial Agent through the first few checkpoits. So some human demonstrations where provided (CP3Demo, within the Assets/demonstrations folder) - But the initial Agnet behaviour was still pretty random, even with a GAIL strength of 0.25.

The main growth in learning was through a slow Incremential Learning Regime. (Training Levels) The Initial Goal, and positive reward, being simply to get to Check Point 1 at Training level 1. The Training level was incrmented upon 20 successive positive goal acheivements, and a Decrement in Training Level, upon 10 failures was also implmented. (However no decrement in Training Level was ever observed) At each Training Level, the length of the Race Track increased by a single Check point, so a Total of 70+ Training levels, corresponding to the full Track length of 70 CheckPoint colliders,  was necessary to Train the Agent around the whol Track. 

Initially more optimisitic 5 CP jumps per Training level were employed. However the Agent appeared to struggle to achieve growth across multiple Check Points. So the final succesfull training was acheived with singular Check point increases in Lap length.    


### ML Agent Training
See the /config/GrandPrixML.yaml configuration file.  
There was little adjustment and refinement to the PPO hyper parameters being necessary. See the Training Notes.  The Race Car Agent appered to learn reasonbaly easily, with a batch size of 256, buffer six of 2048 and a time horizon of 256. With 2 layers of 512 nteworks for the main and GAIl networks.  The overall Training was pretty very slow,  as only a single Unity environment was used (Not replicated, as I only had a per screen Head Up Display canvas, and a single large Terrian. So I left it to train against the single Car Agent). However the training was very sensitive to very small increments in the Race Lap length. So Training could only be achieved with Lap length increasing by a single Check Point at a time. 

A Total of 70x Training Levels was required, to train the agent to achieve the whole lap.  This was acheived in over around 20 Million Training Epochs.    The training was left for a total of 25 Million epochs, and the Training level counted up to up to 75 (Any levels above 70, simply to reinforce the same full 70 CP Track length.) This was in an attempt to further reinforce and improve the Agnets Race Track performance.   

![ScreenShot](TrainingGrowth.PNG)

The final performance can be seen on the You Tube video. Its quite fun to watch. 

### Conclusions
The Trained Agent was fairly robust to train, albeit rather slow learning through each and all 70 Check Points. 
The Perfomance of the race car was comparable with a reasonably experienced human player (Myself) with some practice on the same Track. 
There is still quite a lot of steering twitching, even on the straights. The Race car dynamics had a lot of understeer, and sliding and so the momentum tends to carry the car forwards despite small twitches in steering. 
I have not tried the agent on different circuits. The slow growth in per Checkpoint Training, susggests that the Agent perfomance would NOT generalise to alternative Track configurations very well. 

* * *

### Acknowledgements

- Blender: 3D Modelling :   https://www.blender.org/
- Unity: Game Development : https://unity.com/
- Unity ML Agents:  https://github.com/Unity-Technologies/ml-agents
- Code Monkey Unity and ML –Agent Tutorials:  https://www.youtube.com/c/CodeMonkeyUnity
- Imphenzia: 3D Blender Modelling : https://www.youtube.com/c/Imphenzia
- Sound Track: Fleetwood Mac: “The Chain“ : https://www.fleetwoodmac.com

## Always be Learning, Creating and Contributing. Do Not Just Consume !
