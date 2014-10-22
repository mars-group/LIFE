Notes about the movement usage and structure
********************************************

Axis definitions:

  x-Axis:  (-)  nearer |  farer   (+)       
  y-Axis:  (-) ← left  |  right → (+) 
  z-Axis:  (-) ↓ down  |     up ↑ (+)

  The x and y axis form a plane. The z axis represents the height and is used 
  for 3D scenarios.


To facilitate agent movement, the abstract base 'Agent' is inherited by the 'SpatialAgent'. 
... tbc ... 





Abstract structures:

  The classes ML0 to ML* (ML stands for movement level) build a hierarchy with 
  each class based on the level before. ML0 is the lowest class. It uses the IESC 
  implementation to link the agent's movement to the collision detection system 
  (or later, a physics engine).

  With each higher level, the complexity of the movement stack increases. 

  To restrain access to the functions designed for a level, the movement stack is 
  abstract. Hereby the use of inherited methods is impossible (you probably won't 
  allow direct displacement when you use a physical simulation).

