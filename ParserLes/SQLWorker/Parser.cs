using Newtonsoft.Json;
using ParserLes.DataFolder;
using ParserLes.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ParserLes.SQLWorker
{
    class Parser  
    {
        DataBaseContext baseContext;

        List<Deal> dealsAll;
        static IRestResponse response;
        static string ProductParse { get; set; }
        static string requestUrl = "https://www.lesegais.ru/open-area/graphql";
        DateTime lastParseTime;
        bool isWorked;
        double tenMinuts;
        int pageSize ;
        int productsCounts ;
        int numberPage = 0;
        public Parser(int pageSize,  int minutese )
        {
            this.baseContext = new DataBaseContext();
            this.dealsAll = new List<Deal>();
            this.pageSize = pageSize;
            tenMinuts = minutese * 60;
            try
            {
                numberPage = this.baseContext.parserSettings.First().NumberPage;
            }
            catch
            {
                numberPage = 0;
                baseContext.parserSettings.Add(new ParserSettings { NumberPage = 0, ProductsCount = 0 });
                baseContext.SaveChanges();
            }
            GC.KeepAlive(this.baseContext);
        }


        public void ParseStart()
        {
            isWorked = true;
            FirstParse();
            GC.Collect();
            lastParseTime = DateTime.Now;
            while (isWorked)
            {
                if(DateTime.Now.Subtract(lastParseTime).TotalSeconds>= tenMinuts)
                {
                    SecondParse();
                    lastParseTime = DateTime.Now;
                }
            }

        }


        /// <summary>
        /// Записывает последнюю использываемую страницу
        /// </summary>
        /// <param name="numberPage"></param>
        private void SaveLastPage(int numberPage)
        {
            baseContext.parserSettings.First().NumberPage = numberPage;
            baseContext.SaveChanges();
        }

        /// <summary>
        /// Первоначальный парсинг
        /// </summary>
        private void FirstParse()
        {
            ResquestLoadGetDealsCount();
            int dbProductsCount = baseContext.parserSettings.First().ProductsCount;
            if (productsCounts <= dbProductsCount) return;
            while (productsCounts > 0)
            {
                productsCounts -= pageSize;
                ParseDeals(RequestLoadDeals(numberPage));
                numberPage++;
                SaveLastPage(numberPage);
            }
            AllDeals allProduct = new AllDeals();
            allProduct.content = dealsAll;
            JSONData jsData = new JSONData();
            jsData.Data = JsonConvert.SerializeObject(allProduct);
            baseContext.JSONData.Add(jsData);
            baseContext.SaveChanges();
        }

        /// <summary>
        /// Получаем колличество сделок на сайте
        /// </summary>
        void ResquestLoadGetDealsCount()
        {
            var client = new RestClient(requestUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("User-Agent", "C# App");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"query\": \"query SearchReportWoodDealCount($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\\n    total\\n    number\\n    size\\n    overallBuyerVolume\\n    overallSellerVolume\\n    __typename\\n  }\\n}\\n\",\r\n  \"variables\": {\r\n    \"size\": 0,\r\n    \"number\": 0,\r\n    \"filter\": null\r\n  },\r\n  \"operationName\": \"SearchReportWoodDealCount\"\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var responceContent = response.Content;
            responceContent = responceContent.Substring(responceContent.LastIndexOf("total"));
            responceContent = responceContent.Substring(0, responceContent.LastIndexOf("number"));
            GetTotal(responceContent);
        }

        /// <summary>
        /// Проверяем на вхождение 
        /// </summary>
        /// <param name="deals"></param>
         void ParseDeals(List<Deal> deals)
        {            
            for (int i = 0; i < deals.Count; i++)
            {
                if (IsContain(deals[i].DealNumber))
                {
                    deals.Remove(deals[i]);
                }
            }
            if (deals.Count != 0) dealsAll.AddRange(deals);
        }

        /// <summary>
        /// Получаем список 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private List<Deal> RequestLoadDeals(int number)
        {
            var client = new RestClient(requestUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("User-Agent", "C# App");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"query\": " +
                                "\"query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!])" +
                                " {\\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) " +
                                "{\\n   content " +
                                "{\\n   sellerName\\n sellerInn\\n buyerName\\n buyerInn\\n woodVolumeBuyer\\n woodVolumeSeller\\n dealDate\\n dealNumber\\n __typename\\n" +
                                "}\\n   __typename\\n  }\\n}\\n\",\r\n  \"variables\": {\r\n\"size\": " + pageSize.ToString() + ",\r\n \"number\": " + number.ToString() + ",\r\n\"filter\": null,\r\n\"orders\": null\r\n  },\r\n\"operationName\": \"SearchReportWoodDeal\"\r\n}",
                                ParameterType.RequestBody);
            response = client.Execute(request);

            var responce = response.Content;
            responce = responce.Remove(0, responce.IndexOf('['));
            responce = responce.Remove(responce.IndexOf(']') + 1);
            return JsonConvert.DeserializeObject<List<Deal>>(responce);
        }

        /// <summary>
        /// Получаем из строки количество Сделок
        /// </summary>
        /// <param name="total"></param>
        void GetTotal(string total)
        {
            StringBuilder sb = new StringBuilder(total);
            string count = "";
            for (int i = 0; i < sb.Length; i++)
            {
                if (char.IsDigit(sb[i]))
                {
                    count += sb[i].ToString();
                }
            }
            productsCounts = Convert.ToInt32(count);
            int dataBAseCount = 0;
            try
            {
                dataBAseCount = baseContext.parserSettings.First().ProductsCount;

            }
            catch { dataBAseCount = 0; }

            if (productsCounts > dataBAseCount)
            {
                baseContext.parserSettings.First().ProductsCount = productsCounts;
                baseContext.SaveChanges();
            }


            
        }

        /// <summary>
        /// Последуещий парсинг
        /// </summary>
        void SecondParse()
        {
            dealsAll = new List<Deal>();
            ResquestLoadGetDealsCount();
            int dbProductsCount = baseContext.parserSettings.First().ProductsCount;
            if(productsCounts <= dbProductsCount) return;

            int secProductsCounts = productsCounts - dbProductsCount + pageSize;
            pageSize = productsCounts - dbProductsCount;
            numberPage = productsCounts / pageSize - 1;
            numberPage -= 1;

            while (secProductsCounts > 0)
            {
                productsCounts -= pageSize;
                var newProducts = RequestLoadDeals(numberPage);
                for (int i = 0; i < newProducts.Count; i++)
                {
                    if (!IsContain(newProducts[i].DealNumber))
                    {
                        AddToDB(newProducts[i]);
                    }
                }
                if (newProducts.Count != 0) dealsAll.AddRange(newProducts);

                numberPage++;
                SaveLastPage(numberPage);
            }
            
        }

        /// <summary>
        /// проверяет на вхождение сделки в БД
        /// </summary>
        /// <param name="dealNumber"></param>
        /// <returns></returns>
        bool IsContain(string dealNumber)
        {
            string sqlExpression = $@"SELECT Deals.DealNumber FROM [Parser].[dbo].[JSONDatas] 
                                     Cross Apply OpenJSON([Data], '$.content') with (DealNumber NVARCHAR(100)
                                    '$.DealNumber', SellerName NVARCHAR(15) '$.SellerName') as Deals 
                                     Where Deals.DealNumber = N'{dealNumber}'";

            var d = baseContext.Database.SqlQuery<String>(sqlExpression);

            using (SqlConnection connection = new SqlConnection(baseContext.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if(reader.GetValue(0).ToString() == dealNumber)
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }

        /// <summary>
        /// Добавление в БД новой сделки
        /// </summary>
        /// <param name="deal"></param>
       public void AddToDB(Deal deal)
        {
            string sqlExpression = $@"UPDATE [dbo].[JSONDatas]
                                    SET [Data] = JSON_MODIFY([Data], 'append$.content',  
                                    JSON_QUERY(N'{JsonConvert.SerializeObject(deal)}'))";

            using (SqlConnection connection = new SqlConnection(baseContext.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();                
            }

        }

    }
    
}
