SELECT RoadmapName,
	CAST(REPLACE(CAST([XML] AS VARCHAR(MAX)), 'encoding="utf-16"', 'encoding="utf-8"')AS XML)
  FROM [ElmerFudd].[dbo].[RoadmapGroups]
  where RoadmapGroupId=40