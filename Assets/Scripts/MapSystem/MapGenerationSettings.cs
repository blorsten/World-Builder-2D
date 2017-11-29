﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    /// <summary>
    /// Purpose: Withholds all the settings for the map generation system.
    /// Creator: MP
    /// </summary>
    public class MapGenerationSettings : ScriptableObject 
    {
        [Header("Gizmo Settings:")]
        [SerializeField] private Color _defaultConnectionColor;
        [SerializeField] private Color _criticalConnectionColor;

        [Header("Biome Setings")]
        [SerializeField] private List<string> _biomes = new List<string>();

        public Color DefaultConnectionColor
        {
            get { return _defaultConnectionColor; }
            set { _defaultConnectionColor = value; }
        }

        public Color CriticalConnectionColor
        {
            get { return _criticalConnectionColor; }
            set { _criticalConnectionColor = value; }
        }

        public List<string> Biomes
        {
            get { return _biomes; }
            set { _biomes = value; }
        }

        public string NoiseToBiome(float noice)
        { 
            float index = Mathf.FloorToInt(Mathf.Clamp(noice * _biomes.Count,0, _biomes.Count - 1));
            return _biomes[Mathf.Clamp((int)index,0, _biomes.Count-1)];

        }
    }
}