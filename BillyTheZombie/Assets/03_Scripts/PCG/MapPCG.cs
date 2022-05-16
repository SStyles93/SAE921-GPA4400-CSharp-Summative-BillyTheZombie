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
    [SerializeField] private Tilemap _objectTileMap;

    [Header("Tiles")]
    [Tooltip("Basic white tile")]
    [SerializeField] private RuleTile _waterRuleTile;
    [SerializeField] private RuleTile _groundRuleTile;
    [SerializeField] private RuleTile _objectRuleTile;

    [Header("Rooms")]
    [SerializeField] private BoundsInt _mainArea;
    [SerializeField] private Queue<BoundsInt> _areaQueue = new Queue<BoundsInt>();
    [SerializeField] private float _ratioLowerBound = 0.45f;
    [SerializeField] private float _ratioUpperBound = 0.55f;

    [SerializeField] private int _minWidth = 40;
    [SerializeField] private int _minHeight = 40;

    [SerializeField] private int _roomFitOffset = 2;

    private List<BoundsInt> _areaList = new List<BoundsInt>();
    private List<BoundsInt> _roomList = new List<BoundsInt>();
    private HashSet<Vector2Int> _tilePositions = new HashSet<Vector2Int>();
    //Vector3[,] links = new Vector3[,] { };
    //HashSet<Vector3> savedLinks = new HashSet<Vector3>();
    HashSet<Vector2Int> allPositions = new HashSet<Vector2Int>();


    private enum SplitDirection
    {
        HORIZONTAL,
        VERTICAL
    }
    private SplitDirection splitDirection = SplitDirection.HORIZONTAL;

    public void Generate()
    {
        //Reset Tiles
        DeleteTiles();
        GenerateBounds();

        //Create Water
        _roomList.Add(_mainArea);
        GetTilePositionsFromRoom();
        FillWaterRoom(_waterTileMap, _waterRuleTile);
        _roomList.Clear();
        _tilePositions.Clear();

        //Create Ground
        _areaQueue.Enqueue(_mainArea);
        AreaBSP();
        FillAreaWithRoom();
        GetTilePositionsFromRoom();
        SetMap();
        FillRoom(_groundTileMap, _groundRuleTile);
        FillRoom(_objectTileMap, _objectRuleTile);

    }
    public void DeleteTiles()
    {
        _waterTileMap.ClearAllTiles();
        _groundTileMap.ClearAllTiles();
        _objectTileMap.ClearAllTiles();
        _areaList.Clear();
        _areaQueue.Clear();
        _roomList.Clear();
        _tilePositions.Clear();
        allPositions.Clear();
    }

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

        //Gizmos.color = Color.yellow;
        //if (links.Length == 0) return;
        //for (int linkIdx = 0; linkIdx < links.Length / 2; linkIdx++)
        //{
        //    Gizmos.DrawLine(links[linkIdx, 0], links[linkIdx, 1]);
        //}

        //Gizmos.color = Color.yellow;
        //if (links.Length == 0) return;
        //for (int linkIdx = 0; linkIdx < links.Length / 2; linkIdx++)
        //{
        //    Gizmos.DrawLine(links[linkIdx, 0], links[linkIdx, 1]);
        //}
    }

    ///// <summary>
    ///// Paint a random tile in the mainRoom boundaries
    ///// </summary>
    //private void RandomTileSet()
    //{
    //    Vector2Int tilepos = new Vector2Int(Random.Range(0, _mainArea.size.x), Random.Range(0, _mainArea.size.y));
    //    Vector3Int tilePosition = new Vector3Int(tilepos.x, tilepos.y, 0);
    //    _tileMap.SetTile(tilePosition, _ruleTileWater);
    //}

    /// <summary>
    /// Sets the TileMap size to the MainArea size
    /// </summary>
    private void GenerateBounds()
    {
        _waterTileMap.size = _mainArea.size;
        _groundTileMap.size = _mainArea.size;
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

            if (roomToProcess.size.x < _minWidth || roomToProcess.size.y < _minHeight)
            {
                _areaList.Add(roomToProcess);
            }
            else
            {
                SplitRoom(roomToProcess, splitDirection, Random.Range(_ratioLowerBound, _ratioUpperBound), out room1, out room2);
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
            newRoom.xMin = _areaList[i].xMin + _roomFitOffset;
            newRoom.xMax = _areaList[i].xMax - _roomFitOffset;
            newRoom.yMin = _areaList[i].yMin + _roomFitOffset;
            newRoom.yMax = _areaList[i].yMax - _roomFitOffset;
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
                    _tilePositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    /// <summary>
    /// Creates the final
    /// </summary>
    private void SetMap()
    {
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        HashSet<Vector2Int> corridorsPositions = new HashSet<Vector2Int>();
        
        foreach (BoundsInt room in _roomList)
        {
            // Add positions
            allPositions.UnionWith(_tilePositions);
            // Collect centers
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        corridorsPositions = ConnectRooms(roomCenters);
        allPositions.UnionWith(corridorsPositions);
    }

    /// <summary>
    /// Connects all the rooms together
    /// </summary>
    /// <param name="roomCenters">List of room centers</param>
    /// <returns>HashSet of positions connecting rooms</returns>
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
    /// Connects two rooms together
    /// </summary>
    /// <param name="origin">The origin room</param>
    /// <param name="destination">The destination room</param>
    /// <returns>HashSet of position connecting rooms</returns>
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
    /// Fills all the rooms with the given tile
    /// </summary>
    /// <param name="tile"></param>
    private void FillWaterRoom(Tilemap map, RuleTile tile)
    {
        foreach (Vector2Int position in _tilePositions)
        {
            map.SetTile(map.WorldToCell((Vector3Int)position), tile);
        }
    }
    private void FillRoom(Tilemap map, RuleTile tile)
    {
        foreach (Vector2Int position in allPositions)
        {
            map.SetTile(map.WorldToCell((Vector3Int)position), tile);
        }
    }


    //private void SetRoomLinks()
    //{
    //    //List<BoundsInt> roomsToCheck = new List<BoundsInt>();
    //    //List<float> minDistances = new List<float>();


    //    ////Initialize the lists of rooms to check
    //    //for (int roomIdx = 0; roomIdx < _roomList.Count; roomIdx++)
    //    //{
    //    //    roomsToCheck.Add(_roomList[roomIdx]);
    //    //    minDistances.Add(roomIdx);
    //    //    minDistances[roomIdx] = Mathf.Infinity;

    //    //}

    //    //links = new Vector3[_roomList.Count-1, 2];
    //    //links = new Vector3[_roomList.Count - 1, 2];

    //    //for (int link = 0; link < links.Length/2; link++)
    //    //{
    //    //    for (int roomA = 0; roomA < roomsToCheck.Count; roomA++)
    //    //    {
    //    //        for (int roomB = 0; roomB < _roomList.Count; roomB++)
    //    //        {
    //    //            if (roomA == roomB || (_roomList[roomB].center - roomsToCheck[roomA].center).magnitude == 0f) continue;
    //    //            if ((_roomList[roomB].center - roomsToCheck[roomA].center).magnitude < minDistances[link])
    //    //            {
    //    //                minDistances[link] = (_roomList[roomB].center - roomsToCheck[roomA].center).magnitude;
    //    //                links[link, 0] = roomsToCheck[roomA].center;
    //    //                links[link, 1] = _roomList[roomB].center;

    //    //            }
    //    //        }
    //    //        roomsToCheck.RemoveAt(roomA);
    //    //    }
    //    //}



    //    //links = new Vector3[roomsToCheck.Count, _roomList.Count];
    //    //savedLinks.Clear();

    //    //for (int roomA = 0; roomA < _roomList.Count; roomA++)
    //    //{
    //    //    for (int roomB = 0; roomB < roomsToCheck.Count; roomB++)
    //    //    {
    //    //        //stop links between the same rooms
    //    //        if (roomA == roomB) continue;
    //    //        if ((roomsToCheck[roomB].center - _roomList[roomA].center).magnitude < minDistances[roomA])
    //    //        {
    //    //            minDistances[roomA] = (roomsToCheck[roomB].center - _roomList[roomA].center).magnitude;
    //    //            links[roomA, roomB] = _roomList[roomA].center;
    //    //            links[roomB, roomA] = roomsToCheck[roomB].center;
    //    //            savedLinks.Add(_roomList[roomA].center);
    //    //            savedLinks.Add(roomsToCheck[roomB].center);
    //    //        }
    //    //    }
    //    //}

    //    //    links = new Vector3[roomsToCheck.Count-1, 2];

    //    //    for (int roomA = roomsToCheck.Count-1; roomA > 0; --roomA)
    //    //    {
    //    //        for (int roomB = 0; roomB < _roomList.Count; roomB++)
    //    //        {
    //    //            //stop links between the same rooms
    //    //            if (roomsToCheck[roomA].center == _roomList[roomB].center) continue;

    //    //            //Check to save only One A-B link
    //    //            if ((roomsToCheck[roomA].center - _roomList[roomB].center).magnitude < minDistances[roomA])
    //    //            {
    //    //                minDistances[roomA] = (roomsToCheck[roomA].center - _roomList[roomB].center).magnitude;
    //    //                ////Save the link
    //    //                links[roomA-1, 0] = roomsToCheck[roomA].center;
    //    //                links[roomA-1, 1] = _roomList[roomB].center;
    //    //            }
    //    //        }
    //    //        roomsToCheck.RemoveAt(roomA);
    //    //    }
    //}
}
