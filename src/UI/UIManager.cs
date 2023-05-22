﻿using BoneLib.Nullables;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class UIManager
    {
        internal static readonly string propMarkerBarcode = "NotEnoughPhotons.MonoDirector.Spawnable.UIPropMarker";
        internal static readonly string infoInterfaceBarcode = "NotEnoughPhotons.MonoDirector.Spawnable.InformationInterface";

        internal static List<AssetPoolee> Warmup(string barcode, int size, bool startActive = false)
        {
            List<AssetPoolee> cache = new List<AssetPoolee>();

            for (int i = 0; i < size; i++)
            {
                SpawnableCrateReference reference = new SpawnableCrateReference(barcode);

                Spawnable spawnable = new Spawnable()
                {
                    crateRef = reference
                };

                AssetSpawner.Register(spawnable);
                NullableMethodExtensions.PoolManager_Spawn(spawnable, default, default, null, false, null, new Action<GameObject>((obj) => CreateObject(ref cache, obj, startActive)));
            }

            return cache;
        }

        private static void CreateObject(ref List<AssetPoolee> cache, GameObject obj, bool startActive = false)
        {
            obj.SetActive(startActive);
            cache.Add(obj.GetComponent<AssetPoolee>());
        }
    }
}