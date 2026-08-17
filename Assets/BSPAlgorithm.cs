using System.Collections.Generic;
using UnityEngine;

public class BSPAlgorithm : MonoBehaviour
{

    public static List<BoundsInt> BSPGeneration(BoundsInt splitSpace, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>(); //Queue of BoundsInt values acting as the rooms
        List<BoundsInt> roomsList = new List<BoundsInt>(); //List to store rooms after they've been split
        roomsQueue.Enqueue(splitSpace);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minWidth) //If the room is bigger than the minimum size allocated through min variables
            {
                if(Random.value < 0.5f) //Randomising which axis will be checked first for splitting to allow for more varied splits
                {
                    if (room.size.y >= minHeight * 2) //If room is two times the minimum size on the y axis or more, it is split horizontally
                    {
                        SplitHorizontally(roomsQueue, room);
                    } else if (room.size.x >= minWidth * 2) //If room is two times the minimum size on the x axis or more, it is split vertcally
                    {
                        SplitVertixally(roomsQueue, room);
                    } else if (room.size.x >= minWidth && room.size.y >= minHeight) //If two rooms can't fit into the room, the room is saved in the list
                    {
                        Debug.Log("Room added from first chunk");
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertixally(roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                        Debug.Log("Room added from second chunk");
                    }
                }
            } 
        }
        foreach (var room4 in roomsList) //Debugging room locations
        {
            Debug.Log(room4);
        }
        return roomsList;
    }

    private static void SplitVertixally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        Debug.Log("Room Split Verically");
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), 
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        Debug.Log("Room Split Horizontally");
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}