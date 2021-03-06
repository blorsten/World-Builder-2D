using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.ChunkSystem;
using MapGeneration.Extensions;
using UnityEngine;

namespace MapGeneration.Algorithm
{
    /// <summary>
    /// Creates deadends for the map.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dead End Maker", menuName = "2D Map Generation/Algorithms/Dead End Maker")]
    public class DeadEndMaker : DrunkardWalkAlgorithm
    {
        private List<Queue<KeyValuePair<ChunkHolder, CardinalDirections?>>> _roads;

        //Contains all the chunks it should start a drunkard walk from.
        private List<ChunkHolder> _myMarkedChunks;

        //How many entanglements can the dead end maker make.
        [SerializeField] private int _nrOfDeadEnds = 2;

        /// <summary>
        /// This finds valid places to create dead ends and tries to create them.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="usableChunks"></param>
        /// <returns></returns>
        public override bool Process(Map map, List<Chunk> usableChunks)
        {
            //Reset all collections
            Reset();

            //Find out which chunks has aleready been changed by other algorithms.
            FindMarkedChunks(map);

            //Then start the dead end maker.
            StartWalk(map, usableChunks, Vector2Int.zero);

            if (_roads == null || _roads != null && !_roads.Any())
                return false;

            //Creates a list of the types a deadend can have
            List<ChunkType> types = new List<ChunkType>();
            foreach (ChunkType item in Enum.GetValues(typeof(ChunkType)))
            {
                types.Add(item);
            }
            types.Remove(ChunkType.End);
            types.Remove(ChunkType.Start);
            types.Remove(ChunkType.Default);
            types.Remove(ChunkType.Solid);

            //Finds all the chunkholders in _roads and adds them to a sepperate list
            List<ChunkHolder> chunkHolders = new List<ChunkHolder>();
            foreach (Queue<KeyValuePair<ChunkHolder, CardinalDirections?>> item in _roads)
            {
                foreach (KeyValuePair<ChunkHolder, CardinalDirections?> pair in item)
                {
                    chunkHolders.Add(pair.Key);
                }
            }

            //Sets the chunktype of each deadend to a random type.
            foreach (ChunkHolder holder in chunkHolders)
            {
                if (holder.ChunkOpenings.IsDeadEnd() && holder != map.StartChunk || holder != map.EndChunk)
                {
                    ChunkType newType = types[map.Random.Range(0, types.Count)];
                    holder.ChunkType = newType;
                }
            }

            //Backtacks all roads that has been created.
            _roads.ForEach(pairs => BackTrackChunks(pairs));

            return true;
        }

        /// <summary>
        /// Find all chunkholders that has been changed by algorithms.
        /// </summary>
        /// <param name="map"></param>
        private void FindMarkedChunks(Map map)
        {
            foreach (ChunkHolder chunk in map.Grid)
            {
                if (!chunk.ChunkOpenings.IsEmpty())
                {
                    MarkedChunks.Enqueue(chunk);
                }
            }
        }

        /// <summary>
        /// Starts the dead ens maker.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="usableChunks"></param>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        protected override Queue<KeyValuePair<ChunkHolder, CardinalDirections?>> StartWalk(Map map, List<Chunk> usableChunks, Vector2Int startPosition)
        {
            //Save references to start and end.
            ChunkHolder start = map.StartChunk;
            ChunkHolder end = map.EndChunk;

            //Create a new queue for the upcomming road.
            _roads = new List<Queue<KeyValuePair<ChunkHolder, CardinalDirections?>>>();

            //Create a copy of the marked chunks, this will be all the chunks we can visit.
            _myMarkedChunks = MarkedChunks.ToList();

            //Iterate until we reached x number of dead ends.
            for (int i = 0; i < _nrOfDeadEnds; i++)
            {
                //If we dont have any marked chunks to start one, break.
                if (!_myMarkedChunks.Any())
                    break;

                ChunkHolder startChunk = _myMarkedChunks[map.Random.Range(0, _myMarkedChunks.Count)];
                startPosition = startChunk.Position;

                //Tries and do a drunkard walk on the marked chunk.
                Queue<KeyValuePair<ChunkHolder, CardinalDirections?>> newRoad = base.StartWalk(map, usableChunks, startPosition);

                _myMarkedChunks.Remove(startChunk);

                //If it succeeded remove it from start candidates and add it to the main road.
                if (newRoad != null)
                    _roads.Add(newRoad);
                else
                    i--; //If it failed to find a new road, dont count it as a dead end iteration.
            }

            //Now that we have made alot of dead ends, set the start and end to the first ones.
            map.StartChunk = start;
            map.EndChunk = end;
            return null;
        }
    }
}