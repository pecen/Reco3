using DataLayer.Database;
using Reco3Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Manager
{
    public class RoadmapManager
    {
        public DatabaseContext DbCtx { get; set; }

        public bool SaveRoadmap(int nStartYear, int nEndYear, string Alias, int nRoadmapGroupId)
        {
            try
            {
                // Get the roadmap, create if missing
                RoadmapGroup map = GetRoadmapGroup(nRoadmapGroupId, true);
                if (map!=null)
                {
                    map.StartYear = nStartYear;
                    map.EndYear = nEndYear;
                    map.RoadmapName = Alias;
                    // If the roadmapgroup is newly created, add it to the database
                    if (map.RoadmapGroupId == -1)
                    {
                        DbCtx.RoadmapGroups.Add(map);
                    }
                    DbCtx.SaveChanges();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public bool LockAndCreate(int nRoadmapGroupId)
        {
            try
            {
                // Get the roadmap, do not create if missing
                RoadmapGroup mapGroup = GetRoadmapGroup(nRoadmapGroupId, false);
                if (mapGroup != null)
                {

                    // Get the submaps (one per year in the roadmapgroup)
                    List<Roadmap> maps = GetRoadmaps(mapGroup.RoadmapGroupId);
                    if (maps != null)
                    {
                        // This should not be the case!
                    }
                    
                    int nCurrentYear = mapGroup.StartYear;
                    do
                    {
                        Roadmap tempMap = maps.Find(x => x.CurrentYear == nCurrentYear);
                        if (tempMap == null)
                        {
                            // This creates a new roadmap, with all properties from the RoadmapGroup 
                            //  except for the xml, and that the start/end-year is set to nCurrentYear
                            DbCtx.Roadmaps.Add(new Roadmap(mapGroup, nCurrentYear));
                        }
                    } while (++nCurrentYear <= mapGroup.EndYear);
                    
                    DbCtx.SaveChanges();
                    return true;
                }
                return false;
            }
            catch 
            {

            }
            return false;
        }
        public RoadmapGroup GetRoadmapGroup(int nGroupId, bool bCreateIfMissing)
        {
            try
            {
                RoadmapGroup map = DbCtx
                    .RoadmapGroups
                    .Include("Roadmaps")
                    .Where(x=>x.RoadmapGroupId == nGroupId)
                        .FirstOrDefault();
                if ((map==null) &&
                    (bCreateIfMissing==true))
                {
                    map = new RoadmapGroup();
                }
                return map;
            }
            catch
            {

            }
            return null;
        }

        public List<Roadmap> GetRoadmaps(int nGroupId)
        {
            try
            {
                List<Roadmap> maps = DbCtx
                                        .Roadmaps
                                        .Where(x => x.RoadmapGroupId == nGroupId)
                                            .ToList();
                return maps;
            }
            catch
            {
            }
            return null;
        }

        public bool PopulateRoadmaps(ref List<Roadmap> maps)
        {
            try
            {
                // This could take forever so use it only when needed
                foreach(Roadmap map in maps)
                {
                    map.VehicleCount = DbCtx
                                        .Vehicle
                                        .Where(x=> x.GroupId == map.RoadmapId)
                                            .Count();
                    map.VSumCount = DbCtx
                                        .VSum
                                        .Where(x => x.SimulationId == map.RoadmapId)
                                            .Count();


                }
                return true;
            }
            catch 
            {
            }
            return false;
        }

        public bool GenerateRoadmapsForGroup(RoadmapGroup group)
        {
            try
            {
                List<Roadmap> maps = GetRoadmaps(group.RoadmapGroupId);

            }
            catch 
            {
            }
            return false;
        }

        public bool DeleteRoadmap(int nRoadmapId)
        {
            try
            {

            }
            catch
            {
            }
            return false;
        }
    }
}
