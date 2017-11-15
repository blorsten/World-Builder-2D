﻿using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration;
using MapGeneration.Extensions;
using UnityEngine;

namespace MapGeneration.Algorithm
{
    /// <summary>
    /// Purpose: Base class for all path algortihms.
    /// Creator: MP & NJ
    /// </summary>
    public class PathAlgorithm : MapGenerationAlgorithm
    {
        protected CardinalDirections NextDirection;
        protected Queue<ChunkHolder> MarkedChunks = new Queue<ChunkHolder>();
        protected Queue<CardinalDirections> DirectionsTaken = new Queue<CardinalDirections>();
        protected List<CardinalDirections> DirectionCandidates;

        public override void Process(Map map, List<Chunk> usableChunks)
        {
            base.Process(map, usableChunks);
            MarkedChunks.Clear();
            DirectionsTaken.Clear();
            ResetDirectionCandidates();
        }

        /// <summary>
        /// Backtraks both queues
        /// </summary>
        /// <param name="chunks">Chunks to backtrack</param>
        /// <param name="directions">Directions to backtrack</param>
        protected void BackTrackChunks(Queue<ChunkHolder> chunks, Queue<CardinalDirections> directions)
        {
            CardinalDirections currentDirection = CardinalDirections.Bottom;
            while (chunks.Count > 0)
            {
                ChunkHolder currentChunk = chunks.Dequeue();

                if (directions.Count > 0)
                    currentDirection = directions.Dequeue();

                if (currentChunk.Instance && chunks.Count > 0)
                    SetChunkConnections(currentDirection, currentChunk, chunks.First());
            }
        }

        /// <summary>
        /// Resets direction candidates back to a list full of directions.
        /// </summary>
        protected void ResetDirectionCandidates()
        {
            DirectionCandidates = ((CardinalDirections[])Enum.GetValues(typeof(CardinalDirections))).ToList();
        }

        protected bool FindNextChunk(Map map, List<Chunk> usableChunks, ref Vector2Int currentPos)
        {
            //find the next direction among the candidates.
            NextDirection = DirectionCandidates[map.Random.Range(0, DirectionCandidates.Count)];

            //Get the next position
            Vector2Int? nextPosition = CheckNextPosition(currentPos, NextDirection, map);

            //if the position is valid continue the process
            if (nextPosition != null)
            {
                ChunkHolder nextChunk = map.Grid[nextPosition.Value.x, nextPosition.Value.y];

                //if the next chunk isnt marked, continue the process
                if (!MarkedChunks.Contains(nextChunk))
                {
                    //enqueue the direction that was taken
                    DirectionsTaken.Enqueue(NextDirection);

                    //set current position to the next position
                    currentPos = nextPosition.Value;

                    //enqueue the next chunk, so we know it is used.
                    MarkedChunks.Enqueue(nextChunk);

                    //Change the prefab on the found chunk to another one. TODO: Find another way to mark marked chunks.
                    nextChunk.Prefab = usableChunks.FirstOrDefault();
                    //Reset candidates.

                    ResetDirectionCandidates();
                    return true;
                }

                DirectionCandidates.Remove(NextDirection);
            }
            else
                DirectionCandidates.Remove(NextDirection);

            map.EndChunk = MarkedChunks.LastOrDefault();
            return false;
        }
    }
}