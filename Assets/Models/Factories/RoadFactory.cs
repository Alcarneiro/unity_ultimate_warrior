﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Models.Factories
{
    public class RoadFactory : Factory
    {
        public override void Create(Vector2 tileMercPos, JSONObject geo, Transform tt = null)
        {
            if (geo["geometry"]["type"].str == "LineString")
            {
                var roadEnds = new List<Vector3>();
                for (int i = 0; i < geo["geometry"]["coordinates"].list.Count; i++)
                {
                    var c = geo["geometry"]["coordinates"][i];
                    var dotMerc = GM.LatLonToMeters(c[1].f, c[0].f);
                    var localMercPos = new Vector2(dotMerc.x - tileMercPos.x, dotMerc.y - tileMercPos.y);
                    roadEnds.Add(localMercPos.ToVector3xz());
                }
                CreateRoadSegment(tt, geo, roadEnds);
            }
            else if (geo["geometry"]["type"].str == "MultiLineString")
            {
                for (int i = 0; i < geo["geometry"]["coordinates"].list.Count; i++)
                {
                    var roadEnds = new List<Vector3>();
                    var c = geo["geometry"]["coordinates"][i];

                    for (int j = 0; j < c.list.Count; j++)
                    {
                        var seg = c[j];
                        var dotMerc = GM.LatLonToMeters(seg[1].f, seg[0].f);
                        var localMercPos = new Vector2(dotMerc.x - tileMercPos.x, dotMerc.y - tileMercPos.y);
                        roadEnds.Add(localMercPos.ToVector3xz());
                    }
                    CreateRoadSegment(tt, geo, roadEnds);
                }
            }
        }

        private void CreateRoadSegment(Transform tt, JSONObject geo, List<Vector3> roadEnds)
        {
            var m = new GameObject("road").AddComponent<RoadPolygon>();
            m.transform.SetParent(tt, false);
            try
            {
                m.Initialize(geo["properties"]["id"].str, tt.position, roadEnds, geo["properties"]["kind"].str);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            m.transform.localScale = Vector3.one;
        }
    }
}
