SELECT
	  Deals.DealNumber, 
	  Deals.SellerName,
	  Deals.SellerInn,
	  Deals.BuyerName,
	  Deals.BuyerInn,
	  Deals.DealDate,
	  Deals.WoodVolumeBuyer,
	  Deals.WoodVolumeSeller
  FROM [Parser].[dbo].[JSONDatas]
	 Cross Apply OpenJSON([Data], '$.content')
  with(
		  DealNumber NVARCHAR(100) '$.DealNumber', 
		  SellerName NVARCHAR(100) '$.SellerName',
        SellerInn NVARCHAR(20) '$.SellerInn',
        BuyerName NVARCHAR(100) '$.BuyerName',
        BuyerInn NVARCHAR(20) '$.BuyerInn',
        DealDate DATETIME2 '$.DealDate',
        WoodVolumeBuyer FLOAT'$.WoodVolumeBuyer',
        WoodVolumeSeller FLOAT'$.WoodVolumeSeller'		 
  ) as Deals 