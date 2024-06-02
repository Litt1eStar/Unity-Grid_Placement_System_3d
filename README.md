# Unity - Grid Placment System 3d
## Read this message
This is Grid Placement System in 3d where you can Place and Delete building on grid
- Basically, I have create this repository for my experimental to create Grid System from Scracth by myself.
- I found that Grid Placement System logic is does not complex that much by itself but the part that i found that it's a bit **triggy** is Grid Visualize.
- Grid Visualize is feature that will make your grid have some visual so basically in most of game didn't need to do this in real gameplay but it's very useful for **Development Stage**.
- **In my opinion**, Grid Visualize is make developer can see what they do are correctly or not on grid and it's great in testing purpose.
- In this Repository, I have add some comparment about perfomance of Grid Visualize too. So you will see that it's use a lot of resource to run this feature in real-time rendering

## If you want to looking to Grid System then these are class that you need to work with
#### GridSystem.cs
**File**: `Assets/Script/Grid/GridSystem.cs`

**Description**:
This is C# Object that have contain logic of Grid System. For example PlacementBuilding, DeleteBuilding and other method to work with grid system
- I have contain grid data as Dictionary<Vector2Int, GridObject> that's key of Dictionary is grid position and value of Dictionary is grid object
- #### For Placement building on grid logic, basically in GridObject there is 1 field named **ref_head** that's GridObject data type.
  - In my experimental project, User need to click on 1 grid to place building.
  - So, we will have data of GridPosition where user is clicked and we need another data that's building size.
  - Let said that in this case it's (3x3) bulding size.
  - Now to place building on grid, we already have 2 data (**Vector2Int** clickedPosition, **Vector2Int** buildingSize) and we have another data named **headGrid** that's grid at the position of clickedPosition then it's just iterating to all grid in range of buildingSize and loop is started from clickedPosition and it will assign value of ref_head to be **headGrid**
  - For now, all grid that started from clickedPosition and it's in range of buildingSize(3x3) will have ref_head as headGrid and we assign building data to headGrid only.
- #### For Delete building on grid logic
  - In my experimental project, User can click at any grid of building to delete that building.
  - To Delete building we need 2 data (**Vector2Int** clickedPosition, **Gameobejct** buildingOnScene).
  - When user is clicked on grid then we will have that GridObject at position of clickedPosition and if that GridObject have ref_head it's meaning that this grid have building on it.
  - So, to delete this building. we have another data that get from clickedPosition that's ref_head of grid then we're going to start loop at ref_head grid position and loop in range of buildingSize and set ref_head to be null.
  - Now all grid that's started from ref_head position and it's in range of buildingSize will have ref_head value to be null so this is meaning that buildingData is deleted.
  - Last thing that we need to do is delete building data at ref_head and Destroy buildingOnScene Gameobject
#### GridVisualizer.cs
**File**: `Assets/Script/Grid/GridVisualizer.cs`
#### GridObject.cs
**File**: `Assets/Script/Grid/GridObject.cs`

