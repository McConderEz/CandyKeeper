using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class QueryWindow : UserControl
{

	private readonly IDatabaseService _databaseService;
	private List<QueryResult> _queryResults;
	private QueryResult _selectedQuery;


	public QueryWindow()
	{
		InitializeComponent();
		_databaseService = App.Host.Services.GetRequiredService<IDatabaseService>();
		_queryResults = new List<QueryResult>();
		Loaded += async (s, e) => await LoadDataAsync();
	}


	private async Task LoadDataAsync()
	{
		var queries = await GetSampleQueries();

		foreach (var query in queries)
		{
			_queryResults.Add(new QueryResult
			{
				Description = query.Item1,
				QueryName = query.Item2
			});
		}
		
		QueryResultsListBox.ItemsSource = _queryResults;
	}

	private async Task<List<(string, string)>> GetSampleQueries()
	{
		return new List<(string, string)>
		{
			("Продукты по название", $"""
			                          select	p.Name as ProductName,
			                                  s.Name as StoreName,
			                          		d.Name as DistrictName,
			                          		c.Name as CityName,
			                          		Price, 
			                          		Volume 
			                          		from ProductForSales as pfs 
			                          		Join Products as p On ProductId = p.Id
			                          		Join Stores as s On StoreId = s.Id
			                          		Join Districts as d On s.DistrictId = d.Id
			                          		Join Cities as c On d.CityId = c.Id
			                          	where p.Name = CONVERT(NVARCHAR, @StringArgument);
			                          """),
			("Магазины по типу собственности", $"""
			                                    select s.Name as StoreName,
			                                    	   o.Name as OwnershipTypeName, 
			                                    	   c.Name as CityName, 
			                                    	   d.Name as DistrictName, 
			                                    	   Phone, 
			                                    	   StoreNumber, 
			                                    	   CAST(YearOfOpened as DATE) as YearOfOpened
			                                    	   from Stores as s
			                                    	   Join OwnershipTypes as o On OwnershipTypeId = o.Id
			                                    	   Join Districts as d On DistrictId = d.Id
			                                    	   Join Cities as c On d.CityId = c.Id
			                                    	where o.Name = CONVERT(NVARCHAR, @StringArgument)
			                                    """),
			("Магазины открытые с определённой даты", $"""
			                                           select s.Name as StoreName,
			                                           	   o.Name as OwnershipTypeName, 
			                                           	   c.Name as CityName, 
			                                           	   d.Name as DistrictName, 
			                                           	   Phone, 
			                                           	   StoreNumber, 
			                                           	   CAST(YearOfOpened as DATE) as YearOfOpened
			                                           	   from Stores as s
			                                           	   Join OwnershipTypes as o On OwnershipTypeId = o.Id
			                                           	   Join Districts as d On DistrictId = d.Id
			                                           	   Join Cities as c On d.CityId = c.Id
			                                           	where YearOfOpened > @DateArgument1
			                                           """),
			("Магазины открытые в определённый временной промежуток", $"""
			                                                           select store.Name,
			                                                           	   supplier.Name, 
			                                                           	   CAST(DeliveryDate as Date) as DeliveryDate
			                                                           	   from ProductDeliveries as pd 
			                                                           	   Join Stores as store on StoreId = store.Id
			                                                           	   Join Suppliers as supplier on SupplierId = supplierId
			                                                           	where DeliveryDate between @DateArgument1 and @DateArgument2
			                                                           """),
			("Вывод продуктов", $"""
			                     select	p.Name as ProductName,
			                             s.Name as StoreName,
			                     		d.Name as DistrictName,
			                     		c.Name as CityName,
			                     		pt.Name as ProductTypeName,
			                     		pcg.Name as PackagingName,
			                     		Price, 
			                     		Volume 
			                     		from ProductForSales as pfs 
			                     		Join Products as p On ProductId = p.Id
			                     		Join Stores as s On StoreId = s.Id
			                     		Join ProductTypes as pt On p.ProductTypeId = pt.Id
			                     		Join Packaging as pcg On PackagingId = pcg.Id
			                     		Join Districts as d On s.DistrictId = d.Id
			                     		Join Cities as c On d.CityId = c.Id
			                     """),
			("Вывод магазинов", $"""
			                     select s.Name as StoreName,
			                     	   o.Name as OwnershipTypeName, 
			                     	   c.Name as CityName, 
			                     	   d.Name as DistrictName, 
			                     	   Phone, 
			                     	   StoreNumber,
			                     	   s.NumberOfEmployees,
			                     	   CAST(YearOfOpened as DATE) as YearOfOpened
			                     	   from Stores as s
			                     	   Join OwnershipTypes as o On OwnershipTypeId = o.Id
			                     	   Join Districts as d On DistrictId = d.Id
			                     	   Join Cities as c On d.CityId = c.Id
			                     """),
			("Вывод поставщиков", $"""
			                       select supplier.Name as SupplierName, 
			                       	   supplier.Phone, 
			                       	   city.Name as CityName, 
			                       	   ownershipType.Name as OwnershipTypeName
			                       	   from Suppliers as supplier
			                       	   Join Cities as city on supplier.CityId = city.Id
			                       	   Join OwnershipTypes as ownershipType on supplier.OwnershipTypeId = ownershipType.Id
			                       """),
			("Вывод магазинов(левое соединение)", $"""
			                                       select s.Name as StoreName,
			                                       	   o.Name as OwnershipTypeName, 
			                                       	   c.Name as CityName, 
			                                       	   d.Name as DistrictName, 
			                                       	   Phone, 
			                                       	   StoreNumber,
			                                       	   s.NumberOfEmployees,
			                                       	   CAST(YearOfOpened as DATE) as YearOfOpened
			                                       	   from Stores as s
			                                       	   Left Join OwnershipTypes as o On OwnershipTypeId = o.Id
			                                       	   Left Join Districts as d On DistrictId = d.Id
			                                       	   Left Join Cities as c On d.CityId = c.Id
			                                       """),
			("Вывод поставщиков(правое соединение)", $"""
			                                          select supplier.Name, 
			                                          	   supplier.Phone, 
			                                          	   city.Name as CityName, 
			                                          	   ownershipType.Name as OwnershipTypeName
			                                          	   from Suppliers as supplier
			                                          	   Right Join Cities as city on supplier.CityId = city.Id
			                                          	   Right Join OwnershipTypes as ownershipType on supplier.OwnershipTypeId = ownershipType.Id
			                                          """),
			("Вывод всех магазинов с подсчётом поставщиков и продуктов открытых в опред. промежуток времени", $"""
				 WITH FilteredStore as (
				 	select Name, 
				 		   StoreNumber, 
				 		   Phone, 
				 		   DistrictId, 
				 		   OwnershipTypeId, 
				 		   Cast(YearOfOpened as Date) as YearOfOpened,
				 		   NumberOfEmployees,
				 		   (select Count(ssLink.StoresId) from StoreSupplierLink as ssLink) as SuppliersCount,
				 		   (select Count(pfs.StoreId) from ProductForSales as pfs
				 					where pfs.StoreId = s.Id) as ProductForSalesCount 
				 		   from Stores as s
				 		where YearOfOpened between @DateArgument1 and @DateArgument2
				 )

				 select fs.Name,	
				        fs.Phone, 
				 	   fs.StoreNumber, 
				 	   fs.YearOfOpened,
				 	   fs.NumberOfEmployees, 
				 	   fs.SuppliersCount, 
				 	   fs.ProductForSalesCount,
				 	   d.Name as DistrictName,
				 	   c.Name as CityName,
				 	   ot.Name as OwnershipTypeName
				 	   from FilteredStore as fs
				 	   left join Districts as d on fs.DistrictId = d.Id
				 	   left join Cities as c on d.CityId = c.Id
				 	   left join OwnershipTypes as ot on fs.OwnershipTypeId = ot.Id
				 """),
			("Количество продукции, средняя цена, средний объём, суммарная цена, суммарный объём", $"""
				 select 
				 	count(*) as TotalProductForSales,
				 	avg(pfs.Price) as AveragePriceProductForSale,
				 	avg(pfs.Volume) as AverageVolumeProductForSale,
				 	sum(pfs.Price) as TotalPriceProductForSale,
				 	sum(pfs.Volume) as TotalVolumeProductForSale	
				 	from ProductForSales as pfs
				 """),
			("Магазины с средней ценой на товары меньше или равны указанному значению", $"""
				 select s.Name, 
				 	   s.StoreNumber,
				 	   s.Phone, 
				 	   d.Name as DistrictName,
				 	   c.Name as CityName,
				 	   ot.Name as OwnershipTypeName,
				 	   (select avg(pfs.Price) from ProductForSales as pfs
				 							  where pfs.StoreId = s.Id) as AveragePriceOnProducts
				 	   from Stores as s
				 	   left join Districts as d on s.DistrictId = d.Id
				 	   left join Cities as c on d.CityId = c.Id
				 	   left join OwnershipTypes as ot on s.OwnershipTypeId = ot.Id
				 	where
				 		(
				 		select avg(pfs.Price) from ProductForSales as pfs
				 							  where pfs.StoreId = s.Id) <= @NumberArgument
				 """),
			("Магазины с самыми минимальными средними ценами на продукты в каждом городе", $"""
				 with StoreAveragePrices as (
				     select 
				         s.Id as StoreId,
				         s.Name as StoreName, 
				         s.StoreNumber,
				         s.Phone, 
				         d.Name as DistrictName,
				         c.Name as CityName,
				         ot.Name as OwnershipTypeName,
				         avg(pfs.Price) as AveragePriceOnProducts
				     from 
				         Stores as s
				     left join 
				         Districts as d on s.DistrictId = d.Id
				     left join 
				         Cities as c on d.CityId = c.Id
				     left join 
				         OwnershipTypes as ot on s.OwnershipTypeId = ot.Id
				     left join 
				         ProductForSales as pfs on pfs.StoreId = s.Id
				     group by 
				         s.Id, s.Name, s.StoreNumber, s.Phone, d.Name, c.Name, ot.Name
				 )
				 , CityMinimalPrices as (
				     select 
				         CityName,
				         min(AveragePriceOnProducts) as MinAveragePrice
				     from 
				         StoreAveragePrices
				     group by 
				         CityName
				 )
				 select 
				     sap.StoreName, 
				     sap.StoreNumber,
				     sap.Phone, 
				     sap.DistrictName,
				     sap.CityName,
				     sap.OwnershipTypeName,
				     sap.AveragePriceOnProducts
				 from 
				     StoreAveragePrices as sap
				 join CityMinimalPrices as cmp on sap.CityName = cmp.CityName AND sap.AveragePriceOnProducts = cmp.MinAveragePrice
				 order by 
				     MinAveragePrice;
				 """),
			("Поставщик с максимальный общим ценником на доставку за определённый промежуток времени.", $"""
				 with SupplierTotalPrices as (
				     select 
				         supplier.Name as SupplierName,
				         SUM(pfs.Price) as TotalPrice,
				         COUNT(pd.Id) as DeliveryCount
				     from 
				         ProductDeliveries as pd
				     LEFT JOIN 
				         Suppliers as supplier on pd.SupplierId = supplier.Id
				     LEFT JOIN 
				         ProductForSales as pfs on pfs.ProductDeliveryId = pd.Id
				     where 
				         pd.DeliveryDate BETWEEN @DateArgument1 AND @DateArgument2
				     group by 
				         supplier.Name
				 ),
				 MaxTotalPriceSupplier as (
				     select 
				         TOP 1 SupplierName,
				         TotalPrice
				     from 
				         SupplierTotalPrices
				     order by 
				         TotalPrice desc
				 )
				 select 
				     stp.SupplierName,
				     stp.TotalPrice,
				     stp.DeliveryCount
				 from 
				     SupplierTotalPrices stp
				 inner join 
				     MaxTotalPriceSupplier mtps on stp.SupplierName = mtps.SupplierName
				 order by 
				     stp.SupplierName;
				 """),
			("Выборка магазинов, где средняя цена на товары выше средней цены на товары за все магазины", $"""
				 select
				     s.StoreNumber,
				     s.Name as StoreName,
				     s.Phone as StorePhone,
				     ot.Name as OwnershipType,
				     d.Name as District,
				     c.Name as City,
				     (
				         select AVG(pfs.Price)
				         from ProductForSales pfs
				         where pfs.StoreId = s.Id
				     ) as AveragePriceInStore,
				     (
				         select AVG(pfs.Price)
				         from ProductForSales pfs
				     ) as OverallAveragePrice
				 from
				     Stores s
				     JOIN OwnershipTypes ot on s.OwnershipTypeId = ot.Id
				     JOIN Districts d on s.DistrictId = d.Id
				     JOIN Cities c on d.CityId = c.Id
				 where
				     (
				         select AVG(pfs.Price)
				         from ProductForSales pfs
				         where pfs.StoreId = s.Id
				     ) > (
				         select AVG(pfs.Price)
				         from ProductForSales pfs
				     )
				 order by
				     AveragePriceInStore desc
				 """),
			("Подробная информация о продажах продуктов в различных магазинах, включая среднюю цену продукта, дату доставки и информацию о поставщике",
				$"""
				 select
				     ps.ProductId,
				     p.Name as ProductName,
				     ps.StoreId,
				     s.Name as StoreName,
				     ps.Price,
				     (
				         select AVG(ps2.Price)
				         from ProductForSales ps2
				         where ps2.ProductId = ps.ProductId
				     ) as AverageProductPrice,
				     pd.DeliveryDate,
				     su.Name as SupplierName,
				     su.Phone as SupplierPhone
				 from
				     ProductForSales ps
				 INNER JOIN
				     Stores s on ps.StoreId = s.Id
				 INNER JOIN
				     Products p on ps.ProductId = p.Id
				 INNER JOIN
				     ProductDeliveries pd on ps.ProductDeliveryId = pd.Id
				 INNER JOIN
				     Suppliers su on pd.SupplierId = su.Id
				 order by
				     ps.Price desc
				 """),
			("Вывод магазинов в выбранном диапазоне дат", $"""
			                                               select s.Name as StoreName,
			                                               	   s.StoreNumber,
			                                               	   s.Phone, 
			                                               	   cast(s.YearOfOpened as date) as YearOfOpened,
			                                               	   d.Name as DistrictName,
			                                               	   c.Name as CityName,
			                                               	   ot.Name as OwnershipTypeName
			                                               	   from Stores as s
			                                               	   left join Districts as d on s.DistrictId = d.Id
			                                               	   left join Cities as c on d.CityId = c.Id
			                                               	   left join OwnershipTypes as ot on OwnershipTypeId = ot.Id
			                                               	 where s.YearOfOpened IN (Select YearOfOpened from Stores where YearOfOpened > @DateArgument1)
			                                               """),
			("Вывод магазинов, исключая диапазон дат", $"""
			                                            select s.Name as StoreName,
			                                            	   s.StoreNumber,
			                                            	   s.Phone, 
			                                            	   cast(s.YearOfOpened as date) as YearOfOpened,
			                                            	   d.Name as DistrictName,
			                                            	   c.Name as CityName,
			                                            	   ot.Name as OwnershipTypeName
			                                            	   from Stores as s
			                                            	   left join Districts as d on s.DistrictId = d.Id
			                                            	   left join Cities as c on d.CityId = c.Id
			                                            	   left join OwnershipTypes as ot on OwnershipTypeId = ot.Id
			                                            	 where s.YearOfOpened NOT IN (Select YearOfOpened from Stores where YearOfOpened > @DateArgument1)
			                                            """),
			("Вывод продуктов с комментарием к цене", $"""
			                                           select p.Name,		
			                                           	   pfs.Volume, 
			                                           	   (case when pfs.Price > 100000 then N'Дорого'
			                                           		else N'Нормально'
			                                           		end
			                                           	   ) as Comment, 
			                                           	   store.Name as StoreName,
			                                           	   supplier.Name as SupplierName
			                                           	   from ProductForSales as pfs
			                                           	   left join Stores as store on StoreId = store.Id
			                                           	   left join ProductDeliveries as pd on pfs.ProductDeliveryId = pd.Id
			                                           	   left join Suppliers as supplier on pd.SupplierId = supplier.Id
			                                           	   left join Products as p on pfs.ProductId = p.Id
			                                           """),
			("Вывод конд. изделий соответствующих маске 'Торт'", $"""
			                                               select p.Name,		
			                                               	   pfs.Volume, 
			                                               	   pfs.Price,
			                                               	   store.Name as StoreName,
			                                               	   supplier.Name as SupplierName
			                                               	   from ProductForSales as pfs
			                                               	   left join Stores as store on StoreId = store.Id
			                                               	   left join ProductDeliveries as pd on pfs.ProductDeliveryId = pd.Id
			                                               	   left join Suppliers as supplier on pd.SupplierId = supplier.Id
			                                               	   left join Products as p on pfs.ProductId = p.Id
			                                               	where p.Name Like N'%Торт%'
			                                               """),
			("Вывод полной информации о продуктах(объединение)", $"""
			                                                      select
			                                                          ps.ProductId,
			                                                          p.Name as ProductName,
			                                                          ps.StoreId,
			                                                          s.Name as StoreName,
			                                                          s.YearOfOpened,
			                                                          s.NumberOfEmployees,
			                                                          s.Phone as StorePhone,
			                                                          ps.Price,
			                                                          pd.DeliveryDate,
			                                                          su.Name as SupplierName,
			                                                          su.Phone as SupplierPhone
			                                                      from ProductForSales as ps
			                                                       join
			                                                          Products as p on ps.ProductId = p.Id
			                                                       join
			                                                          Stores as s on ps.StoreId = s.Id
			                                                       join
			                                                          ProductDeliveries as pd on ps.ProductDeliveryId = pd.Id
			                                                       join
			                                                          Suppliers as su on pd.SupplierId = su.Id
			                                                      union
			                                                      select
			                                                          ps.ProductId,
			                                                          p.Name as ProductName,
			                                                          ps.StoreId,
			                                                          s.Name as StoreName,
			                                                          s.YearOfOpened,
			                                                          s.NumberOfEmployees,
			                                                          s.Phone as StorePhone,
			                                                          ps.Price,
			                                                          pd.DeliveryDate,
			                                                          su.Name as SupplierName,
			                                                          su.Phone as SupplierPhone
			                                                      from
			                                                          ProductForSales ps
			                                                       join
			                                                      Products as p on ps.ProductId = p.Id
			                                                       join
			                                                          Stores as s on ps.StoreId = s.Id
			                                                       join
			                                                          ProductDeliveries as pd on ps.ProductDeliveryId = pd.Id
			                                                       join
			                                                          Suppliers as su on pd.SupplierId = su.Id
			                                                      order by
			                                                          Price desc;
			                                                      """),
			("Вывод общей информации(запрос всего и в том числе)", $"""
			                                                        select
			                                                            case when grouping(s.Name) = 1 then N'Всего' else s.Name end as GroupName,
			                                                            count(pfs.Id) as TotalProducts
			                                                        from
			                                                            ProductForSales pfs
			                                                            LEFT JOIN Stores s on pfs.StoreId = s.Id
			                                                        group by
			                                                            grouping sets (
			                                                                (s.Name),
			                                                                ()
			                                                            )
			                                                        order by
			                                                            case when grouping(s.Name) = 1 then 1 else 2 end;
			                                                        """)
		};
	}
	
	
    private void OnQuerySelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
	    if(QueryResultsListBox.SelectedItem is QueryResult selectedQuery)
	    {
		    _selectedQuery = selectedQuery;
	    }
    }


    private async void OnExecuteQueryClick(object sender, RoutedEventArgs e)
    {
	    if (_selectedQuery != null)
	    {
		    var stringArg = StringArgumentTextBox.Text;
		    var numberArg = NumberArgumentTextBox.Value;
		    var dateArg1 = DateArgumentPicker1.SelectedDate;
		    var dateArg2 = DateArgumentPicker2.SelectedDate;

		    var parameters = new Dictionary<string, object>
		    {
			    { "@StringArgument", stringArg },
			    { "@NumberArgument", numberArg },
			    { "@DateArgument1", dateArg1 ?? (object)DBNull.Value },
			    { "@DateArgument2", dateArg2 ?? (object)DBNull.Value }
		    };

		    var result = await _databaseService.ExecuteQueriesAsync((_selectedQuery.Description, _selectedQuery.QueryName), parameters);

		    _selectedQuery.Result = result.Result;
		    ResultsDataGrid.ItemsSource = _selectedQuery.Result.DefaultView;
	    }
    }
}