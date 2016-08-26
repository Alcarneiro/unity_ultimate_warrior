﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.Factories;
using UnityEngine;
using UniRx;

namespace Assets
{
    public class Tile :MonoBehaviour
    {
        public string MapImageUrlBase = "http://b.tile.openstreetmap.org/";

        [SerializeField]
        public Rect Rect;

        private Dictionary<Type, Factory> _factories;
        private Settings _settings;

        public Tile Initialize(Dictionary<Type, Factory> factory, Settings settings)
        {
            _factories = factory;
            _settings = settings;
            Rect = GM.TileBounds(_settings.TileTms, _settings.Zoom);
            return this;
        }

        public void ConstructTile(string text)
        {
            ConstructAsync(text);
        }

        private void ConstructAsync(string text)
        {
            string url;

            var heavyMethod = Observable.Start(() => new JSONObject(text));
            
            heavyMethod.ObserveOnMainThread().Subscribe(mapData =>
            {
                if (!this) // checks if tile still exists and haven't destroyed yet
                    return;

                var go = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                go.name = "map";
                go.SetParent(transform, true);
                go.localScale = new Vector3(Rect.width, Rect.width, 1);
                go.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
                go.localPosition = Vector3.zero;
                go.localPosition -= new Vector3(0, 1, 0);
                var rend = go.GetComponent<Renderer>();
                rend.material = Resources.Load<Material>("Ground");
                rend.material.mainTexture = new Texture2D(512, 512, TextureFormat.DXT5, false);
                rend.material.color = new Color(.1f, .1f, .1f, 1f);

                if (_settings.LoadImages)
                {
                    rend.material.color = new Color(1f, 1f, 1f, 1f);
                    url = MapImageUrlBase + _settings.Zoom + "/" + _settings.TileTms.x + "/" + _settings.TileTms.y + ".png";
                    ObservableWWW.GetWWW(url).Subscribe(x =>
                    {
                        x.LoadImageIntoTexture((Texture2D)rend.material.mainTexture);
                    });
                }

                StartCoroutine(CreateBuildings(mapData["buildings"], _settings.TileCenter));
                StartCoroutine(CreateRoads(mapData["roads"], _settings.TileCenter));
                StartCoroutine(CreateWater(mapData["water"], _settings.TileCenter));
            });
            
        }

        private IEnumerator CreateBuildings(JSONObject mapData, Vector2 tileMercPos)
        {
            foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon"))
            {
                _factories[typeof(BuildingFactory)].Create(tileMercPos, geo, transform);
                yield return null;
            }
        }

        private IEnumerator CreateWater(JSONObject mapData, Vector2 tileMercPos)
        {
            foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon" || x["geometry"]["type"].str == "MultiPolygon"))
            {
                _factories[typeof(WaterFactory)].Create(tileMercPos, geo, transform);
                yield return null;
            }
        }

        private IEnumerator CreateRoads(JSONObject mapData, Vector2 tileMercPos)
        {
            for (int index = 0; index < mapData["features"].list.Count; index++)
            {
                var geo = mapData["features"].list[index];
                _factories[typeof(RoadFactory)].Create(tileMercPos, geo, transform);
                yield return null;
            }
        }

        public class Settings
        {
            public int Zoom { get; set; }
            public Vector2 TileTms { get; set; }
            public Vector3 TileCenter { get; set; }
            public bool LoadImages { get; set; }
        }
    }
}
