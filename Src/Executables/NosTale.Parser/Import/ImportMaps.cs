using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;

namespace NosTale.Parser.Import
{
    public class ImportMaps : IImport
    {
        private readonly ImportConfiguration _configuration;

        public ImportMaps(ImportConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string FileMapIdDat => Path.Combine(_configuration.DatFolder, "MapIDData.dat");

        private string FileMapIdLang =>
            Path.Combine(_configuration.LangFolder, $"_code_{_configuration.Lang}_MapIDData.txt");

        public void Import()
        {
            var existingMaps = DAOFactory.MapDAO.LoadAll().Select(x => x.MapId).ToHashSet();

            var maps = new List<MapDTO>();
            var dictionaryId = new Dictionary<int, string>();
            var dictionaryMusic = new Dictionary<int, int>();
            var dictionaryIdLang = new Dictionary<string, string>();

            var i = 0;
            using (var mapIdStream = new StreamReader(FileMapIdDat, Encoding.GetEncoding(1252)))
            {
                string line;
                while ((line = mapIdStream.ReadLine()) != null)
                {
                    var values = line.Split(' ');
                    if (values.Length <= 1) continue;

                    if (!int.TryParse(values[0], out var mapId)) continue;

                    if (!dictionaryId.ContainsKey(mapId)) dictionaryId.Add(mapId, values[4]);
                }
            }

            using (var mapIdLangStream = new StreamReader(FileMapIdLang, Encoding.GetEncoding(1252)))
            {
                string line;
                while ((line = mapIdLangStream.ReadLine()) != null)
                {
                    var linesave = line.Split('\t');
                    if (linesave.Length <= 1 || dictionaryIdLang.ContainsKey(linesave[0])) continue;
                    dictionaryIdLang.Add(linesave[0], linesave[1]);
                }
            }

            foreach (var atPacket in _configuration.Packets.Where(o => o[0].Equals("at")))
            {
                if (atPacket.Length > 7 && !dictionaryMusic.ContainsKey(int.Parse(atPacket[2])))
                {
                    dictionaryMusic[int.Parse(atPacket[2])] = int.Parse(atPacket[7]);
                }
            }

            foreach (var file in new DirectoryInfo(_configuration.MapFolder).GetFiles())
            {
                addMap(short.Parse(file.Name), short.Parse(file.Name), File.ReadAllBytes(file.FullName));
            }

            void addMap(short mapId, short originalMapId, byte[] mapData)
            {
                string name = "";
                int music = 0;

                if (dictionaryId.ContainsKey(mapId) && dictionaryIdLang.ContainsKey(dictionaryId[mapId]))
                {
                    name = dictionaryIdLang[dictionaryId[mapId]];
                }

                if (dictionaryMusic.ContainsKey(mapId))
                {
                    music = dictionaryMusic[mapId];
                }

                if (existingMaps.Contains(mapId))
                {
                    return;
                }
                
                var map = new MapDTO
                {
                        Name        = name,
                        Music       = music,
                        MapId       = mapId,
                        GridMapId   = originalMapId,
                        Data        = mapData,
                        ShopAllowed = mapId == 147
                };
                
                if (maps.Contains(maps.FirstOrDefault(s => s.MapId == map.MapId)))
                {
                    return;
                }
                
                maps.Add(map);
                i++;
            }

            DAOFactory.MapDAO.Insert(maps);
            Logger.Log.InfoFormat($"{i} Maps parsed");
        }
    }
}