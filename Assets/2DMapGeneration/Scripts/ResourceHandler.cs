﻿using System;
using System.Collections.Generic;
using MapGeneration.ChunkSystem;
using MapGeneration.Utils;
using UnityEngine;

namespace MapGeneration
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    [ExecuteInEditMode]
    public class ResourceHandler : Singleton<ResourceHandler>
    {
        [SerializeField]private List<Chunk> _chunks;
        [SerializeField] private List<GameplayObject> _objects;

        public List<Chunk> Chunks { get { return _chunks; } private set { _chunks = value; } }
        public List<GameplayObject> Objects{get { return _objects; }private set { _objects = value; }}

        protected override void Awake()
        {
            base.Awake();
            UpdateChunks();
            UpdateObjects();
        }

        /// <summary>
        /// Loads all chunks from resources
        /// </summary>
        public void UpdateChunks()
        {
            Chunks = new List<Chunk>();
            Chunks.AddRange(Resources.LoadAll<Chunk>(String.Empty));
        }

        /// <summary>
        /// Loads all chunks from resources
        /// </summary>
        public void UpdateObjects()
        {
            Objects = new List<GameplayObject>();
            Objects.AddRange(Resources.LoadAll<GameplayObject>(String.Empty));
        }

        [ContextMenu("Update Resources")]
        public void UpdateResoures()
        {
            UpdateChunks();
            UpdateObjects();
        }
    }
}