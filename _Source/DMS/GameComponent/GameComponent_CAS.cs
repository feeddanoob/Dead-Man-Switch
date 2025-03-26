using Microsoft.SqlServer.Server;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Noise;

namespace DMS
{
    public class GameComponent_CAS : GameComponent
    {
        private static GameComponent_CAS instance;

        public GameComponent_CAS() => instance = null;
        public GameComponent_CAS(Game game) => instance = null;

        public override void FinalizeInit() => instance = this;

        private List<AirSupportData> datas = new List<AirSupportData>(), tempRemoveDatas = new List<AirSupportData>();

        public static void AddData(AirSupportData data)
        {
            if (instance == null)
            {
                return;
            }
            instance.datas.Add(data);
            instance.datas.SortBy(d => d.triggerTick);
        }

        //方向性來襲
        public static void AddData(AirSupportData data, float angle, float variant = 0)
        {
            float v = Rand.Range(-variant, variant);
            data.origin = Position(angle + v, data.map).ToVector3();
            AddData(data);
        }

        private static IntVec3 Position(float angle, Map map)
        {
            float theta = Mathf.Deg2Rad * angle; // 角度轉換為弧度

            // 計算方向向量
            float dx = Mathf.Sin(theta);
            float dy = -Mathf.Cos(theta);

            float x = map.Center.x;
            float y = map.Center.y;

            while (x >= 0 && x < map.Size.x && y >= 0 && y < map.Size.y)
            {
                x += dx;
                y += dy;
            }

            int edgeX = Mathf.RoundToInt(x - dx);
            int edgeY = Mathf.RoundToInt(y - dy);

            return new IntVec3(edgeX, 0, edgeY);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (datas.Any())
            {
                foreach (AirSupportData data in datas)
                {
                    data.TickEffecter();
                    if (data.triggerTick == Find.TickManager.TicksGame)
                    {
                        data.Trigger();
                    }
                    if (data.triggerTick + data.def.postTriggerTick == Find.TickManager.TicksGame)
                    {
                        tempRemoveDatas.Add(data);
                    }
                }
                foreach (AirSupportData data in tempRemoveDatas)
                {
                    datas.Remove(data);
                }
                tempRemoveDatas.Clear();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref datas, "datas", LookMode.Deep);
        }
    }
}