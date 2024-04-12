﻿using System;
using System.Collections.Generic;
using System.Linq;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.State;
using SLZ.Marrow.Pool;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class PropMarkerManager
    {
        private static Dictionary<Prop, AssetPoolee> markers = new Dictionary<Prop, AssetPoolee>();
        private static List<AssetPoolee> loadedMarkerObjects = new List<AssetPoolee>();
        private static List<AssetPoolee> activeMarkers = new List<AssetPoolee>();

        public static void Initialize()
        {
            loadedMarkerObjects = WarehouseLoader.Warmup(WarehouseLoader.propMarkerBarcode, 32, false);

            Events.OnPropCreated += AddMarkerToProp;
            Events.OnPropRemoved += RemoveMarkerFromProp;

            Events.OnPlayStateSet += ShowMarkers;
        }

        public static void CleanUp()
        {
            Events.OnPropCreated -= AddMarkerToProp;
            Events.OnPropRemoved -= RemoveMarkerFromProp;

            Events.OnPlayStateSet -= ShowMarkers;

            markers.Clear();
            loadedMarkerObjects.Clear();
            activeMarkers.Clear();
        }

        public static void AddMarkerToProp(Prop prop)
        {
            if (markers.ContainsKey(prop))
            {
                return;
            }

            AssetPoolee asset = loadedMarkerObjects.FirstOrDefault((marker) => !activeMarkers.Contains(marker));

            asset.gameObject.SetActive(true);

            asset.transform.SetParent(prop.transform);
            asset.transform.localPosition = new Vector3(0f, 1.25f + asset.spawnableCrate.ColliderBounds.extents.y, 0f);

            markers.Add(prop, asset);
            activeMarkers.Add(asset);
        }

        public static void RemoveMarkerFromProp(Prop prop)
        {
            if (!markers.ContainsKey(prop))
            {
                return;
            }

            AssetPoolee marker = markers[prop];
            marker.Despawn();
            marker.gameObject.SetActive(false);
            marker.transform.parent = null;

            markers.Remove(prop);
            activeMarkers.Remove(marker);
        }

        private static void ShowMarkers(PlayState playState)
        {
            if(playState == PlayState.Preplaying || playState == PlayState.Prerecording)
            {
                foreach (var marker in activeMarkers)
                {
                    marker.gameObject.SetActive(false);
                }
            }

            if(playState == PlayState.Stopped)
            {
                foreach (var marker in activeMarkers)
                {
                    marker.gameObject.SetActive(true);
                }
            }
        }
    }
}
