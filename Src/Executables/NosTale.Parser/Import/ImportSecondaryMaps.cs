using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTale.Parser.Import
{
    class ImportSecondaryMaps : IImport
    {
        private readonly ImportConfiguration _configuration;

        public ImportSecondaryMaps(ImportConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Import()
        {
            var existingMaps = DAOFactory.MapDAO.LoadAll().Select(x => x.MapId).ToHashSet();
            var maps = new List<MapDTO>();
            var i = 0;

            MapDTO map;

            if (existingMaps.Contains(2644))
            {
                map = DAOFactory.MapDAO.LoadById(2644);
                for (short a = 2645; a != 2649; a++)
                {
                    var name = (a - 2643);
                    addMap(a, $"Tour céleste {name}", map.Music, map.MapId, map.Data);
                }
            }

            if (existingMaps.Contains(135))
            {
                map = DAOFactory.MapDAO.LoadById(135);
                addMap(20049, map.Name, map.Music, map.MapId, map.Data);
            }

            if (existingMaps.Contains(137))
            {
                map = DAOFactory.MapDAO.LoadById(137);
                addMap(20034, map.Name, map.Music, map.MapId, map.Data);
            }

            if (existingMaps.Contains(139))
            {
                map = DAOFactory.MapDAO.LoadById(139);
                addMap(20036, map.Name, map.Music, map.MapId, map.Data);
            }

            if (existingMaps.Contains(141))
            {
                map = DAOFactory.MapDAO.LoadById(141);
                addMap(20048, map.Name, map.Music, map.MapId, map.Data);
            }

            void addMap(short mapId, string name, int music, short originalMapId, byte[] mapData)
            {
                if (existingMaps.Contains(mapId))
                {
                    return;
                }

                var newmap = new MapDTO
                {
                    Name = name,
                    Music = music,
                    MapId = mapId,
                    GridMapId = originalMapId,
                    Data = mapData,
                    ShopAllowed = mapId == 147
                };

                if (maps.Contains(maps.FirstOrDefault(s => s.MapId == newmap.MapId)))
                {
                    return;
                }

                maps.Add(newmap);
                i++;
            }

            DAOFactory.MapDAO.Insert(maps);
            Logger.Log.InfoFormat($"{i} Secondary Maps parsed");
        }
    }
}
