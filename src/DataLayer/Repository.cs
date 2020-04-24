using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Reco3Common.Reco3_Enums;

namespace DataLayer.Database
{
	public class Reco3ComponentRepository : IPagination<Reco3Component>
	{
		DatabaseContext dbx = new DatabaseContext();

		public IQueryable<Reco3Component> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords,
			out int recordsFiltered)
		{
			/*
			var data = dbx.Reco3Components.Select(PDNumber => dbx.Reco3Components[PDNumber], DownloadedTimestamp => data[DownloadedTimestamp],
				Component_TypeStr => data[Component_TypeStr], ComponentId => data[ComponentId]);
			*/
			var data = dbx.Reco3Components.AsQueryable();
			totalRecords = data.Count();

			if (!string.IsNullOrEmpty(filter))
			{
				data = data.Where(x => x.PDNumber.ToUpper().Contains(filter.ToUpper()));
			}

			//data = data.Select(t => new { t.PDNumber, t.DownloadedTimestamp, t.Component_TypeStr })
			recordsFiltered = data.Count();
			data = data
				.OrderBy(x => x.PDNumber)
				.Skip((initialPage * pageSize))
				.Take(pageSize);


			/*
			{ "data": "PDNumber", "title": "PD#"  },
			{ "data": "DownloadedTimestamp", "title": "Added"  },
			{ "data": "Component_TypeStr", "title": "Type" },
			{ "data": "ComponentId", "visible": false }

	*/

			return data;
		}

	}

    public class Co2RoadmapRepository : IPagination<Roadmap>
    {
        DatabaseContext dbx = new DatabaseContext();

        public IQueryable<Roadmap> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords,
            out int recordsFiltered)
        {
            /*
			var data = dbx.Reco3Components.Select(PDNumber => dbx.Reco3Components[PDNumber], DownloadedTimestamp => data[DownloadedTimestamp],
				Component_TypeStr => data[Component_TypeStr], ComponentId => data[ComponentId]);
			*/
            var data = dbx.Roadmaps.AsQueryable();
            totalRecords = data.Count();

            if (!string.IsNullOrEmpty(filter))
            {
                data = data.Where(x => x.RoadmapName.ToUpper().Contains(filter.ToUpper()));
            }

            //data = data.Select(t => new { t.PDNumber, t.DownloadedTimestamp, t.Component_TypeStr })
            recordsFiltered = data.Count();
            data = data
                .OrderBy(x => x.RoadmapId)
                .Skip((initialPage * pageSize))
                .Take(pageSize);
            return data;
        }
    }

    public class Co2RoadmapGroupRepository : IPagination<RoadmapGroupDTO>
    {
        DatabaseContext dbx = new DatabaseContext();

        public IQueryable<RoadmapGroupDTO> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords, out int recordsFiltered)
        {
            try
            {
                
                var data = dbx.RoadmapGroups
                               .Select(p => new RoadmapGroupDTO()
                               {
                                   RoadmapGroupId = p.RoadmapGroupId,
                                   OwnerSss = p.OwnerSss,
                                   RoadmapName = p.RoadmapName,
                                   Protected = p.Protected,
                                   CreationTime = p.CreationTime,
                                   StartYear = p.StartYear,
                                   EndYear = p.EndYear,
                                   XML = ""
                               }).AsEnumerable();
                
                totalRecords = data.Count();

                if (!string.IsNullOrEmpty(filter))
                {
                    data = data.Where(x => x.RoadmapName.ToUpper().Contains(filter.ToUpper()));
                }

                //data = data.Select(t => new { t.PDNumber, t.DownloadedTimestamp, t.Component_TypeStr })
                recordsFiltered = data.Count();
                data = data
                    .OrderBy(x => x.RoadmapGroupId)
                    .Skip((initialPage * pageSize))
                    .Take(pageSize);
                
                return data.AsQueryable();
            }
            catch(Exception ex)
            {
                int n = 0;
            }
            totalRecords = 0;
            recordsFiltered = 0;

            return null;
        }
    }
    /*
    public class SimulationsRepository : IPagination<SimulationJob>
    {
        DatabaseContext dbx = new DatabaseContext();

        public IQueryable<SimulationJob> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords,
            out int recordsFiltered)
        {
            var data = dbx.SimulationJob.AsQueryable();
            totalRecords = data.Count();

            if (!string.IsNullOrEmpty(filter))
            {
                data = data.Where(x => x.Name.ToUpper().Contains(filter.ToUpper()));
            }

            //data = data.Select(t => new { t.PDNumber, t.DownloadedTimestamp, t.Component_TypeStr })
            recordsFiltered = data.Count();
            data = data
                .OrderBy(x => x.SimulationJobId)
                .Skip((initialPage * pageSize))
                .Take(pageSize);
            return data;
        }
    }
    */
    public class ImprovementsRepository : IPagination<Reco3Improvement>
    {
        DatabaseContext dbx = new DatabaseContext();

        public IQueryable<Reco3Improvement> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords,
            out int recordsFiltered)
        {
            /*
			var data = dbx.Reco3Components.Select(PDNumber => dbx.Reco3Components[PDNumber], DownloadedTimestamp => data[DownloadedTimestamp],
				Component_TypeStr => data[Component_TypeStr], ComponentId => data[ComponentId]);
			*/
            var data = dbx.Reco3Improvements
                .Include("Reco3Component")
                //.Include("Reco3Condition")
                .AsQueryable();
            totalRecords = data.Count();

            if (!string.IsNullOrEmpty(filter))
            {
                data = data.Where(x => x.Name.ToUpper().Contains(filter.ToUpper()));
            }

            //data = data.Select(t => new { t.PDNumber, t.DownloadedTimestamp, t.Component_TypeStr })
            recordsFiltered = data.Count();
            data = data
                .OrderBy(x => x.ImprovementId)
                .Skip((initialPage * pageSize))
                .Take(pageSize);
            return data;
        }
    }

    public class ConditionsRepository : IPagination<Reco3Condition>
    {
        DatabaseContext dbx = new DatabaseContext();
        int _improvementId;
        public ConditionsRepository(string improvementId)
        {
            _improvementId = Convert.ToInt32(improvementId);
        }
        public IQueryable<Reco3Condition> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords, out int recordsFiltered)
        {
            Reco3Improvement improvement = dbx.Reco3Improvements
                                  .Include("Conditions")
                                  .Include("Conditions.ConditionalReco3Component")
                                  .Include("Conditions.Reco3Tag")
                                  .Where(x => x.ImprovementId == _improvementId)
                                        .First();
            if (improvement == null)
            {
                totalRecords = 0;
                recordsFiltered = 0;
                return null;
            }
            var data = improvement.Conditions.AsQueryable();
            totalRecords = data.Count();
            recordsFiltered = data.Count();
            data = data
                .OrderBy(x => x.Reco3ConditionId)
                .Skip((initialPage * pageSize))
                .Take(pageSize);
            
            return data;
        }
    }

    public class Reco3IntroductionPointRepository : IPagination<Reco3IntroductionPoint>
    {
        DatabaseContext dbx = new DatabaseContext();

        public IQueryable<Reco3IntroductionPoint> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords, out int recordsFiltered)
        {
            var data = dbx.IntroductionPoints.AsQueryable();
            totalRecords = data.Count();
            recordsFiltered = data.Count();
            data = data
                .OrderBy(x => x.IntroductionDate)
                .Skip((initialPage * pageSize))
                .Take(pageSize);

            return data;
        }
    }

    public class Reco3UserRepository : IPagination<Sec_User>
    {
        DatabaseContext dbx = new DatabaseContext();

        public IQueryable<Sec_User> GetPaginated(string filter, int initialPage, int pageSize, out int totalRecords, out int recordsFiltered)
        {
            
            var data = dbx.Sec_Users.AsQueryable();
            totalRecords = data.Count();
            recordsFiltered = data.Count();
            data = data
                .OrderBy(x => x.UserId)
                .Skip((initialPage * pageSize))
                .Take(pageSize);

            return data;
        }
    }



}
