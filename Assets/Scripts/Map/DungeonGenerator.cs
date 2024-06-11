using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    private int maxEnemies;
    private int currentFloor = 0; // Added currentFloor variable

    List<Room> rooms = new List<Room>();

    private List<string> enemyNames = new List<string>
    {
        "Enemy1", "Enemy2", "Enemy3", "Enemy4", "Enemy5", "Enemy6", "Enemy7", "Enemy8", "Enemy9", "Enemy10"
    };

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    public void SetCurrentFloor(int floor) // Function to set current floor
    {
        currentFloor = floor;
    }

    public void Generate()
    {
        rooms.Clear();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            // if the room overlaps with another room, discard it
            if (room.Overlaps(rooms))
            {
                continue;
            }

            // add tiles make the room visible on the tilemap
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX
                        || x == roomX + roomWidth - 1
                        || y == roomY
                        || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            PlaceEnemies(room, maxEnemies);

            // create a coridor between rooms
            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            rooms.Add(room);
        }

        // Place ladders
        if (currentFloor > 0)
        {
            PlaceLadder(rooms[0].Center(), true); // Ladder up in the first room if not on the first floor
        }

        if (currentFloor < maxRooms - 1)
        {
            PlaceLadder(rooms[rooms.Count - 1].Center(), false); // Ladder down in the last room if not on the last floor
        }

        var player = GameManager.Get.CreateActor("Player", rooms[0].Center());
    }

    private void PlaceEnemies(Room room, int maxEnemies)
    {
        int num = Random.Range(0, maxEnemies + 1);

        for (int counter = 0; counter < num; counter++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Determine the index range for enemy selection based on the current floor
            int maxEnemyIndex = Mathf.Min(currentFloor + 2, enemyNames.Count); // Adjust range as needed
            int enemyIndex = Random.Range(0, maxEnemyIndex);

            string enemyName = enemyNames[enemyIndex];
            GameManager.Get.CreateActor(enemyName, new Vector2(x, y));
        }
    }

    private int maxItems;
    private List<GameObject> itemPrefabs = new List<GameObject>();
    private List<Vector3> availablePositions = new List<Vector3>();

    public void SetMaxItems(int max)
    {
        maxItems = max;
    }

    public void AddItemPrefab(GameObject prefab)
    {
        itemPrefabs.Add(prefab);
    }

    private void PlaceItems(Room room, int maxItems)
    {
        // the number of items we want
        int num = Random.Range(0, maxItems + 1);

        for (int counter = 0; counter < num; counter++)
        {
            // The borders of the room are walls, so add and substract by 1
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // create different items
            float value = Random.value;
            if (value > 0.8f)
            {
                GameManager.Get.CreateGameObject("ScrollOfConfusion", new Vector2(x, y));
            }
            else if (value > 0.5f)
            {
                GameManager.Get.CreateGameObject("Fireball", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateGameObject("HealthPotion", new Vector2(x, y));
            }
        }
    }

    private void PlaceLadder(Vector2 position, bool up)
    {
        GameObject ladderObject = GameManager.Get.CreateGameObject("Ladder", position);
        Ladder ladder = ladderObject.GetComponent<Ladder>();
        ladder.Up = up;
        GameManager.Get.AddLadder(ladder);
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        // if this is a floor, it should not be a wall
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            // if not, it can be a wall
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        // this tile should be walkable, so remove every obstacle
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        // set the floor tile
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            // move horizontally, then vertically
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            // move vertically, then horizontally
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        // Generate the coordinates for this tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // Set the tiles for this tunnel
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (!TrySetWallTile(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }
}