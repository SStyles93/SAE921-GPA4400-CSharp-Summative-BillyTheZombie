using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MapPCG : MonoBehaviour
{
    [Header("Tilemap")]
    [Tooltip("TileMap")]
    [SerializeField] private Tilemap _waterTileMap;
    [SerializeField] private Tilemap _groundTileMap;
    [SerializeField] private Tilemap _wallTileMap;

    [Header("Tiles")]
    [Tooltip("Basic white tile")]
    [SerializeField] private RuleTile _waterRuleTile;
    [SerializeField] private RuleTile _groundRuleTile;
    [SerializeField] private RuleTile _wallRuleTile;

    [Header("Rooms")]
    [Tooltip("The Area in which we want to create a map")]
    [SerializeField] private BoundsInt _mainArea;
    [SerializeField] private Queue<BoundsInt> _areaQueue = new Queue<BoundsInt>();
    [Tooltip("The cutting delta set the difference between room after a cut," +
        " set to 0 the rooms will be cut in a 50/50 ratio." +
        " Set to 0.5 the rooms will be randomly cut")]
    [Range(0.0f, 0.5f)]
    [SerializeField] private float _cuttingDelta = 0.0f;
    [SerializeField] private Vector2Int _minWidthHeight = new Vector2Int(10, 10);
    [SerializeField] private int _roomShrink = 2;

    private List<BoundsInt> _areaList = new List<BoundsInt>();
    private List<BoundsInt> _roomList = new List<BoundsInt>();
    private HashSet<Vector2Int> _allPositions = new HashSet<Vector2Int>();
    HashSet<Vector2Int> _roomsPositions = new HashSet<Vector2Int>();

    private enum SplitDirection
    {
        HORIZONTAL,
        VERTICAL
    }
    private SplitDirection splitDirection = SplitDirection.HORIZONTAL;

    public void Generate()
    {
        //Reset Tiles & Generate the TileMap Area
        ClearAll();
        GenerateBounds();

        //Create Water
        _roomList.Add(_mainArea);
        GetTilePositionsFromRoom();
        FillRoom(_waterTileMap, _waterRuleTile, _allPositions);
        ClearLists();

        //Create Ground
        _areaQueue.Enqueue(_mainArea);
        AreaBSP();
        FillAreaWithRoom();
        GetTilePositionsFromRoom();
        SetMap();
        FillRoom(_groundTileMap, _groundRuleTile, _roomsPositions);
        FillRoom(_wallTileMap, _wallRuleTile, _roomsPositions);

        

    }
    /// <summary>
    /// Clears all the TileMaps and lists
    /// </summary>
    public void ClearAll()
    {
        ClearMaps();
        ClearLists();
    }

    /// <summary>
    /// Gets the spawning point
    /// </summary>
    /// <returns>Returns the center of the first room in the RoomList</returns>
    public Vector3 GetSpawningPoint()
    {
        return _roomList[0].center;
    }

    /// <summary>
    /// Draws the Gizmos used to previsualise elements
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_mainArea.center, _mainArea.size);

        Gizmos.color = Color.red;
        if (_areaList.Count == 0) return;
        foreach (BoundsInt area in _areaList)
        {
            Gizmos.DrawWireCube(area.center, area.size);
        }

        Gizmos.color = Color.green;
        if (_roomList.Count == 0) return;
        foreach (BoundsInt room in _roomList)
        {
            Gizmos.DrawWireCube(room.center, room.size);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_roomList[0].center, 1.0f);
    }

    /// <summary>
    /// Clears all the TileMaps
    /// </summary>
    private void ClearMaps()
    {
        _waterTileMap.ClearAllTiles();
        _groundTileMap.ClearAllTiles();
        _wallTileMap.ClearAllTiles();
    }

    /// <summary>
    /// Clears all the lists
    /// </summary>
    private void ClearLists()
    {
        _areaList.Clear();
        _areaQueue.Clear();
        _roomList.Clear();
        _allPositions.Clear();
        _roomsPositions.Clear();
    }

    /// <summary>
    /// Sets the TileMaps size to the MainArea size
    /// </summary>
    private void GenerateBounds()
    {
        _waterTileMap.size = _mainArea.size;
        _groundTileMap.size = _mainArea.size;
        _wallTileMap.size = _mainArea.size;
    }

    /// <summary>
    /// Split rooms into two new rooms
    /// </summary>
    /// <param name="room">The room to split</param>
    /// <param name="splitDirection">The direction to split the room</param>
    /// <param name="ratio">The ratio with which we want to split the room</param>
    /// <param name="firstRoom">The first room result</param>
    /// <param name="secondRoom">The second room result</param>
    private void SplitRoom(BoundsInt room, SplitDirection splitDirection, float ratio, out BoundsInt firstRoom, out BoundsInt secondRoom)
    {
        firstRoom = new BoundsInt();
        secondRoom = new BoundsInt();

        switch (splitDirection)
        {

            case SplitDirection.HORIZONTAL:

                firstRoom.xMin = room.xMin;
                firstRoom.xMax = room.xMax;
                firstRoom.yMin = room.yMin;
                firstRoom.yMax = room.yMin + Mathf.FloorToInt(room.size.y * ratio);

                secondRoom.xMin = room.xMin;
                secondRoom.xMax = room.xMax;
                secondRoom.yMin = firstRoom.yMax;
                secondRoom.yMax = room.yMax;
                break;
            case SplitDirection.VERTICAL:

                firstRoom.xMin = room.xMin;
                firstRoom.xMax = room.xMin + Mathf.FloorToInt(room.size.x * ratio);
                firstRoom.yMin = room.yMin;
                firstRoom.yMax = room.yMax;

                secondRoom.xMin = firstRoom.xMax;
                secondRoom.xMax = room.xMax;
                secondRoom.yMin = room.yMin;
                secondRoom.yMax = room.yMax;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Binary Space Partitionning, splits the rooms until conditions are met
    /// </summary>
    private void AreaBSP()
    {
        BoundsInt room1 = new BoundsInt();
        BoundsInt room2 = new BoundsInt();
        do
        {
            BoundsInt roomToProcess = _areaQueue.Dequeue();

            if (Random.value > 0.5f)
            {
                splitDirection = SplitDirection.HORIZONTAL;
            }
            else
            {
                splitDirection = SplitDirection.VERTICAL;
            }

            if (roomToProcess.size.x < _minWidthHeight.x || roomToProcess.size.y < _minWidthHeight.y)
            {
                _areaList.Add(roomToProcess);
            }
            else
            {
                SplitRoom(roomToProcess, splitDirection, Random.Range(0.5f - _cuttingDelta, 0.5f + _cuttingDelta), out room1, out room2);
                _areaQueue.Enqueue(room1);
                _areaQueue.Enqueue(room2);
            }

        } while (_areaQueue.Count > 0);
    }

    /// <summary>
    /// Fills the Areas with rooms
    /// </summary>
    private void FillAreaWithRoom()
    {
        for (int i = 0; i < _areaList.Count; i++)
        {
            BoundsInt newRoom = new BoundsInt();
            newRoom.xMin = _areaList[i].xMin + _roomShrink;
            newRoom.xMax = _areaList[i].xMax - _roomShrink;
            newRoom.yMin = _areaList[i].yMin + _roomShrink;
            newRoom.yMax = _areaList[i].yMax - _roomShrink;
            _roomList.Add(newRoom);
        }
    }

    /// <summary>
    /// Adds the Tile positions for rooms to tilePositions
    /// </summary>
    private void GetTilePositionsFromRoom()
    {
        foreach (BoundsInt room in _roomList)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    _allPositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    /// <summary>
    /// Sets the final positions of all rooms in one HashSet
    /// </summary>
    private void SetMap()
    {
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        HashSet<Vector2Int> corridorsPositions = new HashSet<Vector2Int>();

        foreach (BoundsInt room in _roomList)
        {
            // Add positions
            _roomsPositions.UnionWith(_allPositions);
            // Collect centers
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        corridorsPositions = ConnectRooms(roomCenters);
        _roomsPositions.UnionWith(corridorsPositions);
    }

    /// <summary>
    /// Connects all the rooms together
    /// </summary>
    /// <param name="roomCenters">The center of the rooms to connect</param>
    /// <returns>The position of the connections</returns>
    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> roomConnections = new HashSet<Vector2Int>();
        Vector2Int currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        Vector2Int closestCenter;

        while (roomCenters.Count > 0)
        {
            List<Vector2Int> centersSort = roomCenters.OrderBy(c => Vector2Int.Distance(c, currentRoomCenter)).ToList();
            closestCenter = centersSort[0];

            HashSet<Vector2Int> oneRoomConnections = OneRoomConnection(currentRoomCenter, closestCenter);

            // Add corridor to all corridors
            roomConnections.UnionWith(oneRoomConnections);
            // Remove the center processed to the list
            roomCenters.Remove(currentRoomCenter);
            // the next center to process is the closest one
            currentRoomCenter = closestCenter;

        }
        return roomConnections;
    }

    /// <summary>
    /// Connects rooms together
    /// </summary>
    /// <param name="origin">The origin position</param>
    /// <param name="destination">The destination position</param>
    /// <returns>A connection between the origin and destination position</returns>
    private HashSet<Vector2Int> OneRoomConnection(Vector2Int origin, Vector2Int destination)
    {
        HashSet<Vector2Int> roomConnection = new HashSet<Vector2Int>();

        var position = origin;
        // Move left or right and find X match between origin and destination
        do
        {
            roomConnection.Add(position);
            if (position.x < destination.x)
            {
                position += Vector2Int.right;
                roomConnection.Add(position += Vector2Int.up);
                roomConnection.Add(position += Vector2Int.up);
                roomConnection.Add(position += Vector2Int.down);
                roomConnection.Add(position += Vector2Int.down);
            }
            else if (position.x > destination.x)
            {
                position += Vector2Int.left;
                roomConnection.Add(position += Vector2Int.up);
                roomConnection.Add(position += Vector2Int.up);
                roomConnection.Add(position += Vector2Int.down);
                roomConnection.Add(position += Vector2Int.down);
            }
        } while (position.x != destination.x);

        // Move up or down and find Y match between origin and destination
        do
        {
            if (position.y < destination.y)
            {
                position += Vector2Int.up;
                roomConnection.Add(position += Vector2Int.left);
                roomConnection.Add(position += Vector2Int.left);
                roomConnection.Add(position += Vector2Int.right);
                roomConnection.Add(position += Vector2Int.right);
            }
            else if (position.y > destination.y)
            {
                position += Vector2Int.down;
                roomConnection.Add(position += Vector2Int.right);
                roomConnection.Add(position += Vector2Int.right);
                roomConnection.Add(position += Vector2Int.left);
                roomConnection.Add(position += Vector2Int.left);
            }
        } while (position.y != destination.y);

        return roomConnection;

    }

    /// <summary>
    /// Fills the room with tiles
    /// </summary>
    /// <param name="map">The TileMap to fill</param>
    /// <param name="ruleTile">The RuleTile to use</param>
    /// <param name="positionsHashSet">The positions of the tiles to fill</param>
    private void FillRoom(Tilemap map, RuleTile ruleTile, HashSet<Vector2Int> positionsHashSet)
    {
        foreach (Vector2Int position in positionsHashSet)
        {
            map.SetTile(map.WorldToCell((Vector3Int)position), ruleTile);
        }
    }
}
