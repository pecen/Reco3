SELECT VehicleId,
	CAST(REPLACE(CAST([XML] AS VARCHAR(MAX)), 'encoding="utf-16"', 'encoding="utf-8"')AS XML)
  FROM [ElmerFudd].[dbo].[Vehicle]
  where GroupId=61