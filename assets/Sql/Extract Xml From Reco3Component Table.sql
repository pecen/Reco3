SELECT ComponentId, PDNumber,
	CAST(REPLACE(CAST([XML] AS VARCHAR(MAX)), 'encoding="utf-16"', 'encoding="utf-8"')AS XML)
  FROM [ElmerFudd].[dbo].[Reco3Component]
  where PDNumber=2713364