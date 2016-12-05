===================================================================================================

							Brightstone V 1.0

===================================================================================================

****PIXEL PERFECT****
1. Scale Ortho Size with Vertical Resolution
- eg. OrthoSize = (Screen Height)/(Pixels Per Unit * Pixels Per Unit Scale) * 0.5
2. Sprite not using mipmaps
3. Sprite using Point filter for rendering.
4. Sprite imported with proper pixels per unit.

Naming Convention
namespace Brightstone -- Pascal Case unless abbreviation then Capital CAse
class Name -- Pascal Case
member mVariable -- m prefixed Pascal Case
member variable of POD struct  -- Camel Case
local variable -- Camel Case
function Name() -- Pascal Case
constant VALUE -- Capital Case

avoid C# property usage.

Notable Types:

BaseComponent : base class for all components
BaseObject : base class for all non-component objects
ObjectType : class containing information about an object whether BaseComponent or BaseObject

World : manager class which manages the "world"
WorldZone : "container" class which holds a group of objects in an area within the world.

InputMgr : manager class which manages inputs
PhysicsMgr : manager class which manages physics
NetworkMgr : manager class which manages network stuffs
NpcMgr : manager class which manages ai
EventMgr : manager class for managing game events.
TypeMgr : manage class for types.

Actor : class for objects in the world
ActorAttachment : class for dynamic creation of objects during runtime.
Player : base class for "player".
HumanPlayer : human player class which handles inputs and translates that into game


--- Tasks:
- Create a player that can move around with WASD keys
- Spawn a player in game via a spawner.
- Allow a player to take damage.
- Allow a player to respawn
- Provide networking to have a player move
- Provide animation for the players movement
- Player movement { Idle, Walk, Run, Strafe, Roll, Block, }



--- Defense...
-