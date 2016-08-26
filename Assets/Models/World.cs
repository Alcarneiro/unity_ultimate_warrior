using System;
using System.Collections.Generic;
using Assets;
using Assets.Helpers;
using Assets.Models;
using UnityEditor.VersionControl;
using UnityEngine;
using System.Collections;
using Assets.Models.Factories;

public class World : MonoBehaviour
{
    [SerializeField] private Settings _settings;
    private TileManager _tileManager;

    private Dictionary<Type, Factory> Factories;

    void Start ()
    {
        Factories = new Dictionary<Type, Factory>();
        var buildingFactory = GetComponentInChildren<BuildingFactory>();
        Factories.Add(typeof(BuildingFactory), buildingFactory);
        var roadFactory = GetComponentInChildren<RoadFactory>();
        Factories.Add(typeof(RoadFactory), roadFactory);
        var waterFactory = GetComponentInChildren<WaterFactory>();
        Factories.Add(typeof(WaterFactory), waterFactory);

        _tileManager = GetComponent<TileManager>();
        _tileManager.Init(Factories, _settings);
	}

    [Serializable]
    public class Settings
    {
		private float lat = 49.553057f; 
        [SerializeField]
		public float Lat {
			get {
				return lat;
			}
			set {
				lat = value;
			}
		}
		private float longit = 5.885954f;
        [SerializeField]
		public float Long {
			get {
				return longit;
			} 
			set {
				longit = value;
			}
		}
		private int range = 3;
        [SerializeField]
		public int Range {
			get {
				return range;
			} 
			set {
				range = value;
			}
		}
		private int detaillevel = 16;
        [SerializeField]
		public int DetailLevel {
			get {
				return detaillevel;
			}
			set {
				detaillevel = value;
			}
		}
		private bool loadimage = false;
        [SerializeField]
		public bool LoadImages {
			get {
				return loadimage;
			}
			set {
				loadimage = value;
			}
		}
		private float tilesize = 100;
        [SerializeField]
		public float TileSize {
			get {
				return tilesize;
			}
			set {
				tilesize = value;
			}
		}
		private string key = "vector-tiles-SspBZQu";
		[SerializeField]
		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}
    }
}
